using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RiskAssessment.Entity.DbEntities;
using RiskAssessment.DAO;
using StackExchange.Redis;
using RiskAssessment.Api.Common;
using RiskAssessment.Entity.DTO;
using AutoMapper;
using RiskAssessment.Api.Validator;
using RiskAssessment.Api.Models;
using RiskAssessment.Entity.CommonModel;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using RiskAssessment.Entity.ViewModel;
using FlexCel.XlsAdapter;
using FlexCel.Report;

namespace RiskAssessment.Api.Controllers
{
    [ApiController]
    [Route("AuditFacility")]
    public class AuditFacilitiesController : BaseController
    {
        protected readonly IConfiguration _config;
        public AuditFacilitiesController(ILoggerManager logger
            , IUnitOfWork uow
            , IDatabase redisDb
            , IConfiguration config
            , IMapper mapper
            , IDatabase iDb) : base(logger, uow, mapper, iDb, config)
        {
            _config = config;
        }
        private void FillParent(AuditFacilityDto obj)
        {
            if (obj.Parent != null)
            {
                var p = _uow.AuditFacilities.FirstOrDefault(a => a.ID == obj.Parent.ID && !obj.Deleted);
                if (p != null)
                {
                    var pm = _mapper.Map<AuditFacilityDto>(p);
                    FillParent(pm);
                    obj.Parent = pm;
                }
            }
        }

        private AuditFacilityDto GetParent(AuditFacilityDto obj)
        {
            var item = obj;
            while (item.Parent != null)
                item = item.Parent;
            return item;
        }
        private List<AuditFacilityDto> GetChilds(AuditFacilityDto root, List<AuditFacilityDto> data)
        {
            var childs = new List<AuditFacilityDto>();
            if (root == null)
                return null;
            data.ForEach(o =>
            {
                var currentObj = o;
                var parent = o.Parent;
                while (parent != null && parent.ID != root.ID)
                {
                    currentObj = parent;
                    parent = parent.Parent;
                }

                if (parent != null && !childs.Exists(a => a.ID == currentObj.ID))
                    childs.Add(currentObj);

            });
            var ancestor_parent = root.ancestor;
            childs.ForEach(o =>
            {
                o.ancestor = ancestor_parent + ">" + "|" + o.ID + "|";
                o.Childs = GetChilds(o, data);
            });

            return childs;
        }
        private void ClearCycle(AuditFacilityDto o)
        {
            o.Parent = null;
            o.Childs.ForEach(a => ClearCycle(a));
        }

        [HttpGet("DownloadTemp")]
        public IActionResult DonwloadFile(string fileName = "")
        {
            var pathImportTemps = _config["Upload:ImportTemplates"];
            var fullPath = Path.Combine(pathImportTemps, fileName);

            var fileInfo = new FileInfo(fullPath);

            if (!fileInfo.Exists || fileInfo.LastWriteTime < DateTime.Now.AddMinutes(-1))
            {
                var bsTemps = _config["Upload:BaseTemplates"];
                var basePath = Path.Combine(bsTemps, fileName);
                var unitTypes = _uow.SystemCategories.GetUnitTypes().ToList();
                var result = new XlsFile(true);
                result.Open(basePath);
                var fr = new FlexCelReport();
                fr.AddTable("dt", unitTypes);
                fr.Run(result);
                fr.Dispose();

                result.Save(fullPath);
            }
            var code = GenCodeDownload(fileName);
            if (string.IsNullOrEmpty(code))
                return NotFound();

            return Ok(new { code });
        }
        [AllowAnonymous]
        [HttpGet("Download")]
        public IActionResult DownloadTemp(string code = "")
        {
            var fullPath = GetTempPath(code, out string fileName);

            if (string.IsNullOrEmpty(fullPath) || string.IsNullOrEmpty(fileName))
                return NotFound();

            var fs = new FileStream(fullPath, FileMode.Open);

            return File(fs, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost("Upload"), DisableRequestSizeLimit]
        public IActionResult Upload()
        {
            try
            {
                var file = Request.Form.Files[0];

                string[] contentType = new string[] { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/vnd.ms-excel" };
                var formats = new string[] { ".xlsx", ".xls" };

                if (!contentType.Any(o => o == file.ContentType) || !formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase)))
                {
                    return BadRequest("File type is not allow!");
                }

                var sizeConfig = string.IsNullOrEmpty(_config["Upload:MaxLength"]) ? "10" : _config["Upload:MaxLength"];

                if (!int.TryParse(sizeConfig, out int allowSize))
                {
                    allowSize = 10;
                }

                if (file.Length > 0 && file.Length < allowSize * 1024 * 1024)
                {
                    if (file.Length > 0)
                    {
                        var data = new List<AuditFacilityDto>();

                        using (var fileStream = new MemoryStream())
                        {
                            file.CopyTo(fileStream);
                            var mappingCl = new Dictionary<string, string>() {
                                { "STT","No"},
                                {"Mã đơn vị","code" },
                                {"Mã đơn vị cấp cha","parent_code" },
                                {"Tên đơn vị*","name" },
                                {"Loại đơn vị*","object_class_name" },
                                {"Trạng thái*","status" }
                            };
                            data = Utils.ExcelFn.UploadToList<AuditFacilityDto>(fileStream, 0, true, 2, mappingCl);
                        }

                        bool hasError = false;
                        //var trans = _uow.BeginTransaction();
                        var userInfo = HttpContext.Items["UserInfo"] as User;

                        var allCode = data.Where(o => o.Code != null && o.Code != "").Select(o => o.Code).Distinct().ToList();

                        var codeInDb = _uow.AuditFacilities.Find(o => o.DomainId == userInfo.DomainId && o.Code != null && o.Code != "" && !o.Deleted).Select(o => o.Code).Distinct().ToList();

                        var s = codeInDb.Where(o => allCode.Contains(o)).Select(o => o).Distinct().ToList();

                        var parentCode = data.Where(o => !string.IsNullOrEmpty(o.ParentCode)).Select(o => o.ParentCode).ToList();

                        var allParentCode = codeInDb.Select(o => o).ToList();

                        allParentCode.AddRange(allCode);

                        var categories = _uow.SystemCategories.GetUnitTypes().ToList();


                        data.ForEach(o =>
                        {
                            o.Valid = true;
                            if (string.IsNullOrEmpty(o.Name))
                            {
                                o.Valid = false;
                                o.Note += "803,";
                            }
                            if (string.IsNullOrEmpty(o.Code))
                            {
                                o.Valid = false;
                                o.Note += "802,";
                            }
                            if (string.IsNullOrEmpty(o.ObjectClassName))
                            {
                                o.Valid = false;
                                o.Note += "805,";
                            }
                            if (!string.IsNullOrEmpty(o.Code) && s.Contains(o.Code))
                            {
                                if (s.Contains(o.Code))
                                {
                                    o.Valid = false;
                                    o.Note += "801,";
                                }

                                var countInExcel = allCode.Count(a => a == o.Code);
                                if (countInExcel > 1)
                                {
                                    o.Valid = false;
                                    o.Note += "807,";
                                }
                            }
                            if (!string.IsNullOrEmpty(o.ParentCode) && (!allParentCode.Contains(o.ParentCode)))
                            {
                                o.Valid = false;
                                o.Note += "804,";
                            }
                            if (!string.IsNullOrEmpty(o.ParentCode) && o.Code == o.ParentCode)
                            {
                                o.Valid = false;
                                o.Note += "806,";
                            }
                            var cate = categories.FirstOrDefault(a => a.name == o.ObjectClassName);
                            if (cate == null)
                            {
                                o.Valid = false;
                                o.Note += "805,";
                            }
                            else
                            {
                                o.ObjectClassId = cate.id;
                                o.ObjectClassName = cate.name;
                            }

                            if (string.IsNullOrEmpty(o.ParentCode) || allParentCode.Contains(o.ParentCode))
                                o.Batch = 0;
                            else o.Batch = 1;
                            if (!string.IsNullOrEmpty(o.ParentCode))
                            {
                                var pr = _uow.AuditFacilities.FirstOrDefault(a => a.DomainId == userInfo.DomainId && a.Code != "" && a.Code == o.ParentCode && !a.Deleted);
                                if (pr != null)
                                    o.Parent = _mapper.Map<AuditFacilityDto>(pr);
                            }

                            if (o.Valid)
                                o.Note = "000";
                        });

                        hasError = data.Any(o => !o.Valid);
                        var anyValid = data.Any(o => o.Valid);
                        if (!hasError)
                        {
                            if (anyValid)
                            {

                                List<AuditFacility> newlist = data.Select(p => _mapper.Map<AuditFacility>(p)).ToList();
                                var batch0 = newlist.Where(o => o.Valid && o.Batch == 0).ToList();
                                foreach (var item in batch0)
                                {
                                    var obj = item;
                                    obj.UserCreate = userInfo.Id;
                                    obj.CreateDate = DateTime.Now;
                                    obj.DomainId = userInfo.DomainId;
                                    _uow.AuditFacilities.Add(obj);
                                    _uow.AuditFacilities.Save();
                                    var child = newlist.Where(o => !string.IsNullOrEmpty(o.ParentCode) && o.ParentCode == obj.Code).ToList();
                                    child.ForEach(o =>
                                    {
                                        o.Parent = obj;
                                        o.ParentId = obj.ID;
                                    });
                                }

                                var batch1 = newlist.Where(o => o.Valid && o.Batch == 1).ToList();
                                foreach (var item in batch1)
                                {
                                    var obj = item;
                                    obj.UserCreate = userInfo.Id;
                                    obj.CreateDate = DateTime.Now;
                                    obj.DomainId = userInfo.DomainId;
                                    _uow.AuditFacilities.Add(obj);
                                    _uow.AuditFacilities.Save();
                                }
                                _uow.AuditFacilities.Save();
                            }
                            return Ok(new { code = "001", data = data, total = data.Count });
                        }
                        else
                        {
                            return Ok(new { code = "800", data = data, total = data.Count });
                        }
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest("File too large!");
                }
            }
            catch
            {
                return Ok(new { code = "800" });
            }
        }

        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            var obj = JsonSerializer.Deserialize<AuditFacilitySearch>(jsonData);
            var userInfo = HttpContext.Items["UserInfo"] as User;
            if (userInfo == null)
            {
                return Unauthorized();
            }
            obj.DomainId = userInfo.DomainId;

            var result = _uow.AuditFacilities.Search(obj, out int count).ToList().Select(o => _mapper.Map<AuditFacilityDto>(o)).ToList();
            var roots = new List<AuditFacilityDto>();

            result.ForEach(o =>
            {
                o.Childs.Clear();
                FillParent(o);

                var root = GetParent(o);
                if (!roots.Exists(a => a.ID == root.ID))
                    roots.Add(root);
            });

            roots.ForEach(o =>
            {
                o.ancestor = "|" + o.ID + "|";
                o.Childs = GetChilds(o, result);
                ClearCycle(o);
            });

            return Ok(new { code = "001", msg = "success", data = roots, total = count });
        }
        [HttpGet("Parents")]
        public IActionResult GetParents()
        {
            var userInfo = HttpContext.Items["UserInfo"] as User;
            if (userInfo == null)
            {
                return Unauthorized();
            }
            var x = _uow.AuditFacilities.Find(o => o.Status && o.DomainId == userInfo.DomainId && (o.ParentId == null || o.ParentId == 0)).ToList();
            var lst = x.Select(o => _mapper.Map<AuditFacilityDto>(o)).ToList();

            return Ok(new { code = "001", msg = "success", data = lst, total = lst.Count });
        }

        [HttpPost]
        public IActionResult UpdateItem(AuditFacilityDto obj)
        {
            var current_id = obj.ID;

            var userInfo = HttpContext.Items["UserInfo"] as User;
            if (userInfo == null)
            {
                return Unauthorized();
            }

            if (!obj.IsValid())
                return BadRequest(new { msg = "Invalid name!" });

            AuditFacility prEntity = null;

            if (obj.ParentId != null && obj.ParentId > 0)
            {
                prEntity = _uow.AuditFacilities.FirstOrDefault(o => o.ID == obj.ParentId);
                if (prEntity != null)
                    obj.ParentName = prEntity.Name;
            }
            var checkByCode = _uow.AuditFacilities.CheckValidCode(obj.Code, obj.ID, userInfo.DomainId);
            if (!checkByCode)
                return Ok(new { code = "003", msg = "success", data = obj });

            UnitTypeView objClass = null;

            if (obj.ObjectClassId != null && obj.ObjectClassId > 0)
            {
                objClass = _uow.SystemCategories.GetUnitTypes().FirstOrDefault(o => o.id == obj.ObjectClassId);
                if (objClass != null)
                    obj.ObjectClassName = objClass.name;
            }

            if (obj.ID > 0)
            {
                var ex = _uow.AuditFacilities.FirstOrDefault(o => o.ID == obj.ID);
                if (!_uow.AuditFacilities.CheckPermission(ex, userInfo.DomainId))
                    return Forbid();
                obj.DomainId = userInfo.DomainId;
                ex = obj.Map(ex);
                ex.ModifiedBy = userInfo.Id;
                ex.LastModified = DateTime.Now;
                //ex.AuditFacilityID = prEntity == null ? 0 : prEntity.ID;
                ex.ParentId = prEntity == null ? null : prEntity.ID;
                ex.ParentName = prEntity == null ? "" : prEntity.Name;
                ex.ObjectClassId = objClass == null ? null : objClass.id;
                ex.ObjectClassName = objClass == null ? null : objClass.name;
                _uow.AuditFacilities.Update(ex);
                obj = _mapper.Map<AuditFacilityDto>(ex);
            }
            else
            {


                var item = _mapper.Map<AuditFacility>(obj);
                item.ParentId = prEntity == null ? null : prEntity.ID;
                item.ParentName = prEntity == null ? "" : prEntity.Name;
                item.ObjectClassId = objClass == null ? null : objClass.id;
                item.ObjectClassName = objClass == null ? null : objClass.name;
                item.UserCreate = userInfo.Id;
                item.CreateDate = DateTime.Now;
                item.DomainId = userInfo.DomainId;
                _uow.AuditFacilities.Add(item);
                obj = _mapper.Map<AuditFacilityDto>(item);
            }

            if (!obj.Status)
            {
                ActiveByParent(obj.ID, obj.Status, userInfo);
            }

            return Ok(new { code = "001", msg = "success", data = obj, id = current_id });
        }

        private void ActiveByParent(int itemId, bool status, User userInfo)
        {
            var c = _uow.AuditFacilities.FirstOrDefault(o => o.ID == itemId);
            if (c == null)
                return;
            c.Status = status;
            c.ModifiedBy = userInfo.Id;
            c.LastModified = DateTime.Now;
            _uow.AuditFacilities.Update(c);
            var childs = _uow.AuditFacilities.Find(o => o.ParentId == c.ID && o.Status == !status).ToList();
            foreach (var cd in childs)
            {
                ActiveByParent(cd.ID, status, userInfo);
            }
        }
        private void DeleteByParent(AuditFacility c, bool status, User userInfo)
        {
            c.Deleted = true;
            c.ModifiedBy = userInfo.Id;
            c.LastModified = DateTime.Now;
            var childs = _uow.AuditFacilities.Find(o => o.ParentId == c.ID && !o.Deleted).ToList();
            foreach (var cd in childs)
            {
                DeleteByParent(cd, status, userInfo);
            }
            _uow.AuditFacilities.Update(c);
        }


        [HttpPost("Delete")]
        public IActionResult DeleteItem(AuditFacilityDto obj)
        {
            var userInfo = HttpContext.Items["UserInfo"] as User;
            if (userInfo == null)
            {
                return Unauthorized();
            }
            var item = _uow.AuditFacilities.FirstOrDefault(o => o.ID == obj.ID);
            if (item == null || !_uow.AuditFacilities.CheckPermission(item, userInfo.DomainId))
                return NotFound();
            var child = _uow.AuditFacilities.Find(o => !o.Deleted && o.ParentId == obj.ID).ToList();

            if (child != null && child.Any())
                return Ok(new { code = "008" });

            var process = _uow.AuditProcess.Find(o => o.FacilityId == item.ID && !o.Deleted);

            if (process != null && process.Any())
                return Ok(new { code = "007" });

            var hasDone = _uow.AssessmentResults.CheckBoardCompleteForItem("DV", item.ID, userInfo.DomainId);
            if (hasDone)
                return Ok(new { code = "009" });

            item.Deleted = true;
            item.ModifiedBy = userInfo.Id;
            item.LastModified = DateTime.Now;
            _uow.AuditFacilities.Update(item);

            return Ok(new { code = "001", msg = "success", data = obj });
        }

        [HttpGet("{id}")]
        public IActionResult GetItem(int id)
        {
            var item = _uow.AuditFacilities.FirstOrDefault(o => o.ID == id);
            var userInfo = HttpContext.Items["UserInfo"] as User;
            if (item == null || !_uow.AuditFacilities.CheckPermission(item, userInfo.DomainId))
                return NotFound();
            var result = _mapper.Map<AuditFacilityDto>(item);


            if (result.UserCreate != null)
            {
                result.NameCreate = _uow.SystemCategories.GetUserFullName(result.UserCreate.Value);
            }
            if (result.ModifiedBy != null)
            {
                result.NameModified = _uow.SystemCategories.GetUserFullName(result.ModifiedBy.Value);
            }
            if (result.CreateDate.HasValue)
                result.CreateDateStr = result.CreateDate.Value.ToString("dd/MM/yyyy HH:mm:ss");
            if (result.LastModified.HasValue)
                result.LastModifiedStr = result.LastModified.Value.ToString("dd/MM/yyyy HH:mm:ss");


            return Ok(new { code = "001", msg = "success", data = result });
        }

    }
}
