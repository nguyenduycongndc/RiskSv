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
using System.IO;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace RiskAssessment.Api.Controllers
{
    [ApiController]
    [Route("BussinessActivity")]
    public class BussinessActivitiesController : BaseController
    {
        protected readonly IConfiguration _config;
        public BussinessActivitiesController(ILoggerManager logger
            , IUnitOfWork uow
            , IDatabase redisDb
            , IMapper mapper
            , IConfiguration config
            , IDatabase iDb) : base(logger, uow, mapper, iDb, config)
        {
            _config = config;
        }

        [HttpGet("DownloadTemp")]
        public IActionResult DonwloadFile(string fileName = "")
        {
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
                        var data = new List<BussinessActivityDto>();

                        using (var fileStream = new MemoryStream())
                        {
                            file.CopyTo(fileStream);
                            var mappingCl = new Dictionary<string, string>() {
                                { "STT","No"},
                                {"Mã hoạt động","code" },
                                {"Mã hoạt động cấp cha","parent_code" },
                                {"Tên hoạt động*","name" },
                                {"Trạng thái*","status" },
                                {"Mô tả","description" }
                            };
                            data = Utils.ExcelFn.UploadToList<BussinessActivityDto>(fileStream, 0, true, 2, mappingCl);
                        }

                        bool hasError = false;
                        //var trans = _uow.BeginTransaction();
                        var userInfo = HttpContext.Items["UserInfo"] as User;
                        var allCode = data.Where(o => o.Code != null && o.Code != "").Select(o => o.Code).Distinct().ToList();

                        var codeInDb = _uow.BsActRepo.Find(o => o.DomainId == userInfo.DomainId && o.Code != null && o.Code != "" && !o.Deleted).Select(o => o.Code).Distinct().ToList();

                        var s = codeInDb.Where(o => allCode.Contains(o)).Select(o => o).Distinct().ToList();

                        var parentCode = data.Where(o => !string.IsNullOrEmpty(o.ParentCode)).Select(o => o.ParentCode).ToList();

                        var allParentCode = codeInDb.Select(o => o).ToList();

                        allParentCode.AddRange(allCode);

                        data.ForEach(o =>
                        {
                            o.Valid = true;

                            if (string.IsNullOrEmpty(o.Code))
                            {
                                o.Valid = false;
                                o.Note += "802,";
                            }
                            if (string.IsNullOrEmpty(o.Name))
                            {
                                o.Valid = false;
                                o.Note += "803,";
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


                            if (string.IsNullOrEmpty(o.ParentCode) || allParentCode.Contains(o.ParentCode))
                                o.Batch = 0;
                            else o.Batch = 1;
                            if (!string.IsNullOrEmpty(o.ParentCode))
                            {
                                var pr = _uow.BsActRepo.FirstOrDefault(a => a.DomainId == userInfo.DomainId && a.Code != "" && a.Code == o.ParentCode && !a.Deleted);
                                if (pr != null)
                                    o.Parent = _mapper.Map<BussinessActivityDto>(pr);
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
                                List<BussinessActivity> newlist = data.Select(p => _mapper.Map<BussinessActivity>(p)).ToList();
                                var batch0 = newlist.Where(o => o.Valid && o.Batch == 0).ToList();
                                foreach (var item in batch0)
                                {
                                    var obj = item;
                                    obj.UserCreate = userInfo.Id;
                                    obj.CreateDate = DateTime.Now;
                                    obj.DomainId = userInfo.DomainId;
                                    _uow.BsActRepo.AddWithoutSave(obj);
                                    _uow.BsActRepo.Save();
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
                                    _uow.BsActRepo.AddWithoutSave(obj);
                                    _uow.BsActRepo.Save();
                                }
                                _uow.BsActRepo.Save();
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
            catch (Exception ex)
            {
                return Ok(new { code = "800" });
            }
        }

        private void FillParent(BussinessActivityDto obj)
        {
            if (obj.Parent != null)
            {
                var p = _uow.BsActRepo.FirstOrDefault(a => a.ID == obj.Parent.ID);
                if (p != null)
                {
                    var pm = _mapper.Map<BussinessActivityDto>(p);
                    FillParent(pm);
                    obj.Parent = pm;
                }
            }
        }
        private BussinessActivityDto GetParent(BussinessActivityDto obj)
        {
            var item = obj;
            while (item.Parent != null)
                item = item.Parent;
            return item;
        }
        private List<BussinessActivityDto> GetChilds(BussinessActivityDto root, List<BussinessActivityDto> data)
        {
            var childs = new List<BussinessActivityDto>();
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
                o.SubActivities = GetChilds(o, data);
            });

            return childs;
        }
        private void ClearCycle(BussinessActivityDto o)
        {
            o.Parent = null;
            o.SubActivities.ForEach(a => ClearCycle(a));
        }
        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            var obj = JsonSerializer.Deserialize<ModelSearch>(jsonData);
            var userInfo = HttpContext.Items["UserInfo"] as User;
            if (userInfo == null)
            {
                return Unauthorized();
            }

            obj.DomainId = userInfo.DomainId;
            var result = _uow.BsActRepo.Search(obj, out int count).ToList().Select(o => _mapper.Map<BussinessActivityDto>(o)).ToList();
            var roots = new List<BussinessActivityDto>();

            result.ForEach(o =>
            {
                o.SubActivities.Clear();
                FillParent(o);
                var root = GetParent(o);
                if (!roots.Exists(a => a.ID == root.ID))
                    roots.Add(root);
            });
            roots.ForEach(o =>
            {
                o.ancestor = "|" + o.ID + "|";
                o.SubActivities = GetChilds(o, result);
                ClearCycle(o);
            });

            return Ok(new { code = "001", msg = "success", data = roots, total = count });
        }

        [HttpPost]
        public IActionResult UpdateItem(BussinessActivityDto obj)
        {
            var current_id = obj.ID;
            var userInfo = HttpContext.Items["UserInfo"] as User;
            if (userInfo == null)
            {
                return Unauthorized();
            }

            if (!obj.IsValid())
                return BadRequest(new { msg = "Invalid name!" });

            BussinessActivity parent = null;

            if (obj.Parent != null && obj.Parent.ID > 0)
            {
                parent = _uow.BsActRepo.FirstOrDefault(o => o.ID == obj.Parent.ID);
                if (parent != null)
                    obj.Parent = _mapper.Map<BussinessActivityDto>(parent);

            }

            if (!_uow.BsActRepo.CheckValidCode(obj.Code, obj.ID, userInfo.DomainId))
                return Ok(new { code = "003", msg = "Invalid code!" });

            if (obj.ID > 0)
            {
                var ex = _uow.BsActRepo.FirstOrDefault(o => o.ID == obj.ID);
                if (!_uow.BsActRepo.CheckPermission(ex, userInfo.DomainId))
                    return Forbid();
                ex = obj.Map(ex);

                ex.ParentId = parent?.ID;
                ex.ModifiedBy = userInfo.Id;
                ex.DomainId = userInfo.DomainId;
                ex.LastModified = DateTime.Now;
                _uow.BsActRepo.Update(ex);
                obj = _mapper.Map<BussinessActivityDto>(ex);
            }
            else
            {
                var item = _mapper.Map<BussinessActivity>(obj);
                item.UserCreate = userInfo.Id;
                item.CreateDate = DateTime.Now;
                item.DomainId = userInfo.DomainId;
                _uow.BsActRepo.Add(item);
                obj = _mapper.Map<BussinessActivityDto>(item);
            }

            if (!obj.Status)
            {
                ActiveByParent(obj.ID, obj.Status, userInfo);
            }

            return Ok(new { code = "001", msg = "success", id = current_id });
        }


        private void ActiveByParent(int itemId, bool status, User userInfo)
        {
            var c = _uow.BsActRepo.FirstOrDefault(o => o.ID == itemId);
            if (c == null)
                return;
            c.Status = status;
            c.ModifiedBy = userInfo.Id;
            c.LastModified = DateTime.Now;
            _uow.BsActRepo.Update(c);
            var childs = _uow.BsActRepo.Find(o => o.ParentId == c.ID && o.Status == !status).ToList();
            foreach (var cd in childs)
            {
                ActiveByParent(cd.ID, status, userInfo);
            }
        }

        [HttpPost("Delete")]
        public IActionResult DeleteItem(BussinessActivityDto obj)
        {
            var userInfo = HttpContext.Items["UserInfo"] as User;
            if (userInfo == null)
            {
                return Unauthorized();
            }
            var item = _uow.BsActRepo.FirstOrDefault(o => o.ID == obj.ID);
            if (item == null)
                return NotFound();
            ////if (item.UserCreate != userInfo.Id)
            ////    return Ok(new { code = "006" });
            var child = _uow.BsActRepo.Find(o => o.ParentId == obj.ID && !o.Deleted);

            if (child != null && child.Any())
                return Ok(new { code = "008" });

            var process = _uow.AuditProcess.Find(o => o.ActivityId == item.ID && !o.Deleted);

            if (process != null && process.Any())
                return Ok(new { code = "007" });

            var hasDone = _uow.AssessmentResults.CheckBoardCompleteForItem("HDKD", item.ID, userInfo.DomainId);
            if (hasDone)
                return Ok(new { code = "009" });

            item.Deleted = true;
            item.ModifiedBy = userInfo.Id;
            item.LastModified = DateTime.Now;
            _uow.BsActRepo.Update(item);

            return Ok(new { code = "001", msg = "success", data = obj });
        }

        [HttpGet("{id}")]
        public IActionResult GetItem(int id)
        {
            var userInfo = HttpContext.Items["UserInfo"] as User;
            var item = _uow.BsActRepo.GetAll().ToList().FirstOrDefault(o => o.ID == id);
            if (item == null || !_uow.BsActRepo.CheckPermission(item, userInfo.DomainId))
                return NotFound();
            var result = _mapper.Map<BussinessActivityDto>(item);
            if (result.Parent != null)
            {
                result.Parent.SubActivities.Clear();
                result.Parent.Parent = null;
            }

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
