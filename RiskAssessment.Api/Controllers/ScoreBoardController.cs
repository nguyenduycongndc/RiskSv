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
using RiskAssessment.Entity.ViewModel;
using FlexCel.Core;
using FlexCel.Render;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using Microsoft.AspNetCore.Http;

namespace RiskAssessment.Api.Controllers
{
    [ApiController]
    [Route("ScoreBoard")]
    public class ScoreBoardController : BaseController
    {
        public ScoreBoardController(ILoggerManager logger
            , IUnitOfWork uow
            , IMapper mapper
            , IConfiguration config
            , IDatabase iDb) : base(logger, uow, mapper, iDb, config)
        {
        }

        private void MapParentDto(ScoreBoardIssueDto o, List<ScoreBoardIssueDto> lst)
        {
            var childs = lst.Where(a => a.IssueParentId == o.IssueId).ToList();

            childs.ForEach(a => MapParentDto(a, lst));

            o.SubIssues = childs;
            o.HasChild = o.SubIssues != null && o.SubIssues.Count > 0;
        }

        private List<ScoreBoardIssueDto> RemapDto(List<ScoreBoardIssueDto> lst)
        {
            var ids = lst.Select(o => o.IssueId).ToList();
            ////var parentIds = lst.Where(o => o.IssueParentId.HasValue && o.IssueParentId.Value > 0).Select(o => o.IssueParentId.Value).Distinct().ToList();

            var parent = lst.Where(o => !o.IssueParentId.HasValue || o.IssueParentId == 0 || !ids.Any(a => a == o.IssueParentId.Value)).ToList();

            parent.ForEach(a =>
            {
                MapParentDto(a, lst);
                a.HasChild = a.SubIssues != null && a.SubIssues.Count > 0;
            });
            return parent;
        }



        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            var obj = JsonSerializer.Deserialize<ScoreBoardSearch>(jsonData);
            if (HttpContext.Items["UserInfo"] is not User userInfo)
            {
                return Unauthorized();
            }

            if (obj.Year == null || obj.Year < 1900
                || obj.Stage == null || obj.Stage <= 0 || obj.Stage > 3 || (obj.Stage == 3 && (obj.StageValue < 1 || obj.StageValue > 4))
                || string.IsNullOrEmpty(obj.ApplyFor) || obj.ObjectId == null)
                return BadRequest();

            var cateApplyFor = _uow.SystemCategories.FirstOrDefault(o => o.ParentGroup == "DoiTuongApDung" && o.Code == obj.ApplyFor);
            if (cateApplyFor == null)
                return BadRequest();

            obj.DomainId = userInfo.DomainId;

            var assessmentStage = _uow.AssessmentStages.FirstOrDefault(o => o.Status && !o.Deleted && o.DomainId == obj.DomainId
            && o.Year == obj.Year && ((o.Stage == obj.Stage && obj.Stage != 3)
            || (obj.Stage == 3 && o.StageValue == obj.StageValue)));

            if (assessmentStage == null)
                return Ok(new { code = "004" });

            int objectId = 0;
            ////string objectName = "";
            string processName = "";
            if (obj.ApplyFor == "QT")
            {
                var process = _uow.AuditProcess.FirstOrDefault(o => o.ID == obj.ObjectId && !o.Deleted && o.Status && o.DomainId == obj.DomainId);

                if (process == null)
                    return Ok(new { code = "005" });

                objectId = process.ID;
                ////objectName = process.Name;
            }
            else if (obj.ApplyFor == "DV")
            {
                var facility = _uow.AuditFacilities.FirstOrDefault(o => o.ID == obj.ObjectId && !o.Deleted && o.Status && o.DomainId == obj.DomainId);

                if (facility == null)
                    return Ok(new { code = "005" });

                objectId = facility.ID;
                ////objectName = facility.Name;

                var allProcess = _uow.AuditProcess.Find(o => !o.Deleted && o.Status && o.DomainId == userInfo.DomainId && o.FacilityId == facility.ID).Select(o => o.Name).Distinct().ToArray();
                processName = string.Join("; ", allProcess);
            }
            else if (obj.ApplyFor == "HDKD")
            {
                var activity = _uow.BsActRepo.FirstOrDefault(o => o.ID == obj.ObjectId && !o.Deleted && o.Status && o.DomainId == obj.DomainId);

                if (activity == null)
                    return Ok(new { code = "005" });

                objectId = activity.ID;
                ////objectName = activity.Name;
            }


            var self = _uow.ScoreBoard.FirstOrDefault(o => o.Status && o.DomainId == userInfo.DomainId
            && o.AssessmentStageId == assessmentStage.ID && o.ObjectId == objectId);

            if (self == null)
            {
                if (assessmentStage.StageState != 1)
                    return Ok(new { code = "006" });
                else
                    return Ok(new { code = "103" });
            }

            var file = _uow.ScoreBoard.GetFile(self.ID);
            self.ScoreBoardFile = file;
            if (self.CurrentStatus != 1)
                self.MainProcess = processName;

            var selfData = _uow.ScoreBoard.GetIssues(self.ID).Where(o => o.Status && o.ApplyFor == obj.ApplyFor).ToList();

            if (selfData == null || selfData.Count <= 0)
            {
                var riskIssues = _uow.RiskIssues.Find(o => o.DomainId == userInfo.DomainId && !o.Deleted && o.Status && o.ApplyFor == cateApplyFor.ID).ToList();

                if (riskIssues == null || riskIssues.Count <= 0)
                    return Ok(new { code = "007" });

                selfData = InitBoardIssues(riskIssues, riskIssues, self.ID, userInfo, obj.ApplyFor);

                if (selfData.Any())
                    _uow.ScoreBoard.AddIssues(selfData);
            }

            var selfDataDto = selfData.OrderByDescending(a=>a.CreateDateIssue).Select(o => _mapper.Map<ScoreBoardIssueDto>(o)).ToList();

            selfDataDto = RemapDto(selfDataDto);

            var selfDto = _mapper.Map<ScoreBoardDto>(self);
            var stageDto = _mapper.Map<AssessmentStageDto>(assessmentStage);

            return Ok(new { code = "001", self = selfDto, selfDataDto, stage = stageDto });

        }
        [HttpPost("InitBoard")]
        public IActionResult InitBoard(ScoreBoardSearch obj)
        {
            if (HttpContext.Items["UserInfo"] is not User userInfo)
            {
                return Unauthorized();
            }

            if (obj.Year == null || obj.Year < 1900
                || obj.Stage == null || obj.Stage <= 0 || obj.Stage > 3 || (obj.Stage == 3 && (obj.StageValue < 1 || obj.StageValue > 4))
                || string.IsNullOrEmpty(obj.ApplyFor) || obj.ObjectId == null)
                return BadRequest();
            var cateApplyFor = _uow.SystemCategories.FirstOrDefault(o => !o.Deleted && o.Status && o.DomainId == userInfo.DomainId && o.ParentGroup == "DoiTuongApDung" && o.Code == obj.ApplyFor);
            if (cateApplyFor == null)
                return BadRequest();

            obj.DomainId = userInfo.DomainId;
            var assessmentStage = _uow.AssessmentStages.FirstOrDefault(o => o.Status && !o.Deleted && o.DomainId == obj.DomainId
            && o.Year == obj.Year && ((o.Stage == obj.Stage && obj.Stage != 3)
            || (obj.Stage == 3 && o.StageValue == obj.StageValue)));

            if (assessmentStage == null)
                return Ok(new { code = "004" });

            int objectId = 0;
            string objectName = "";
            string objectCode = "";
            string classObjectId = "";

            if (obj.ApplyFor == "QT")
            {
                var process = _uow.AuditProcess.FirstOrDefault(o => o.ID == obj.ObjectId && !o.Deleted && o.Status && o.DomainId == obj.DomainId);

                if (process == null)
                    return Ok(new { code = "005" });

                objectId = process.ID;
                objectName = process.Name;
                objectCode = process.Code;
            }
            else if (obj.ApplyFor == "DV")
            {
                var facility = _uow.AuditFacilities.FirstOrDefault(o => o.ID == obj.ObjectId && !o.Deleted && o.Status && o.DomainId == obj.DomainId);

                if (facility == null)
                    return Ok(new { code = "005" });

                objectId = facility.ID;
                objectName = facility.Name;
                objectCode = facility.Code;
                classObjectId = (facility.ObjectClassId ?? 0).ToString("0");
            }
            else if (obj.ApplyFor == "HDKD")
            {
                var activity = _uow.BsActRepo.FirstOrDefault(o => o.ID == obj.ObjectId && !o.Deleted && o.Status && o.DomainId == obj.DomainId);

                if (activity == null)
                    return Ok(new { code = "005" });

                objectId = activity.ID;
                objectName = activity.Name;
                objectCode = activity.Code;
            }

            var selfData = _uow.ScoreBoard.FirstOrDefault(o => o.Status && !o.Deleted && o.DomainId == userInfo.DomainId
            && o.AssessmentStageId == assessmentStage.ID && o.ObjectId == objectId);

            if (selfData != null)
                return Ok(new { code = "002" });

            var iQRiskIssues = _uow.RiskIssues.Find(o => o.DomainId == userInfo.DomainId && o.Status && !o.Deleted && o.ApplyFor == cateApplyFor.ID);
            if (obj.ApplyFor == "DV")
            {
                iQRiskIssues = iQRiskIssues.Where(o => o.ClassType.Contains(classObjectId));
            }

            var riskIssues = iQRiskIssues.ToList();

            riskIssues = base.GetParents(riskIssues);

            if (riskIssues == null || riskIssues.Count <= 0)
                return Ok(new { code = "007" });
            //add score board
            var board = new ScoreBoard()
            {
                UserCreate = userInfo.Id,
                CreateDate = DateTime.Now,
                DomainId = userInfo.DomainId,
                Status = true,
                AssessmentStageId = assessmentStage.ID,
                Year = assessmentStage.Year,
                Stage = assessmentStage.Stage,
                StageValue = assessmentStage.StageValue,
                ObjectId = objectId,
                ObjectName = objectName,
                ObjectCode = objectCode,
                CurrentStatus = 0,
                ApplyFor = obj.ApplyFor
            };

            board = _uow.ScoreBoard.Add(board);

            //add risk issue
            var boardIssues = InitBoardIssues(riskIssues, riskIssues, board.ID, userInfo, obj.ApplyFor);

            if (boardIssues.Any())
                _uow.ScoreBoard.AddIssues(boardIssues);

            //add result
            var result = new AssessmentResult()
            {
                ScoreBoardId = board.ID,
            };

            var lastBoard = _uow.ScoreBoard.GetAll().Where(o => o.DomainId == userInfo.DomainId && o.ApplyFor == obj.ApplyFor
            && o.ObjectId == board.ObjectId && o.CurrentStatus == 1 && !o.Deleted).OrderByDescending(o => o.CreateDate).FirstOrDefault();
            if (lastBoard != null)
            {
                var lastAudit = _uow.AssessmentResults.FirstOrDefault(o => o.ScoreBoardId == lastBoard.ID && !o.Deleted);
                if (lastAudit != null)
                {
                    result.LastAudit = lastAudit.AuditDate;
                    result.LastRiskLevel = string.IsNullOrEmpty(lastAudit.RiskLevelChangeName) ? lastAudit.LastRiskLevel : lastAudit.RiskLevelChangeName;
                }
            }


            _uow.AssessmentResults.Add(result);

            return Ok(new { code = "001" });
        }
        [HttpPost, DisableRequestSizeLimit]
        public IActionResult Update()
        {
            try
            {
                IFormFileCollection file = Request.Form.Files;
                var objJson = Request.Form["objData"];

                if (string.IsNullOrEmpty(objJson))
                    return Ok(new { code = "004" });
                var userInfo = HttpContext.Items["UserInfo"] as User;
                var obj = JsonSerializer.Deserialize<ScoreBoardDto>(objJson);
                if (obj == null)
                    return Ok(new { code = "004" });

                obj.DomainId = userInfo.DomainId;

                var assessmentStage = _uow.AssessmentStages.FirstOrDefault(o => o.Status && !o.Deleted && o.DomainId == obj.DomainId
                && o.Year == obj.Year && ((o.Stage == obj.Stage && obj.Stage != 3)
                || (obj.Stage == 3 && o.StageValue == obj.StageValue)));

                if (assessmentStage == null)
                    return Ok(new { code = "004" });

                int objectId = 0;
                ////string objectName = "";

                if (obj.ApplyFor == "QT")
                {
                    var process = _uow.AuditProcess.FirstOrDefault(o => o.ID == obj.ObjectId && !o.Deleted && o.Status && o.DomainId == obj.DomainId);

                    if (process == null)
                        return Ok(new { code = "005" });

                    objectId = process.ID;
                    ////objectName = process.Name;
                }
                else if (obj.ApplyFor == "DV")
                {
                    var facility = _uow.AuditFacilities.FirstOrDefault(o => o.ID == obj.ObjectId && !o.Deleted && o.Status && o.DomainId == obj.DomainId);

                    if (facility == null)
                        return Ok(new { code = "005" });

                    objectId = facility.ID;
                    ////objectName = facility.Name;
                }
                else if (obj.ApplyFor == "HDKD")
                {
                    var activity = _uow.BsActRepo.FirstOrDefault(o => o.ID == obj.ObjectId && !o.Deleted && o.Status && o.DomainId == obj.DomainId);

                    if (activity == null)
                        return Ok(new { code = "005" });

                    objectId = activity.ID;
                    ////objectName = activity.Name;
                }


                var self = _uow.ScoreBoard.FirstOrDefault(o => o.Status && o.DomainId == userInfo.DomainId
                && o.AssessmentStageId == assessmentStage.ID && o.ObjectId == objectId && !o.Deleted);

                if (self == null || !_uow.ScoreBoard.CheckPermission(self, userInfo.DomainId))
                    return Ok(new { code = "006" });

                self.ObjectName = obj.ObjectName;
                ////self.StateInfo = obj.StateInfo;
                ////self.Point = obj.Point;
                ////self.RiskLevel = obj.RiskLevel;
                self.Target = obj.Target;
                self.MainProcess = obj.MainProcess;
                self.ITSystem = obj.ITSystem;
                self.Project = obj.Project;
                self.Outsourcing = obj.Outsourcing;
                self.Customer = obj.Customer;
                self.Supplier = obj.Supplier;
                self.InternalRegulations = obj.InternalRegulations;
                self.LawRegulations = obj.LawRegulations;

                self.ModifiedBy = userInfo.Id;
                self.LastModified = DateTime.Now;
                self.DomainId = userInfo.DomainId;
                self.Description = obj.Description;
                #region for attach
                List<ScoreBoardFile> listattach = new List<ScoreBoardFile>();
                if (file != null)
                {
                    foreach (var _file in file)
                    {
                        ScoreBoardFile scoreBoardFile = new ScoreBoardFile();
                        var attachPath = "";
                        var formats = _config["Upload:FileTypeAllow"].Split(';');
                        if (!formats.Any(item => _file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase)))
                        {
                            return Ok(new { code = "509" });
                        }

                        var sizeConfig = string.IsNullOrEmpty(_config["Upload:MaxLength"]) ? "10" : _config["Upload:MaxLength"];

                        if (!int.TryParse(sizeConfig, out int allowSize))
                        {
                            allowSize = 10;
                        }
                        if (_file.Length > 0 && _file.Length < allowSize * 1024 * 1024)
                        {
                            var pathToSave = Path.Combine(_config["Upload:AttachsPath"], "Attachs");
                            if (!Directory.Exists(pathToSave))
                                Directory.CreateDirectory(pathToSave);
                            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(_file.FileName)?.Trim();
                            var extension = Path.GetExtension(_file.FileName);
                            //var realPath = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(_file.FileName);
                            var realPath = fileNameWithoutExtension + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                            var fullPath = Path.Combine(pathToSave, realPath);
                            attachPath = Path.Combine("Attachs", realPath);
                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                _file.CopyTo(stream);
                            }
                            scoreBoardFile.Path = attachPath;
                            scoreBoardFile.CreateAt = DateTime.Now;
                            scoreBoardFile.CreatedBy = userInfo.Id;
                            self.ScoreBoardFile.Add(scoreBoardFile);

                        }
                    }
                }
                #endregion

                _uow.ScoreBoard.Update(self);


                return Ok(new { code = "001" });

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
        [HttpPost("RefreshIssue")]
        public IActionResult Refresh(ScoreBoardSearch input)
        {
            if (HttpContext.Items["UserInfo"] is not User userInfo)
            {
                return Unauthorized();
            }

            if (input.Year == null || input.Year < 1900
                || input.Stage == null || input.Stage <= 0 || input.Stage > 3 || (input.Stage == 3 && (input.StageValue < 1 || input.StageValue > 4))
                || string.IsNullOrEmpty(input.ApplyFor) || input.ObjectId == null)
                return BadRequest();
            var cateApplyFor = _uow.SystemCategories.FirstOrDefault(o => !o.Deleted && o.Status && o.DomainId == userInfo.DomainId && o.ParentGroup == "DoiTuongApDung" && o.Code == input.ApplyFor);
            if (cateApplyFor == null)
                return BadRequest();

            input.DomainId = userInfo.DomainId;
            var assessmentStage = _uow.AssessmentStages.FirstOrDefault(o => o.Status && !o.Deleted && o.DomainId == input.DomainId
            && o.Year == input.Year && ((o.Stage == input.Stage && input.Stage != 3)
            || (input.Stage == 3 && o.StageValue == input.StageValue)));

            if (assessmentStage == null || assessmentStage.StageState == 1)
                return Ok(new { code = "004" });

            int objectId = 0;
            ////string objectName = "";
            ////string objectCode = "";
            string classObjectId = "";

            if (input.ApplyFor == "QT")
            {
                var process = _uow.AuditProcess.FirstOrDefault(o => o.ID == input.ObjectId && !o.Deleted && o.Status && o.DomainId == input.DomainId);

                if (process == null)
                    return Ok(new { code = "005" });

                objectId = process.ID;
                ////objectName = process.Name;
                ////objectCode = process.Code;
            }
            else if (input.ApplyFor == "DV")
            {
                var facility = _uow.AuditFacilities.FirstOrDefault(o => o.ID == input.ObjectId && !o.Deleted && o.Status && o.DomainId == input.DomainId);

                if (facility == null)
                    return Ok(new { code = "005" });

                objectId = facility.ID;
                ////objectName = facility.Name;
                ////objectCode = facility.Code;
                classObjectId = (facility.ObjectClassId ?? 0).ToString("0");
            }
            else if (input.ApplyFor == "HDKD")
            {
                var activity = _uow.BsActRepo.FirstOrDefault(o => o.ID == input.ObjectId && !o.Deleted && o.Status && o.DomainId == input.DomainId);

                if (activity == null)
                    return Ok(new { code = "005" });

                objectId = activity.ID;
                ////objectName = activity.Name;
                ////objectCode = activity.Code;
            }

            var self = _uow.ScoreBoard.FirstOrDefault(o => o.Status && !o.Deleted && o.DomainId == userInfo.DomainId
            && o.AssessmentStageId == assessmentStage.ID && o.ObjectId == objectId);

            if (self == null)
                return Ok();

            base.RefreshScoreBoard(self, userInfo, cateApplyFor, classObjectId);

            return Ok();
        }
        protected string CreateDownloadCode(string path, string name)
        {
            var fullPath = Path.Combine(_config["Upload:AttachsPath"], path);

            if (!System.IO.File.Exists(fullPath))
                return "";

            var code = Guid.NewGuid().ToString().Replace("-", "");

            _redis.StringSet(code, fullPath);
            _redis.StringSet(code + "_name", name);
            return code;
        }
        protected string GetPathDownload(string code, out string fileName)
        {
            fileName = _redis.StringGet(code + "_name");
            var fullPath = _redis.StringGet(code);

            if (string.IsNullOrEmpty(fileName))
                return "";

            if (!System.IO.File.Exists(fullPath))
                return "";

            return fullPath;
        }

        [HttpGet("DownloadAttach")]
        public IActionResult DonwloadFile(int id = 0, string filename = "")
        {
            var userInfo = HttpContext.Items["UserInfo"] as User;
            //var self = _uow.ScoreBoard.FirstOrDefault(o => o.ID == id);
            //if (self == null || !_uow.ScoreBoard.CheckPermission(self, userInfo.DomainId))
            //    return NotFound(new { code = "401" });
            //var
            var file = _uow.ScoreBoard.GetFileById(id);
            var code = CreateDownloadCode(file.Path, filename);
            if (string.IsNullOrEmpty(code))
                return NotFound();

            return Ok(new { code });
        }
        [AllowAnonymous]
        [HttpGet("Download")]
        public IActionResult DownloadTemp(string code = "")
        {
            var fullPath = GetPathDownload(code, out string fileName);

            if (string.IsNullOrEmpty(fullPath) || string.IsNullOrEmpty(fileName))
                return NotFound();

            var fs = new FileStream(fullPath, FileMode.Open);

            return File(fs, "application/octet-stream", fileName);
        }
        [HttpPost("RemoveAttach")]
        public IActionResult RemoveAttach(ScoreBoardDto obj)
        {
            var userInfo = HttpContext.Items["UserInfo"] as User;
            //var self = _uow.ScoreBoard.FirstOrDefault(o => o.ID == obj.ID);
            //if (self == null || !_uow.ScoreBoard.CheckPermission(self, userInfo.DomainId))
            //    return NotFound(new { code = "401" });

            //if (self.CurrentStatus == 1)
            //    return Ok(new { code = "009" });
            var file = _uow.ScoreBoard.GetFileById(obj.ID);
            if (file == null)
            {
                return NotFound();
            }
            var check = false;
            if (!string.IsNullOrEmpty(file.Path))
            {
                var fullPath = Path.Combine(_config["Upload:AttachsPath"], file.Path);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                    check = true;
                }
                else
                {
                    return NotFound();
                }
            }
            if (check)
            {
                file.IsDelete = true;
                file.DeleteAt = DateTime.Now;
                file.DeleteBy = userInfo.Id;
                _uow.ScoreBoard.UpdateFileStatus();
            }
            return Ok(new { code = "001" });
        }

        [HttpGet("RiskIssue")]
        public ActionResult GetPoint(int issue, int score_board)
        {
            var userInfo = HttpContext.Items["UserInfo"] as User;
            var self = _uow.ScoreBoard.FirstOrDefault(o => o.ID == score_board);
            if (self == null || !_uow.ScoreBoard.CheckPermission(self, userInfo.DomainId))
                return NotFound(new { code = "401" });

            if (self.CurrentStatus == 1)
                return Ok(new { code = "009" });

            var issueData = _uow.ScoreBoard.GetIssues(score_board).Where(o => o.IssueId == issue).FirstOrDefault();
            if (issueData == null)
                return NotFound(new { code = "402" });

            var item = _mapper.Map<ScoreBoardIssueDto>(issueData);

            return Ok(new { code = "001", item });
        }


        [HttpPost("RiskIssue")]
        public ActionResult UpdatePoint(ScoreBoardIssueDto obj)
        {
            var _applyfor = obj.ApplyFor;
            var userInfo = HttpContext.Items["UserInfo"] as User;
            var self = _uow.ScoreBoard.FirstOrDefault(o => o.ID == obj.ScoreBoardId);
            if (self == null || !_uow.ScoreBoard.CheckPermission(self, userInfo.DomainId))
                return NotFound(new { code = "401" });

            if (self.CurrentStatus == 1)
                return Ok(new { code = "103" });

            var allIssues = _uow.ScoreBoard.GetIssues(obj.ScoreBoardId).Where(o => o.Status).ToList();
            var item = allIssues.FirstOrDefault(o => o.IssueId == obj.IssueId);
            if (item == null)
                return NotFound(new { code = "402" });

            if (obj.ApplyFor == "0")
            {
                if (item.MethodId == 0)
                {
                    var scales = _uow.RiskAssessmentScales.Find(o => o.DomainId == userInfo.DomainId
                    && o.RiskIssueId == item.IssueId && o.Status && !o.Deleted).ToList();
                    if (scales.Count() == 0)
                        return Ok(new { code = "104" });
                    var x = scales.FirstOrDefault(o => (o.MinValue.HasValue ? o.MinValue <= obj.RiskValue : true) && (o.MaxValue ?? obj.RiskValue) >= obj.RiskValue);
                    if (x == null)
                        return Ok(new { code = "104" });
                    if (x.MinValue.HasValue && !string.IsNullOrEmpty(x.LowerConditionName) && x.LowerConditionName == ">")
                    {
                        if(obj.RiskValue == x.MinValue)
                            return Ok(new { code = "104" });
                    }
                    if (x.MaxValue.HasValue && !string.IsNullOrEmpty(x.UpperConditionName) && x.UpperConditionName == "<")
                    {
                        if (obj.RiskValue == x.MaxValue)
                            return Ok(new { code = "104" });
                    }
                    obj.Point = x.Point;
                }

                item.Point = obj.Point;
                item.Condition = obj.Condition;
                item.RiskValue = obj.RiskValue;
                item.Description = obj.Description;
                item.ModifiedBy = userInfo.Id;
                item.LastModified = DateTime.Now;
                _uow.ScoreBoard.UpdateIssues(item);
            }
            else
            {
                item.Proportion = obj.Proportion;
                item.ProportionModify = obj.ProportionModify;
                item.Reason = obj.Reason;
                item.ModifiedBy = userInfo.Id;
                item.IsApply = obj.IsApplyPost == 0;
                item.LastModified = DateTime.Now;
                _uow.ScoreBoard.UpdateIssues(item);
            }

            #region calculator point for parent

            allIssues = CalculatorForParent(allIssues, item, userInfo);
            allIssues = allIssues.Where(a => a.IsApply != false).ToList();
            CalculatorRoot(allIssues, self, userInfo);

            var _code = CalculatorForScoreboard(self, userInfo);

            #endregion

            return Ok(new { code = _code, applyfor = _applyfor });
        }


        #region result
        [HttpGet("ResultSearch")]
        public IActionResult ResultSearch(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<AssessmentResultSearch>(jsonData);
                if (HttpContext.Items["UserInfo"] is not User userInfo)
                {
                    return Unauthorized();
                }

                obj.DomainId = userInfo.DomainId;

                if (obj.Year == null || obj.Stage == null)
                    return BadRequest();

                var lst = _uow.AssessmentResults.Search(obj);

                //sort
                if (obj.SortBy == "AssessmentPoint")
                {
                    if (obj.SortType == "asc")
                        lst = lst.OrderBy(o => o.Point);
                    else
                        lst = lst.OrderByDescending(o => o.Point);
                }
                var count = lst.Count();
                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    lst = lst.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var result = lst.ToList();
                result.ForEach(item =>
                {
                    var lastBoard = _uow.ScoreBoard.GetRecentScoreBoard(item.ID, item.ObjectId, obj.DomainId, item.ApplyForId, obj.Year.Value, 1);
                    item.LastRiskLevel = lastBoard?.FirstOrDefault()?.assessment_risklevel ?? "";
                    var lastAudit = _uow.ScoreBoard.GetRecentAudit(item.ApplyForId, item.ObjectId, obj.Year.Value, 1);
                    item.LastAudit = lastAudit?.FirstOrDefault()?.Time ?? "";
                });
                return Ok(new { code = "001", data = result, total = count });
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpGet("AuditHistory")]
        public IActionResult AuditHistory(int boardId)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not User userInfo)
                {
                    return Unauthorized();
                }
                var board = _uow.ScoreBoard.FirstOrDefault(o => o.ID == boardId && o.DomainId == userInfo.DomainId);
                if (board == null)
                    return NotFound();

                ////var assessments = _uow.ScoreBoard.GetAll().Where(o => o.Status && o.ObjectId == board.ObjectId && o.ApplyFor == board.ApplyFor
                ////&& o.DomainId == userInfo.DomainId && o.CurrentStatus == 1).OrderByDescending(o => o.CreateDate).Take(3).Select(o => _mapper.Map<ScoreBoardDto>(o)).ToList();
                var assessments = _uow.ScoreBoard.GetRecentScoreBoard(board.ID, board.ObjectId, userInfo.DomainId, board.ApplyFor, board.Year, 3).Select(o => _mapper.Map<RecentScoreBoard>(o)).ToList();
                var recentaudit = _uow.ScoreBoard.GetRecentAudit(board.ApplyFor, board.ObjectId, board.Year, 3);
                return Ok(new { code = "001", assessments, audit = recentaudit });
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpGet("AuditResult")]
        public IActionResult AuditResult(int boardId)
        {
            if (HttpContext.Items["UserInfo"] is not User userInfo)
            {
                return Unauthorized();
            }

            var board = _uow.ScoreBoard.FirstOrDefault(o => o.ID == boardId && o.DomainId == userInfo.DomainId);
            if (board == null)
                return NotFound();
            var itemSearch = new AssessmentResultSearch() { DomainId = userInfo.DomainId, BoardId = boardId };

            var result = _uow.AssessmentResults.GetResult(itemSearch);
            if (result == null)
                return NotFound();

            return Ok(new { code = "001", assessment = result });

        }
        [HttpPost("AuditResult")]
        public IActionResult UpdateAuditResult(AssessmentResultView obj)
        {
            if (HttpContext.Items["UserInfo"] is not User userInfo)
            {
                return Unauthorized();
            }

            var board = _uow.ScoreBoard.FirstOrDefault(o => o.ID == obj.ID && o.DomainId == userInfo.DomainId);
            if (board == null)
                return NotFound();
            if (board.CurrentStatus == 1)
                return Ok(new { code = "103" });

            var result = _uow.AssessmentResults.FirstOrDefault(o => o.ScoreBoardId == board.ID);
            if (result == null)
                return NotFound();

            CatRiskLevel riskLevel = null;

            var oldScale = _uow.RatingScales.FirstOrDefault(o => o.ID == board.RatingScaleId && o.ApplyFor == board.ApplyFor); //huybt bổ sung loại áp dụng cho thang điểm xếp hạng
            if (oldScale == null)
                return Ok(new { code = "107" });
            riskLevel = _uow.SystemCategories.GetRiskLevelCategory().FirstOrDefault(o => o.Id == oldScale.RiskLevel);

            if (obj.RiskLevelChange > 0)
            {

                riskLevel = _uow.SystemCategories.GetRiskLevelCategory().FirstOrDefault(o => o.Id == obj.RiskLevelChange);

                if (riskLevel == null)
                    return Ok(new { code = "107" });

                result.RiskLevelChange = obj.RiskLevelChange;
                result.RiskLevelChangeName = riskLevel.Name;
            }
            else
            {
                result.RiskLevel = riskLevel.Id;
                result.RiskLevelChange = 0;
                result.RiskLevelChangeName = "";
            }
            var auditCycle = _uow.AuditCycle.FirstOrDefault(o => o.DomainId == userInfo.DomainId && !o.Deleted && o.RatingPoint == riskLevel.Id && o.Status);
            if (auditCycle != null)
                board.AuditCycleName = auditCycle.Name;
            board.ModifiedBy = userInfo.Id;
            board.LastModified = DateTime.Now;
            board.DomainId = userInfo.DomainId;
            _uow.ScoreBoard.Update(board);


            result.Description = obj.Description;
            result.Audit = obj.Audit;
            result.AuditReason = !result.Audit ? null : obj.AuditReasonId;
            result.PassAuditReason = obj.PassAuditReason;

            result.ModifiedBy = userInfo.Id;
            result.LastModified = DateTime.Now;
            result.DomainId = userInfo.DomainId;

            _uow.AssessmentResults.Update(result);
            return Ok(new { code = "001" });
        }
        [HttpGet("ExportReport")]
        public IActionResult ExportReport(int? year, int? stage, int? stageValue)
        {
            try
            {
                var result = new XlsFile(true);

                var tempPath = _config["Report:TemplateReport"];
                var tempExport = _config["Report:ExportPath"];
                var pathTemplate = Path.Combine(tempPath, "AssessmentResults.xlsx");
                result.Open(pathTemplate);
                var fr = new FlexCelReport();

                var userInfo = HttpContext.Items["UserInfo"] as User;
                var obj = new AssessmentResultSearch()
                {
                    Year = year,
                    Stage = stage,
                    StageValue = stageValue,
                    DomainId = userInfo.DomainId,
                    ApplyFor = "",
                    Key = ""
                };
                var lst = _uow.AssessmentResults.Search(obj).ToList();
                var stagename = stage == 1 ? $"năm {year}" : $"quý {stageValue} năm {year}";
                stagename = stage == 2 ? $"bán niên năm {year}" : stagename;
                var title = $"Kỳ báo cáo {stagename}";
                fr.SetValue("title", title);
                fr.AddTable("dt", lst);
                fr.Run(result);
                fr.Dispose();

                var fileName = $"Tổng hợp kết quả đánh giá rủi ro cấp độ tổ chức - {title}.xlsx";

                var realPath = Guid.NewGuid().ToString().Replace("-", "") + ".xlsx";
                var pathExport = Path.Combine(tempExport, realPath);

                result.Save(pathExport);

                var code = Guid.NewGuid().ToString().Replace("-", "");

                _redis.StringSet(code, pathExport);
                _redis.StringSet(code + "_name", fileName);

                return Ok(new { code });
            }
            catch
            {
                return null;
            }
        }
        [HttpPost("EndStage")]
        public IActionResult EndStage(AssessmentResultSearch obj)
        {
            if (HttpContext.Items["UserInfo"] is not User userInfo)
            {
                return Unauthorized();
            }

            obj.DomainId = userInfo.DomainId;

            if (obj.Year == null || obj.Stage == null)
                return BadRequest();

            var assessmentStage = _uow.AssessmentStages.FirstOrDefault(o => o.Status && o.DomainId == obj.DomainId
            && o.Year == obj.Year && ((o.Stage == obj.Stage && obj.Stage != 3)
            || (obj.Stage == 3 && o.StageValue == obj.StageValue)));

            if (assessmentStage == null)
                return Ok(new { code = "004" });

            var lst = _uow.AssessmentResults.GetIdBoardsForResult(obj).ToList();

            var allowEnd = true;
            var _msg = "";
            lst.ForEach(id =>
            {
                if (allowEnd)
                {
                    allowEnd = !_uow.ScoreBoard.GetIssues(id).Any(o => o.Point == null);
                }
            });

            if (allowEnd)
            {
                lst.ForEach(id =>
                {
                    var board = _uow.ScoreBoard.FirstOrDefault(o => o.ID == id && o.DomainId == obj.DomainId);
                    if (board != null)
                    {
                        board.CurrentStatus = 1;
                        board.ModifiedBy = userInfo.Id;
                        board.LastModified = DateTime.Now;
                        board = _uow.ScoreBoard.Update(board);
                        var boardResult = _uow.AssessmentResults.FirstOrDefault(o => o.ScoreBoardId == id && o.DomainId == obj.DomainId);
                        if (boardResult != null)
                        {
                            boardResult.StageStatus = 1;
                            boardResult.AuditDate = DateTime.Now;
                            boardResult.ModifiedBy = userInfo.Id;
                            boardResult.LastModified = DateTime.Now;

                            boardResult = _uow.AssessmentResults.Update(boardResult);
                        }

                        var nextBoards = _uow.ScoreBoard.GetAll().Where(o => o.DomainId == userInfo.DomainId
                        && o.ApplyFor == board.ApplyFor && o.ObjectId == board.ObjectId && !o.Deleted
                        && o.Year >= board.Year && o.Stage == board.Stage && o.ID != board.ID).ToList();
                        ScoreBoard nextBoard = null;

                        if (nextBoards.Count > 0)
                        {
                            if (board.Stage == 1 || board.Stage == 2)
                                nextBoard = nextBoards.FirstOrDefault(a => a.Year > board.Year);
                            else
                            {
                                nextBoard = nextBoards.Where(a => (a.Year == board.Year && a.Stage == 3 && a.StageValue > board.Stage)
                                || (a.Year > board.Year)).OrderBy(o => o.Year).ThenBy(o => o.StageValue).FirstOrDefault();
                            }

                            if (nextBoard != null)
                            {
                                var nextResult = _uow.AssessmentResults.FirstOrDefault(o => o.ScoreBoardId == nextBoard.ID && !o.Deleted);
                                if (nextResult != null)
                                {
                                    nextResult.LastAudit = boardResult?.AuditDate;
                                    nextResult.LastRiskLevel = string.IsNullOrEmpty(boardResult?.RiskLevelChangeName ?? "") ? boardResult?.LastRiskLevel : boardResult?.RiskLevelChangeName;
                                    _uow.AssessmentResults.Update(nextResult);
                                }
                            }
                        }
                    }
                });

                assessmentStage.StageState = 1;
                assessmentStage.ModifiedBy = userInfo.Id;
                assessmentStage.LastModified = DateTime.Now;
                _uow.AssessmentStages.Update(assessmentStage);
            }

            return Ok(new { code = allowEnd ? "001" : "102", msg = _msg });
        }
        [HttpGet("ResultSearchScope")]
        public IActionResult ResultSearchScope(string jsonData)
        {

            if (HttpContext.Items["UserInfo"] is not User userInfo)
            {
                return Unauthorized();
            }
            var obj = JsonSerializer.Deserialize<ScopeSearch>(jsonData);
            obj.DomainId = userInfo.DomainId;

            if (obj.Year == null || obj.Stage == null)
                return BadRequest();
            var lst = new List<ScopeSearchResultView>();
            ////var processlst = _uow.ScoreBoard.GetprocessBoard().ToArray();
            var processlst = _uow.AuditProcess.Find(a => !a.Deleted).Select(i => new ProcessResultView()
            {
                ID = i.ID,
                ProcessCode = i.Code,
                ProcessName = i.Name,
                ProcessId = i.ID,
                ActivityId = i.ActivityId,
                ActivityName = i.ActivityName,
                FacilityId = i.FacilityId,
                FacilityName = i.FacilityName,
            });
            switch (obj.ApplyFor)
            {
                case "QT":
                    lst = _uow.AssessmentResults.SearchScope(obj, 1).ToList();
                    lst.ForEach(o =>
                    {
                        var lstprocess = processlst.FirstOrDefault(a => a.ID == o.ObjectId);
                        o.ActivityName = lstprocess?.ActivityName;
                        o.FacilityName = lstprocess?.FacilityName;
                    });
                    break;
                case "DV":
                    lst = _uow.AssessmentResults.SearchScope(obj, 2).ToList();
                    lst.ForEach(o =>
                    {
                        var lstprocess = processlst.Where(a => a.FacilityId == o.ObjectId && (obj.keyprocess == "" || a.ProcessName.ToLower().Contains(obj.keyprocess.Trim().ToLower()))).ToList();
                        o.sub_activities = lstprocess;
                    });
                    if (!string.IsNullOrEmpty(obj.keyprocess))
                    {
                        lst.RemoveAll(a => a.sub_activities.Count == 0);
                    }
                    break;
                case "HDKD":
                    lst = _uow.AssessmentResults.SearchScope(obj, 3).ToList();
                    lst.ForEach(o =>
                    {
                        var lstprocess = processlst.Where(a => a.ActivityId == o.ObjectId && (obj.keyprocess == "" || a.ProcessName.ToLower().Contains(obj.keyprocess.Trim().ToLower()))).ToList();
                        o.sub_activities = lstprocess;
                    });
                    if (!string.IsNullOrEmpty(obj.keyprocess))
                    {
                        lst.RemoveAll(a => a.sub_activities.Count == 0);
                    }
                    break;
                case "":
                    lst = _uow.AssessmentResults.SearchScope(obj, 4).ToList();
                    break;
            }
            if (obj.RiskLevel.HasValue)
            {
                lst = lst.Where(a => string.IsNullOrEmpty(a.RiskLevelChangeName) ? a.RiskLevel.Contains(obj.RiskLevelName) : a.RiskLevelChange == obj.RiskLevel).ToList();
            }

            return Ok(new { code = "001", data = lst });
        }
        [HttpPost("GetDataScopeResult")]
        public IActionResult GetDataScopeResult(ScopeGetInfo listassign)
        {
            if (HttpContext.Items["UserInfo"] is not User userInfo)
            {
                return Unauthorized();
            }
            var _lst = new List<ScopeResultView>();
            var _lst_facility = new List<ScopeFacilityResultView>();

            if (listassign != null && listassign.lstselect.Count > 0)
            {
                var lstItem = listassign.lstselect;
                var lst = new List<int?>();
                var lstMain = lstItem.Select(a => a.main_id).ToArray();
                lst.AddRange(lstMain);
                var scoreBoard = _uow.ScoreBoard.Find(a => lst.Contains(a.ID)).ToArray();
                var auditprocess = _uow.AuditProcess.Find(a => a.DomainId == userInfo.DomainId && !a.Deleted).ToArray();
                var reason = _uow.SystemCategories.Find(a => a.Status && a.DomainId == userInfo.DomainId && a.ParentGroup == "LyDoKiemToan" && !a.Deleted).ToArray();
                var result = _uow.AssessmentResults.Find(a => lst.Contains(a.ScoreBoardId) && !a.Deleted).ToArray();
                var list_dv = new List<int?>();
                var list_dv_check = new List<int?>();
                var list_qt_check = new List<int?>();
                foreach (var item in lstItem)
                {
                    var _item = scoreBoard.FirstOrDefault(a => a.ID == item.main_id);
                    var select_id = item.select_id;
                    var apply_for = item.apply_for;
                    if (_item.ApplyFor == "QT")
                    {
                        var _listprocess = auditprocess.FirstOrDefault(a => a.ID == _item.ObjectId);
                        var _result = result.FirstOrDefault(a => a.ScoreBoardId == _item.ID);
                        var _reason = "";
                        if (_result != null)
                            _reason = reason.FirstOrDefault(a => a.ID == _result.AuditReason)?.Name;
                        var _lstItem = new ScopeResultView()
                        {
                            ID = _item.ID,
                            ProcessName = _item.ObjectName,
                            FacilityName = _listprocess?.FacilityName,
                            ActivityName = _listprocess?.ActivityName,
                            ProcessID = _item.ObjectId,
                            FacilityID = _listprocess?.FacilityId,
                            ActivityID = _listprocess?.ActivityId,
                            RiskLevel = _item.RiskLevel,
                            RiskLevelChange = _result?.RiskLevelChange,
                            RiskLevelChangeName = _result?.RiskLevelChangeName,
                            IsLevelChange = _result.RiskLevel != _result?.RiskLevelChange,
                            AuditReason = _reason,
                            LastAuditTime = _result.LastAudit,
                            LastAudit = !_result.LastAudit.HasValue ? "" : _result.LastAudit.Value.ToString("MM/yyyy"),
                        };
                        list_dv.Add(_listprocess?.FacilityId);
                        _lst.Add(_lstItem);
                        list_qt_check.Add(_item.ObjectId);
                    }
                    else
                    {
                        if (apply_for != "")
                        {

                            var _result = result.FirstOrDefault(a => a.ScoreBoardId == _item.ID);
                            var _reason = "";
                            if (_result != null)
                                _reason = reason.FirstOrDefault(a => a.ID == _result.AuditReason)?.Name;
                            var _process = auditprocess.FirstOrDefault(a => a.ID == select_id);
                            if (_process != null)
                            {
                                if (!list_qt_check.Contains(_process?.ID))
                                {
                                    var _lstItem = new ScopeResultView()
                                    {
                                        ID = _item.ID,
                                        ProcessName = _process.Name,
                                        FacilityName = _process.FacilityName,
                                        ActivityName = _process.ActivityName,
                                        ProcessID = _process.ID,
                                        FacilityID = _process.FacilityId,
                                        ActivityID = _process.ActivityId,
                                        RiskLevel = "",//_item.RiskLevel,
                                                       //RiskLevelChange = _result?.RiskLevelChange,
                                        RiskLevelChangeName = "",//_result?.RiskLevelChangeName,
                                                                 //IsLevelChange = _result.RiskLevel != _result?.RiskLevelChange,
                                        AuditReason = "",// _reason,
                                                         //LastAuditTime = _result.LastAudit,
                                                         //LastAudit = !_result.LastAudit.HasValue ? "" : _result.LastAudit.Value.ToString("MM/yyyy"),
                                    };
                                    _lst.Add(_lstItem);
                                }
                            }
                            var facilityid = _process?.FacilityId ?? _item.ObjectId;
                            if (apply_for == "DV")
                            {
                                //var _lst_facility_tem = new ScopeFacilityResultView()
                                //{
                                //    ID = _item.ID,
                                //    FacilityName = _item.ObjectName,
                                //    FacilityID = _item.ObjectId,
                                //    RiskLevel = _item.RiskLevel,
                                //    RiskLevelChange = _result?.RiskLevelChange,
                                //    RiskLevelChangeName = _result?.RiskLevelChangeName,
                                //    IsLevelChange = _result.RiskLevel != _result?.RiskLevelChange,
                                //    AuditReason = _reason,
                                //    LastAuditTime = _result.LastAudit,
                                //    LastAudit = !_result.LastAudit.HasValue ? "" : _result.LastAudit.Value.ToString("MM/yyyy"),
                                //};
                                //_lst_facility.Add(_lst_facility_tem);
                                
                                list_dv.Add(facilityid);
                            }
                            if (apply_for == "HDKD")
                            {
                                list_dv.Add(facilityid);
                            }
                        }
                        else
                        {
                            var _result = result.FirstOrDefault(a => a.ScoreBoardId == _item.ID);
                            var _reason = "";
                            if (_result != null)
                                _reason = reason.FirstOrDefault(a => a.ID == _result.AuditReason)?.Name;
                            if (_item.ApplyFor == "DV")
                            {
                                var lstprocess = auditprocess.Where(a => a.FacilityId == _item.ObjectId).ToList();
                                foreach (var item_ in lstprocess)
                                {
                                    if (!list_qt_check.Contains(item_.ID))
                                    {
                                        var _lstItem = new ScopeResultView()
                                        {
                                            ID = _item.ID,
                                            ProcessName = item_.Name,
                                            FacilityName = item_.FacilityName,
                                            ActivityName = item_.ActivityName,
                                            ProcessID = item_.ID,
                                            FacilityID = item_.FacilityId,
                                            ActivityID = item_.ActivityId,
                                            RiskLevel = "",
                                            RiskLevelChangeName = "",
                                            AuditReason = "",
                                            LastAudit = "",
                                        };
                                        _lst.Add(_lstItem);
                                    }

                                }

                                var _lst_facility_tem = new ScopeFacilityResultView()
                                {
                                    ID = _item.ID,
                                    FacilityName = _item.ObjectName,
                                    FacilityID = _item.ObjectId,
                                    RiskLevel = _item.RiskLevel,
                                    RiskLevelChange = _result?.RiskLevelChange,
                                    RiskLevelChangeName = _result?.RiskLevelChangeName,
                                    IsLevelChange = _result.RiskLevel != _result?.RiskLevelChange,
                                    AuditReason = _reason,
                                    LastAuditTime = _result.LastAudit,
                                    LastAudit = !_result.LastAudit.HasValue ? "" : _result.LastAudit.Value.ToString("MM/yyyy"),
                                };
                                _lst_facility.Add(_lst_facility_tem);
                                list_dv_check.Add(_item.ObjectId);
                            }
                            else if (_item.ApplyFor == "HDKD")
                            {
                                var lstprocess = auditprocess.Where(a => a.ActivityId == _item.ObjectId).ToList();
                                foreach (var item_ in lstprocess)
                                {
                                    if (!list_qt_check.Contains(item_.ID))
                                    {
                                        var _lstItem = new ScopeResultView()
                                        {
                                            ID = _item.ID,
                                            ProcessName = item_.Name,
                                            FacilityName = item_.FacilityName,
                                            ActivityName = item_.ActivityName,
                                            ProcessID = item_.ID,
                                            FacilityID = item_.FacilityId,
                                            ActivityID = item_.ActivityId,
                                            RiskLevel = "",
                                            RiskLevelChangeName = "",
                                            AuditReason = "",
                                            LastAudit = "",
                                        };
                                        _lst.Add(_lstItem);
                                    }
                                    list_dv.Add(item_?.FacilityId);
                                }
                            }
                        }
                    }
                }
                var list_dv_final = list_dv.Where(a => !list_dv_check.Contains(a)).Distinct().ToList();
                var year = lstItem.FirstOrDefault()?.year;
                var scoreBoard_dv = _uow.ScoreBoard.Find(a => list_dv_final.Contains(a.ObjectId) && a.Year == year).AsEnumerable().GroupBy(a => a.ObjectId).Select(g => g.OrderByDescending(x => x.ID).FirstOrDefault()).ToArray();
                var scoreBoard_dv_id = scoreBoard_dv.Select(a => a.ID).ToArray();
                var result_dv = _uow.AssessmentResults.Find(a => scoreBoard_dv_id.Contains(a.ScoreBoardId) && !a.Deleted).ToArray();
                foreach (var item in list_dv_final)
                {
                    var _scoreBoard = scoreBoard_dv.FirstOrDefault(a => a.ObjectId == item);
                    if (_scoreBoard != null)
                    {
                        var _result = result_dv.FirstOrDefault(a => a.ScoreBoardId == _scoreBoard.ID);
                        var _reason = "";
                        if (_result != null)
                            _reason = reason.FirstOrDefault(a => a.ID == _result.AuditReason)?.Name;
                        var _lst_facility_tem = new ScopeFacilityResultView()
                        {
                            ID = _scoreBoard.ID,
                            FacilityName = _scoreBoard.ObjectName,
                            FacilityID = _scoreBoard.ObjectId,
                            RiskLevel = _scoreBoard.RiskLevel,
                            RiskLevelChange = _result?.RiskLevelChange,
                            RiskLevelChangeName = _result?.RiskLevelChangeName,
                            IsLevelChange = _result.RiskLevel != _result?.RiskLevelChange,
                            AuditReason = _reason,
                            LastAuditTime = _result.LastAudit,
                            LastAudit = !_result.LastAudit.HasValue ? "" : _result.LastAudit.Value.ToString("MM/yyyy"),
                        };
                        _lst_facility.Add(_lst_facility_tem);
                    }
                    else
                    {
                        var facility = _uow.AuditFacilities.FirstOrDefault(a => a.DomainId == userInfo.DomainId && !a.Deleted && a.ID == item);
                        if (facility != null)
                        {
                            var _lst_facility_tem = new ScopeFacilityResultView()
                            {
                                FacilityName = facility.Name,
                                FacilityID = facility.ID,
                                RiskLevel = "",
                                RiskLevelChangeName = "",
                                AuditReason = "",
                                LastAudit = "",
                            };
                            _lst_facility.Add(_lst_facility_tem);
                        }

                    }

                }
            }
            return Ok(new { code = "1", data = _lst, data_facility = _lst_facility, msg = "success" });
        }
        #endregion
    }
}
