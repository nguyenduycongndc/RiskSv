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
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.IO;

namespace RiskAssessment.Api.Controllers
{
    [ApiController]
    [Route("RiskAssessmentScale")]
    public class RiskAssessmentScalesController : BaseController
    {
        public RiskAssessmentScalesController(ILoggerManager logger
            , IUnitOfWork uow
            , IConfiguration config
            , IMapper mapper
            , IDatabase iDb) : base(logger, uow, mapper, iDb, config)
        {
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
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "Temps");
                    if (file.Length > 0)
                    {
                        var _fileName = Guid.NewGuid().ToString().Replace("-", "") + System.IO.Path.GetExtension(file.FileName);
                        var fullPath = Path.Combine(pathToSave, _fileName);
                        ////var dbPath = Path.Combine("Temps", fileName);
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        var _data = Utils.ExcelFn.ExcelToList<RiskAssessmentScaleDto>(fullPath);

                        bool hasError = false;
                        var userInfo = HttpContext.Items["UserInfo"] as User;

                        var categories = _uow.SystemCategories.Find(o => o.DomainId == userInfo.DomainId && o.Status && o.ParentGroup == "DieuKienDinhLuong").ToList();

                        _data.ForEach(o =>
                        {
                            o.Valid = true;
                            if (string.IsNullOrEmpty(o.IssueCode))
                            {
                                o.Valid = false;
                                o.Note += "Issue cannot be null!<br />";
                            }
                            if (string.IsNullOrEmpty(o.Condition))
                            {
                                o.Valid = false;
                                o.Note += "Condition cannot be null!<br />";
                            }
                            if (o.Point <= 0)
                            {
                                o.Valid = false;
                                o.Note += "Point must be greater than 0!<br />";
                            }
                            if ((o.MinValue.HasValue || !string.IsNullOrEmpty(o.LowerCondition))
                            && (!o.MinValue.HasValue || string.IsNullOrEmpty(o.LowerCondition)))
                            {
                                o.Valid = false;
                                o.Note += "Min value and Lower condition must be not null!<br />";
                            }
                            if ((o.MaxValue.HasValue || !string.IsNullOrEmpty(o.UpperCondition))
                            && (!o.MaxValue.HasValue || string.IsNullOrEmpty(o.UpperCondition)))
                            {
                                o.Valid = false;
                                o.Note += "Max value and Upper condition must be not null!<br />";
                            }

                            if (!string.IsNullOrEmpty(o.IssueCode))
                            {
                                var issue = _uow.RiskIssues.FirstOrDefault(a => a.DomainId == userInfo.DomainId && a.Code == o.IssueCode);
                                if (issue == null)
                                {
                                    o.Valid = false;
                                    o.Note += "Issue code is invalid!<br />";
                                }
                                else
                                {
                                    o.RiskIssue = _mapper.Map<RiskIssueDto>(issue);
                                }

                            }

                            if (!string.IsNullOrEmpty(o.LowerCondition))
                            {
                                var ct = categories.FirstOrDefault(a => a.Code == o.LowerCondition);
                                if (ct == null)
                                {
                                    o.Valid = false;
                                    o.Note += "LowerCondition is invalid!<br />";
                                }
                                else o.MinCondition = _mapper.Map<SystemCategoryDto>(ct);
                            }
                            if (!string.IsNullOrEmpty(o.UpperCondition))
                            {
                                var ct = categories.FirstOrDefault(a => a.Code == o.UpperCondition);
                                if (ct == null)
                                {
                                    o.Valid = false;
                                    o.Note += "UpperCondition is invalid!<br />";
                                }
                                else o.MaxCondition = _mapper.Map<SystemCategoryDto>(ct);
                            }

                        });

                        hasError = _data.Any(o => !o.Valid);

                        if (!hasError)
                        {
                            _data.ForEach(o =>
                            {
                                if (o.IsValid())
                                {
                                    var obj = _mapper.Map<RiskAssessmentScale>(o);
                                    obj.UserCreate = userInfo.Id;
                                    obj.CreateDate = DateTime.Now;
                                    obj.DomainId = userInfo.DomainId;
                                    obj.Status = true;
                                    _uow.RiskAssessmentScales.Add(obj);
                                    o.Note = "";
                                }
                                else o.Note = "Data is invalid!";
                            });
                        }

                        return Ok(new { code = "001", data = _data, total = _data.Count, fileName = _fileName });
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
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            var obj = JsonSerializer.Deserialize<RiskIssueSearch>(jsonData);
            if (HttpContext.Items["UserInfo"] is not User userInfo)
            {
                return Unauthorized();
            }
            obj.DomainId = userInfo.DomainId;
            var lst = _uow.RiskAssessmentScales.Search(obj, out int count).Select(o => _mapper.Map<RiskAssessmentScaleDto>(o)).ToList();

            return Ok(new { code = "001", msg = "success", data = lst, total = count });
        }

        [HttpPost]
        public IActionResult UpdateItem(RiskAssessmentScaleDto obj)
        {
            var current_id = obj.ID;
            if (HttpContext.Items["UserInfo"] is not User userInfo)
            {
                return Unauthorized();
            }

            if (!obj.IsValid())
                return Ok(new { code = "202", msg = "invalid risk issue" });
            RiskIssue riskIssue = null;

            if (obj.RiskIssue != null && obj.RiskIssue.ID > 0)
            {
                riskIssue = _uow.RiskIssues.FirstOrDefault(o => o.ID == obj.RiskIssue.ID);
                if (riskIssue != null)
                    obj.RiskIssue = _mapper.Map<RiskIssueDto>(riskIssue);
                else return Ok(new { code = "202", msg = "" });
            }

            if (riskIssue == null)
                return Ok(new { code = "202", msg = "" });


            if ((obj.MinValue > 0 || (obj.MinCondition != null && obj.MinCondition.ID > 0)) && (!obj.MinValue.HasValue /*|| obj.MinValue == 0*/ || obj.MinCondition == null || obj.MinCondition.ID < 0))
            {
                return Ok(new { code = "203", msg = "" });
            }
            if ((obj.MaxValue > 0 || (obj.MaxCondition != null && obj.MaxCondition.ID > 0)) && (!obj.MaxValue.HasValue /*|| obj.MaxValue == 0*/ || obj.MaxCondition == null || obj.MaxCondition.ID < 0))
            {
                return Ok(new { code = "204", msg = "" });
            }

            var lowerConditions = _uow.SystemCategories.Find(o => o.ParentGroup == "DieuKienDuoi" && o.Status && !o.Deleted)
                .Select(o => _mapper.Map<SystemCategoryDto>(o)).ToList();
            var upperConditions = _uow.SystemCategories.Find(o => o.ParentGroup == "DieuKienTren" && o.Status && !o.Deleted)
                .Select(o => _mapper.Map<SystemCategoryDto>(o)).ToList();

            if (obj.MinCondition != null && obj.MinCondition.ID > 0)
            {
                var cd = lowerConditions.FirstOrDefault(o => o.ID == obj.MinCondition.ID);
                if (cd == null)
                    return Ok(new { code = "203", msg = "" });
                obj.MinCondition = cd;
            }
            if (obj.MaxCondition != null && obj.MaxCondition.ID > 0)
            {
                var cd = upperConditions.FirstOrDefault(o => o.ID == obj.MaxCondition.ID);
                if (cd == null)
                    return Ok(new { code = "204", msg = "" });
                obj.MaxCondition = cd;
            }
            obj.DomainId = userInfo.DomainId;
            var hasConfig = _uow.RiskAssessmentScales.HasConfig(obj);
            if (hasConfig)
                return Ok(new { code = "205", msg = "" });

            if (obj.ID > 0)
            {
                var ex = _uow.RiskAssessmentScales.FirstOrDefault(o => o.ID == obj.ID);
                ex = obj.Map(ex);

                ex.MinValue = obj.MinValue;
                ex.LowerCondition = obj.MinCondition == null || obj.MinCondition.ID == 0 ? (int?)null : obj.MinCondition.ID;
                ex.LowerConditionName = obj.MinCondition == null || obj.MinCondition.ID == 0 ? "" : obj.MinCondition.Name;
                ex.MaxValue = obj.MaxValue;
                ex.UpperCondition = obj.MaxCondition == null || obj.MaxCondition.ID == 0 ? (int?)null : obj.MaxCondition.ID;
                ex.UpperConditionName = obj.MaxCondition == null || obj.MaxCondition.ID == 0 ? "" : obj.MaxCondition.Name;
                ex.Point = obj.Point;
                ex.Condition = GetConditionCustom(obj, obj.RiskIssue.MethodId);
                ex.RiskIssueId = riskIssue.ID;

                ex.RiskIssueName = riskIssue.Name;
                ex.RiskIssueCode = riskIssue.Code;
                ex.RiskIssueCodeMethod = riskIssue.MethodId;
                ex.DomainId = userInfo.DomainId;
                ex.ModifiedBy = userInfo.Id;
                ex.LastModified = DateTime.Now;
                _uow.RiskAssessmentScales.Update(ex);
                obj = _mapper.Map<RiskAssessmentScaleDto>(ex);
            }
            else
            {
                var item = _mapper.Map<RiskAssessmentScale>(obj);
                item.UserCreate = userInfo.Id;
                item.CreateDate = DateTime.Now;
                item.DomainId = userInfo.DomainId;
                _uow.RiskAssessmentScales.Add(item);
                obj = _mapper.Map<RiskAssessmentScaleDto>(item);
            }

            return Ok(new { code = "001", msg = "success", data = obj, id = current_id });
        }

        [HttpPost("Delete")]
        public IActionResult DeleteItem(RiskAssessmentScaleDto obj)
        {
            if (HttpContext.Items["UserInfo"] is not User userInfo)
            {
                return Unauthorized();
            }
            var item = _uow.RiskAssessmentScales.FirstOrDefault(o => o.ID == obj.ID);
            if (item == null)
                return NotFound();
            var check  = _uow.ScoreBoard.CheckRiskIssue(item.RiskIssueCode);
            if(check)
                return Ok(new { code = "601", msg = "success", data = obj });

            ////if (item.UserCreate != userInfo.Id)
            ////{
            ////    _logger.LogError($"{userInfo.UserName} delete 'risk assessment scale' {item.ID}: don't have permission!");
            ////    return BadRequest();
            ////}
            item.Deleted = true;
            item.ModifiedBy = userInfo.Id;
            item.LastModified = DateTime.Now;
            _uow.RiskAssessmentScales.Update(item);
            return Ok(new { code = "001", msg = "success", data = obj });
        }

        [HttpGet("{id}")]
        public IActionResult GetItem(int id)
        {
            var userInfo = HttpContext.Items["UserInfo"] as User;
            var item = _uow.RiskAssessmentScales.FirstOrDefault(o => o.ID == id);
            if (item == null || !_uow.RiskAssessmentScales.CheckPermission(item, userInfo.DomainId))
                return NotFound();
            var result = _mapper.Map<RiskAssessmentScaleDto>(item);
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

        public string GetCondition(RiskAssessmentScale obj, int method)
        {
            if (obj == null)
                return "";
            if (method == 1)
                return obj.Condition;
            var min = "";
            var max = "";
            if (obj.MinValue.HasValue)
                min = obj.MinValue + " " + (obj.LowerConditionName == ">" ? "<" : "<=");
            if (obj.MaxValue.HasValue)
                max = obj.UpperConditionName + " " + obj.MaxValue;
            if (min != "" || max != "")
                return min + " Giá trị " + max;
            return "";
        }
        public string GetConditionCustom(RiskAssessmentScaleDto obj, int method)
        {
            if (obj == null)
                return "";
            if (method == 1)
                return obj.Condition;
            var min = "";
            var max = "";
            if (obj.MinValue.HasValue)
                min = obj.MinValue + " " + (obj.LowerConditionName == ">" ? "<" : "<=");
            if (obj.MaxValue.HasValue)
                max = obj.UpperConditionName + " " + obj.MaxValue;
            if (min != "" || max != "")
                return min + " Giá trị " + max;
            return "";
        }
    }
}
