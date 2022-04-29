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
    [Route("RatingScale")]
    public class RatingScalesController : BaseController
    {
        public RatingScalesController(ILoggerManager logger
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

                        var _data = Utils.ExcelFn.ExcelToList<RatingScaleDto>(fullPath);

                        bool hasError = false;
                        ////var trans = _uow.BeginTransaction();
                        var userInfo = HttpContext.Items["UserInfo"] as User;
                        ////var allCode = data.Select(o => o.Code).ToList();

                        var exCode = _uow.RatingScales.Find(o => o.Status && o.DomainId == userInfo.DomainId).Select(o => o.Code).ToList();


                        _data.ForEach(o =>
                        {
                            o.Valid = true;
                            if (string.IsNullOrEmpty(o.Code))
                            {
                                o.Valid = false;
                                o.Note += "Code cannot be null!<br />";
                            }
                            if (string.IsNullOrEmpty(o.Name))
                            {
                                o.Valid = false;
                                o.Note += "Code cannot be null!<br />";
                            }
                            if (o.Min < 0)
                            {
                                o.Valid = false;
                                o.Note += "Rating point must be greater than or equal with 0!<br />";
                            }
                            if (o.Max < 0)
                            {
                                o.Valid = false;
                                o.Note += "Rating point must be greater than or equal with 0!<br />";
                            }
                            if (o.Max < o.Min)
                            {
                                o.Valid = false;
                                o.Note += "Max value must be greater then Min value!<br />";
                            }
                            if (exCode.Contains(o.Code))
                            {
                                o.Valid = false;
                                o.Note += "Code has exists!<br />";
                            }
                        });

                        hasError = _data.Any(o => !o.Valid);

                        if (!hasError)
                        {
                            _data.ForEach(o =>
                            {
                                var obj = _mapper.Map<RatingScale>(o);
                                obj.UserCreate = userInfo.Id;
                                obj.CreateDate = DateTime.Now;
                                obj.DomainId = userInfo.DomainId;
                                obj.Status = true;
                                _uow.RatingScales.Add(obj);
                                o.Note = "200";
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
            var obj = JsonSerializer.Deserialize<RiskCycleSearch>(jsonData);
            if (HttpContext.Items["UserInfo"] is not User userInfo)
            {
                return Unauthorized();
            }

            obj.DomainId = userInfo.DomainId;
            var lst = _uow.RatingScales.Search(obj, out int count).Select(o => _mapper.Map<RatingScaleDto>(o)).ToList();

            return Ok(new { code = "001", msg = "success", data = lst, total = count });
        }

        [HttpPost]
        public IActionResult UpdateItem(RatingScaleDto obj)
        {
            var current_id = obj.ID;
            if (HttpContext.Items["UserInfo"] is not User userInfo)
            {
                return Unauthorized();
            }

            if (!obj.IsValid())
                return BadRequest(new { msg = "Invalid data!" });

            CatRiskLevel cat = _uow.SystemCategories.GetRiskLevelCategory().Where(o => o.Id == obj.RiskLevel).FirstOrDefault();
            if (cat == null)
                return BadRequest(new { code = "404", msg = "Invalid Risk level!" });
            obj.RiskLevel = cat.Id;
            obj.RiskLevelName = cat.Name;
            obj.DomainId = userInfo.DomainId;
            var hasConfig = _uow.RatingScales.HasConfig(obj);
            if (hasConfig)
                return Ok(new { code = "106", msg = "success", data = obj });

            if (obj.ID > 0)
            {
                var ex = _uow.RatingScales.FirstOrDefault(o => o.ID == obj.ID);
                if (!_uow.RatingScales.CheckPermission(ex, userInfo.DomainId))
                    return Forbid();
                ex = obj.Map(ex);
                ex.Min = obj.Min;
                ex.Max = obj.Max;
                ex.MinFunction = obj.MinFunction;
                ex.MaxFunction = obj.MaxFunction;
                ex.RiskLevel = obj.RiskLevel;
                ex.RiskLevelName = obj.RiskLevelName;
                ex.ApplyFor = obj.ApplyFor;
                ex.ModifiedBy = userInfo.Id;
                ex.LastModified = DateTime.Now;
                ex.DomainId = userInfo.DomainId;
                _uow.RatingScales.Update(ex);
                obj = _mapper.Map<RatingScaleDto>(ex);
            }
            else
            {
                var item = _mapper.Map<RatingScale>(obj);
                item.UserCreate = userInfo.Id;
                item.CreateDate = DateTime.Now;
                item.DomainId = userInfo.DomainId;
                _uow.RatingScales.Add(item);
                obj = _mapper.Map<RatingScaleDto>(item);
            }

            return Ok(new { code = "001", msg = "success", data = obj, id = current_id });
        }

        [HttpPost("Delete")]
        public IActionResult DeleteItem(RatingScaleDto obj)
        {
            if (HttpContext.Items["UserInfo"] is not User userInfo)
            {
                return Unauthorized();
            }
            var item = _uow.RatingScales.FirstOrDefault(o => o.ID == obj.ID);
            if (item == null)
                return NotFound();
            ////if (item.UserCreate != userInfo.Id)
            ////{
            ////    _logger.LogError($"{userInfo.UserName} delete 'rating scale' {item.ID}: don't have permission!");
            ////    return BadRequest();
            ////}

            var applied = _uow.ScoreBoard.Find(o => o.DomainId == userInfo.DomainId && !o.Deleted && o.RatingScaleId == item.ID).Any();
            if (applied)
                return Ok(new { code = "601", msg = "success", data = obj });

            item.Deleted = true;
            item.ModifiedBy = userInfo.Id;
            item.LastModified = DateTime.Now;
            _uow.RatingScales.Update(item);
            return Ok(new { code = "001", msg = "success", data = obj });
        }

        [HttpGet("{id}")]
        public IActionResult GetItem(int id)
        {
            var userInfo = HttpContext.Items["UserInfo"] as User;
            var item = _uow.RatingScales.FirstOrDefault(o => o.ID == id);
            if (item == null || !_uow.RatingScales.CheckPermission(item, userInfo.DomainId))
                return NotFound();
            var result = _mapper.Map<RatingScaleDto>(item);
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
