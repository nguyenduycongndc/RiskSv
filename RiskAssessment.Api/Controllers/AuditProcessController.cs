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
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using OfficeOpenXml;
using System.Text.RegularExpressions;

namespace RiskAssessment.Api.Controllers
{
    [ApiController]
    [Route("AuditProcess")]
    public class AuditProcessController : BaseController
    {
        protected readonly IConfiguration _config;
        public AuditProcessController(ILoggerManager logger
            , IUnitOfWork uow
            , IDatabase redisDb
            , IMapper mapper
            , IConfiguration config
            , IDatabase iDb) : base(logger, uow, mapper, iDb, config)
        {
            _config = config;
        }
        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            var obj = JsonSerializer.Deserialize<AuditProcessSearch>(jsonData);
            var userInfo = HttpContext.Items["UserInfo"] as User;
            if (userInfo == null)
            {
                return Unauthorized();
            }

            obj.DomainId = userInfo.DomainId;
            var lst = _uow.AuditProcess.Search(obj, out int count).ToList().Select(o => _mapper.Map<AuditProcessDto>(o)).ToList();

            return Ok(new { code = "001", msg = "success", data = lst, total = count });
        }
        [HttpGet("DownloadTemp")]
        public IActionResult DonwloadFile(string fileName = "")
        {
            var code = GenCodeDownload(fileName);
            if (string.IsNullOrEmpty(code))
                return NotFound();

            return Ok(new { code });
        }

        /// <summary>
        /// Hàm downloadTemp này được chỉnh sửa để bao gồm tên hoạt động, đơn vị vào file excel
        /// Đơn vị sẽ được sắp xếp tương ứng theo quan hệ cha con, con sẽ được đặt bên dưới cha
        /// Mã và tên được ngăn cái bởi 1 chuỗi " - "
        /// Quan hệ con được biểu hiện bằng ký tự '>', số lượng ký tự tùy theo độ sâu của node con
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("Download")]
        public IActionResult DownloadTemp(string code = "")
        {
            var fullPath = GetTempPath(code, out string fileName);
            var userInfo = HttpContext.Items["UserInfo"] as User;
            if (string.IsNullOrEmpty(fullPath) || string.IsNullOrEmpty(fileName))
                return NotFound();

            byte[] fileBytesArray;
            var facilities = _uow.AuditFacilities
                .Find(o => o.Status && o.Deleted != true && o.DomainId == userInfo.DomainId).ToArray();
            var activities = _uow.BsActRepo.Find(o => o.Status && o.Deleted != true && o.DomainId == userInfo.DomainId).ToArray();

            var facilityDictionary = new Dictionary<int, int?>();
            var activityDictionary = new Dictionary<int, int?>();
            facilityDictionary = facilities.ToDictionary(x => x.ID, x => x.ParentId);
            activityDictionary = activities.ToDictionary(x => x.ID, x => x.ParentId);
           

            var lookup = facilities.ToLookup(x => x.ParentId);
            IEnumerable<AuditFacility> FlattenAuditFacility(int? parentId)
            {
                foreach (var node in lookup[parentId])
                {
                    yield return node;
                    foreach (var child in FlattenAuditFacility(node.ID))
                    {
                        yield return child;
                    }
                }
            }

            facilities = FlattenAuditFacility(null).ToArray();

            var lookupActivity = activities.ToLookup(x => x.ParentId);
            IEnumerable<BussinessActivity> FlattenBusinessActivity(int? parentId)
            {
                foreach (var node in lookupActivity[parentId])
                {
                    yield return node;
                    foreach (var child in FlattenBusinessActivity(node.ID))
                    {
                        yield return child;
                    }
                }
            }

            activities = FlattenBusinessActivity(null).ToArray();

            using (var pck = new ExcelPackage())
            {
                using (var stream = System.IO.File.OpenRead(fullPath))
                {
                    pck.Load(stream);

                    var ws = pck.Workbook.Worksheets[1];
                    foreach (var (value, i) in facilities.Select((value, i) => (value, i)))
                    {
                        ExcelRange cell = ws.Cells[(1 + i), 1];
                        cell.Value = value.Code + " - " + value.Name;
                        if (value.ParentId != null)
                        {
                            var depth = DepthLenghtFromRootLevel(facilityDictionary, (int)value.ParentId);
                            for (int j = 1; j <= depth; j++)
                            {
                                cell.Value = ">" + cell.Value;
                            }
                        }

                    }
                    ws = pck.Workbook.Worksheets[2];
                    foreach (var (value, i) in activities.Select((value, i) => (value, i)))
                    {
                        ExcelRange cell = ws.Cells[(1 + i), 1];
                        cell.Value = value.Code + " - " + value.Name;
                        if (value.ParentId != null)
                        {
                            var depth = DepthLenghtFromRootLevel(activityDictionary, (int)value.ParentId);
                            for (int j = 1; j <= depth; j++)
                            {
                                cell.Value = ">" + cell.Value;
                            }
                        }
                    }


                    fileBytesArray = pck.GetAsByteArray();
                }
            }

            return File(fileBytesArray, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }


        /// <summary>
        /// Hàm này dành cho việc import file Excel
        /// Mã và tên được ngăn cái bởi 1 chuỗi " - "
        /// Quan hệ con được biểu hiện bằng ký tự '>', số lượng ký tự tùy theo độ sâu của node con
        /// </summary>
        /// <returns></returns>
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
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "Temps");
                    if (file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString().Replace("-", "") + System.IO.Path.GetExtension(file.FileName);
                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine("Temps", fileName);
                        var data = new List<AuditProcessDto>();
                        var hasError = false;
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                            using (var fileStream = new MemoryStream())
                            {
                                file.CopyTo(fileStream);
                                var mappingCl = new Dictionary<string, string>() {
                                { "STT","No"},
                                {"Mã quy trình*","code" },
                                {"Tên quy trình*","name" },
                                {"Hoạt động liên quan*","activity_code" },
                                {"Đơn vị liên quan*","facility_code" },
                                {"Họ tên người phụ trách","person_charge" },
                                {"Email người phụ trách" ,"person_charge_email"},
                                {"Trạng thái*","risk_type_name" },
                                {"Mô tả quy trình","description" }
                            };
                                data = Utils.ExcelFn.UploadToListProcess<AuditProcessDto>(fileStream, 0, true, 2, mappingCl);
                            }
                        }


                        var dub = data.GroupBy(x => x.Code)
                                      .Where(g => g.Count() > 1)
                                      .Select(y => y.Key)
                                      .ToList();
                        //var trans = _uow.BeginTransaction();
                        var userInfo = HttpContext.Items["UserInfo"] as User;

                        var activityCodes = data.Where(o => !string.IsNullOrEmpty(o.ActivityCode)).Select(o => o.ActivityCode.Split(" - ")[0].Replace(">", "")).ToArray();
                        var facilityCodes = data.Where(o => !string.IsNullOrEmpty(o.FacilityCode)).Select(o => o.FacilityCode.Split(" - ")[0].Replace(">", "")).ToArray();

                        var activities = _uow.BsActRepo.Find(o => o.Status && o.DomainId == userInfo.DomainId && activityCodes.Contains(o.Code) && o.Deleted != true).ToArray();
                        var facilities = _uow.AuditFacilities.Find(o => o.Status && o.DomainId == userInfo.DomainId && facilityCodes.Contains(o.Code) && o.Deleted != true).ToArray();
                        var exCode = _uow.AuditProcess.Find(o => o.DomainId == userInfo.DomainId && o.Deleted != true).Select(o => o.Code).ToArray();
                        string[] statuses = { "Dừng hoạt động", "Đang hoạt động" };
                        data.ForEach(o =>
                        {
                            o.Valid = true;
                            if (string.IsNullOrEmpty(o.Code))
                            {
                                o.Valid = false;
                                o.Note += "EmptyCode,";
                            }
                            if (string.IsNullOrEmpty(o.Name))
                            {
                                o.Valid = false;
                                o.Note += "EmptyName,";
                            }
                            if (exCode.Contains(o.Code))
                            {
                                o.Valid = false;
                                o.Note += "DublicateCode,";
                            }
                            if (dub.Contains(o.Code))
                            {
                                o.Valid = false;
                                o.Note += "DublicateCodeInExcel,";
                            }

                            if (!string.IsNullOrEmpty(o.PersonChargeEmail))
                            {
                                if (!Regex.IsMatch(o.PersonChargeEmail, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
                                {
                                    o.Valid = false;
                                    o.Note += "InvalidMail,";
                                }
                            }

                            if (o.FacilityCode == null || o.FacilityCode == "")
                            {
                                o.Valid = false;
                                o.Note += "FacilityEmpty,";
                            }
                            else
                            {
                                string[] slitedFacilities = !string.IsNullOrEmpty(o.FacilityCode) ? o.FacilityCode.Split(" - ") : null;
                                o.FacilityCode = slitedFacilities[0].Replace(">", "");
                                var fac = facilities.FirstOrDefault(a => a.Code == o.FacilityCode);
                                if (fac == null)
                                {
                                    o.FacilityName = o.FacilityCode;
                                    o.Valid = false;
                                    o.Note += "FacilityNotFound,";
                                }
                                else
                                {
                                    o.FacilityName = fac.Name;
                                    o.FacilityId = fac.ID;
                                }
                            }

                            
                            if (o.ActivityCode == null || o.ActivityCode == "")
                            {
                                o.Valid = false;
                                o.Note += "ActivityEmpty,";
                            }
                            else
                            {
                                string[] splittedActivities = o.ActivityCode.Split(" - ");
                                o.ActivityCode = splittedActivities[0].Replace(">", "");
                                var act = activities.FirstOrDefault(a => a.Code == o.ActivityCode);
                                if (act == null)
                                {
                                    o.ActivityName = o.ActivityCode;
                                    o.Valid = false;
                                    o.Note += "ActivityNotFound,";
                                }
                                else
                                {
                                    o.ActivityName = act.Name;
                                    o.ActivityId = act.ID;
                                }
                            }

                            if (statuses.Contains(o.RiskTypeName))
                            {
                                if (o.RiskTypeName == statuses[0])
                                {
                                    o.Status = false;
                                }
                                else
                                {
                                    o.Status = true;
                                }
                            }
                            else
                            {
                                o.Valid = false;
                                o.Note += "StatusNotValid,";
                            }
                        }
                        );


                        hasError = data.Any(o => !o.Valid);

                        if (!hasError)
                        {
                            data.ForEach(o =>
                            {
                                var obj = _mapper.Map<AuditProcess>(o);
                                obj.UserCreate = userInfo.Id;
                                obj.CreateDate = DateTime.Now;
                                obj.DomainId = userInfo.DomainId;
                                _uow.AuditProcess.Add(obj);
                                o.Note = "000";
                            });
                        }
                        else
                        {
                            return Ok(new { code = "800", data = data, total = data.Count, fileName = fileName });
                        }


                        return Ok(new { code = "001", data = data, total = data.Count, fileName = fileName });
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
        [HttpPost]
        public IActionResult UpdateItem(AuditProcessDto obj)
        {
            var current_id = obj.ID;
            var userInfo = HttpContext.Items["UserInfo"] as User;
            if (userInfo == null)
            {
                return Unauthorized();
            }

            if (!obj.IsValid())
                return BadRequest(new { msg = "Invalid name!" });

            AuditFacility auditFacEntity = null;
            BussinessActivity activity = null;

            if (obj.FacilityId > 0)
            {
                auditFacEntity = _uow.AuditFacilities.FirstOrDefault(o => o.ID == obj.FacilityId);
            }
            if (obj.ActivityId > 0)
            {
                activity = _uow.BsActRepo.FirstOrDefault(o => o.ID == obj.ActivityId);
            }

            if (auditFacEntity == null || activity == null)
                return BadRequest();

            if (!_uow.AuditProcess.CheckValidCode(obj.Code, obj.ID, userInfo.DomainId))
                return Ok(new { code = "003", msg = "", data = obj });

            obj.FacilityName = auditFacEntity.Name;
            obj.ActivityName = activity.Name;

            if (!obj.IsValid())
                return BadRequest(new { msg = "Invalid data!" });

            if (obj.ID > 0)
            {
                var ex = _uow.AuditProcess.FirstOrDefault(o => o.ID == obj.ID);

                if (!_uow.AuditProcess.CheckPermission(ex, userInfo.DomainId))
                    return Forbid();

                obj.DomainId = userInfo.DomainId;
                ex = obj.Map(ex);
                ex.ModifiedBy = userInfo.Id;
                ex.LastModified = DateTime.Now;
                ex.FacilityId = auditFacEntity.ID;
                ex.ActivityId = activity.ID;
                ex.FacilityName = auditFacEntity.Name;
                ex.ActivityName = activity.Name;
                _uow.AuditProcess.Update(ex);
                obj = _mapper.Map<AuditProcessDto>(ex);
            }
            else
            {
                var item = _mapper.Map<AuditProcess>(obj);
                item.UserCreate = userInfo.Id;
                item.CreateDate = DateTime.Now;
                item.DomainId = userInfo.DomainId;
                _uow.AuditProcess.Add(item);
                obj = _mapper.Map<AuditProcessDto>(item);
            }

            return Ok(new { code = "001", msg = "success", data = obj, id = current_id });
        }

        [HttpPost("Delete")]
        public IActionResult DeleteItem(AuditProcessDto obj)
        {
            var userInfo = HttpContext.Items["UserInfo"] as User;
            if (userInfo == null)
            {
                return Unauthorized();
            }
            var item = _uow.AuditProcess.FirstOrDefault(o => o.ID == obj.ID);
            if (item == null)
                return NotFound();
            ////if (item.UserCreate != userInfo.Id)
            ////{
            ////    _logger.LogError($"{userInfo.UserName} delete 'business activity' {item.ID}: don't have permission!");
            ////    return BadRequest();
            ////}

            var hasDone = _uow.AssessmentResults.CheckBoardCompleteForItem("QT", item.ID, userInfo.DomainId);
            if (hasDone)
                return Ok(new { code = "009" });

            item.Deleted = true;
            item.ModifiedBy = userInfo.Id;
            item.LastModified = DateTime.Now;
            _uow.AuditProcess.Update(item);
            obj.Status = false;
            return Ok(new { code = "001", msg = "success", data = obj });
        }

        [HttpGet("{id}")]
        public IActionResult GetItem(int id)
        {
            var userInfo = HttpContext.Items["UserInfo"] as User;
            var item = _uow.AuditProcess.FirstOrDefault(o => o.ID == id);
            if (item == null)
                return NotFound();
            if (!_uow.AuditProcess.CheckPermission(item, userInfo.DomainId))
                return Forbid();
            var result = _mapper.Map<AuditProcessDto>(item);

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
