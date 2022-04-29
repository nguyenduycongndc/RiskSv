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

namespace RiskAssessment.Api.Controllers
{
    [ApiController]
    [Route("RiskIssues")]
    public class RiskIssueController : BaseController
    {
        public RiskIssueController(ILoggerManager logger
            , IUnitOfWork uow
            , IMapper mapper
            , IConfiguration config
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

                        var _data = Utils.ExcelFn.ExcelToList<RiskIssueDto>(fullPath);

                        bool hasError = false;
                        ////var trans = _uow.BeginTransaction();
                        var userInfo = HttpContext.Items["UserInfo"] as User;
                        var allCode = _data.Select(o => o.Code).ToList();
                        var codeInDb = _uow.AuditFacilities.Find(o => o.DomainId == userInfo.DomainId).Select(o => o.Code).ToList();
                        var s = _uow.AuditFacilities.Find(o => o.DomainId == userInfo.DomainId && allCode.Contains(o.Code)).Select(o => o.Code).ToList();

                        var parentCode = _data.Where(o => !string.IsNullOrEmpty(o.ParentCode)).Select(o => o.ParentCode).ToList();

                        var x = parentCode.Where(o => !allCode.Exists(a => a == o)).ToList();
                        var y = x.Where(o => !codeInDb.Exists(a => a == o)).ToList();

                        var doiTuongApDungs = _uow.SystemCategories.Find(o => o.Status && o.DomainId == userInfo.DomainId && o.ParentGroup == "DoiTuongApDung").ToList();
                        ////var loaiDonViApDungs = _uow.SystemCategories.GetUnitTypes().ToList();
                        var formulas = _uow.Formulas.Find(o => o.Status && o.DomainId == userInfo.DomainId).ToList();


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
                                o.Note += "Name cannot be null!<br />";
                            }
                            if (s.Contains(o.Code))
                            {
                                o.Valid = false;
                                o.Note += "Code has exists!<br />";
                            }
                            if (!y.Contains(o.ParentCode))
                            {
                                o.Valid = false;
                                o.Note += "parent code not exists!<br />";
                            }
                            if (o.Code == o.ParentCode)
                            {
                                o.Valid = false;
                                o.Note += "Code and parent code is same!<br />";
                            }
                            var doiTuong = doiTuongApDungs.FirstOrDefault(a => a.Code == o.ApplyCode);
                            if (doiTuong == null)
                            {
                                o.Valid = false;
                                o.Note += "Apply for: not exists!<br />";
                            }
                            else o.ApplyFor = _mapper.Map<SystemCategoryDto>(doiTuong);

                            ////var donvi = loaiDonViApDungs.FirstOrDefault(a => a.code == o.ClassCode);
                            ////if (donvi == null)
                            ////{
                            ////    o.Valid = false;
                            ////    o.Note += "Class type: not exists!<br />";
                            ////}
                            ////else o.ClassType = donvi;


                            var formu = formulas.FirstOrDefault(a => a.Code == o.FormulaCode);
                            if (formu == null)
                            {
                                o.Valid = false;
                                o.Note += "Formula: not exists!<br />";
                            }
                            else o.Formula = _mapper.Map<FormulaDto>(formu);



                            if (string.IsNullOrEmpty(o.ParentCode) || codeInDb.Contains(o.ParentCode))
                                o.Batch = 0;
                            else o.Batch = 1;
                            var pr = _uow.RiskIssues.FirstOrDefault(a => o.Code != "" && a.Code == o.ParentCode);
                            if (pr != null)
                                o.Parent = _mapper.Map<RiskIssueDto>(pr);
                        });

                        hasError = _data.Any(o => !o.Valid);

                        if (!hasError)
                        {
                            var batch0 = _data.Where(o => o.Batch == 0);
                            foreach (var item in batch0)
                            {
                                var obj = _mapper.Map<RiskIssue>(item);
                                obj.UserCreate = userInfo.Id;
                                obj.CreateDate = DateTime.Now;
                                obj.DomainId = userInfo.DomainId;
                                obj.Status = true;
                                _uow.RiskIssues.Add(obj);
                                var child = _data.Where(o => o.ParentCode == obj.Code).ToList();
                                child.ForEach(o => o.Parent = _mapper.Map<RiskIssueDto>(obj));
                            }

                            var batch1 = _data.Where(o => o.Batch == 1);
                            foreach (var item in batch1)
                            {
                                var obj = _mapper.Map<RiskIssue>(item);
                                obj.UserCreate = userInfo.Id;
                                obj.CreateDate = DateTime.Now;
                                obj.DomainId = userInfo.DomainId;
                                obj.Status = true;
                                _uow.RiskIssues.Add(obj);
                            }
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


        private void FillParent(RiskIssueDto obj)
        {
            if (obj.Formula != null)
            {
                var p = _uow.Formulas.FirstOrDefault(a => a.ID == obj.Formula.ID);
                if (p != null)
                {
                    var pm = _mapper.Map<FormulaDto>(p);
                    obj.Formula = pm;
                }
            }
            if (obj.ApplyFor != null)
            {
                var p = _uow.SystemCategories.FirstOrDefault(a => a.ID == obj.ApplyFor.ID);
                if (p != null)
                {
                    var pm = _mapper.Map<SystemCategoryDto>(p);
                    obj.ApplyFor = pm;
                }
            }
            ////if (obj.ClassType != null)
            ////{
            ////    var p = _uow.SystemCategories.GetUnitTypes().FirstOrDefault(a => a.id == obj.ClassType.id);
            ////    if (p != null)
            ////    {
            ////        obj.ClassType = p;
            ////    }
            ////}

            if (obj.Parent != null && obj.ID == obj.Parent.ID)
                obj.Parent = null;

            if (obj.Parent != null && obj.ID != obj.Parent.ID)
            {
                var p = _uow.RiskIssues.FirstOrDefault(a => a.ID == obj.Parent.ID);
                if (p != null)
                {
                    var pm = _mapper.Map<RiskIssueDto>(p);
                    FillParent(pm);
                    obj.Parent = pm;
                }
                else obj.Parent = null;
            }
        }

        private static RiskIssueDto GetParent(RiskIssueDto obj)
        {
            var item = obj;
            while (item.Parent != null)
                item = item.Parent;
            return item;
        }
        private List<RiskIssueDto> GetChilds(RiskIssueDto root, List<RiskIssueDto> data)
        {
            var childs = new List<RiskIssueDto>();
            if (root == null)
                return new List<RiskIssueDto>();
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
                o.SubIssues = GetChilds(o, data);
            });

            return childs;
        }
        private void ClearCycle(RiskIssueDto o)
        {
            o.Parent = null;
            o.SubIssues.ForEach(a => ClearCycle(a));
        }

        private void UpdateScoreByIssueStatus(ScoreBoardIssue item)
        {
            var child = _uow.ScoreBoard.GetIssues(item.ScoreBoardId).Where(o => o.IssueParentId == item.IssueId).ToList();
            if (child.Count > 0)
            {
                child.ForEach(o =>
                {
                    o.Status = item.Status;
                    o = _uow.ScoreBoard.UpdateIssues(o);
                    UpdateScoreByIssueStatus(o);
                });
            }
        }
        private void ReCalculatorForScoreboard(RiskIssue obj, User userInfo)
        {
            var riskAssessmentScales = _uow.RiskAssessmentScales.Find(a => a.RiskIssueId == obj.ID && !a.Deleted && a.Status && a.DomainId == userInfo.DomainId).OrderBy(a => a.Point).ToList();
            var riskAssessmentScale = riskAssessmentScales.FirstOrDefault();
            if (riskAssessmentScale == null)
                return;


            var allApplyItems = _uow.ScoreBoard.IssueUpdateIncomplete(obj.ID, userInfo.DomainId).ToList();
            if (allApplyItems == null || !allApplyItems.Any())
                return;

            var lstIdBigChange = new List<ScoreBoardIssue>();

            allApplyItems.ForEach(o =>
            {
                var hasChange = o.Name != obj.Name
                                || o.Code != obj.Code
                                || o.IssueParentId != obj.ParentId
                                || o.Proportion != obj.Proportion
                                || o.MethodId != obj.MethodId
                                || o.Formula != obj.FormulaName
                                || o.FormulaId != obj.Formula
                                || o.Status != obj.Status;

                if (hasChange)
                {
                    o.Name = obj.Name;
                    o.Code = obj.Code;
                    o.IssueParentId = obj.ParentId;
                    if (o.ProportionModify == o.Proportion)
                        o.ProportionModify = obj.Proportion;
                    o.Proportion = obj.Proportion;

                    if (o.MethodId != obj.MethodId || o.FormulaId != obj.Formula || o.Status != obj.Status)
                    {
                        o.Formula = obj.FormulaName;
                        o.FormulaId = obj.Formula;
                        o.Status = obj.Status;

                        if (o.MethodId != obj.MethodId)
                        {
                            o.MethodId = obj.MethodId;
                            o.RiskValue = null;
                            o.Point = null;

                            var pointRange = new List<RiskPointRange>();
                            if (o.MethodId == 0)
                            {
                                var val = riskAssessmentScale.MinValue ?? 0;
                                var max = riskAssessmentScale.MaxValue ?? 0;
                                while (val <= max)
                                {
                                    pointRange.Add(new RiskPointRange()
                                    {
                                        Point = val,
                                        Condition = "",
                                        Code = riskAssessmentScale.Code,
                                        Name = riskAssessmentScale.Name,
                                        Id = riskAssessmentScale.ID
                                    });
                                    val++;
                                }
                            }
                            else
                            {
                                foreach (var rs in riskAssessmentScales)
                                {
                                    pointRange.Add(new RiskPointRange()
                                    {
                                        Point = rs.Point,
                                        Condition = rs.Condition,
                                        Code = rs.Code,
                                        Name = rs.Name,
                                        Id = rs.ID
                                    });
                                }
                            }
                            o.PointRange = JsonSerializer.Serialize(pointRange);
                        }

                        if (!lstIdBigChange.Exists(a => a.ID == o.ID))
                            lstIdBigChange.Add(o);
                    }

                    o = _uow.ScoreBoard.UpdateIssues(o);
                }
            });
            if (lstIdBigChange.Any())
            {
                var formulas = _uow.Formulas.Find(o => !o.Deleted && o.Status && o.DomainId == userInfo.DomainId).ToList();
                lstIdBigChange.ForEach(item =>
                {
                    UpdateScoreByIssueStatus(item);

                    var self = _uow.ScoreBoard.FirstOrDefault(a => a.ID == item.ScoreBoardId);
                    var allIssues = _uow.ScoreBoard.GetIssues(self.ID).ToList();

                    var isParent = allIssues.Exists(a => a.IssueParentId == item.IssueId);

                    if (item.Status && isParent)
                    {
                        var fml = formulas.FirstOrDefault(o => o.ID == item.FormulaId);
                        if (fml != null)
                        {
                            var childs = allIssues.Where(o => o.IssueParentId == item.IssueId).ToList();
                            double val = 0;
                            if (fml.Code == "TONG")
                            {
                                childs.ForEach(a => val += (a.Point ?? 0));
                            }
                            else if (fml.Code == "BINHQUANGIAQUYEN")
                            {
                                childs.ForEach(o =>
                                {
                                    var propor = o.ProportionModify == null || o.ProportionModify == 0 ? o.Proportion : o.ProportionModify;
                                    val += (o.Point ?? 0) * ((double)propor / 100);
                                });
                            }
                            else if (fml.Code == "TRUNGBINHCONG")
                            {
                                childs.ForEach(o =>
                                {
                                    val += (o.Point ?? 0);
                                });
                                val /= childs.Count;
                            }
                            item.Point = val;

                            item.ModifiedBy = userInfo.Id;
                            item.LastModified = DateTime.Now;

                            item = _uow.ScoreBoard.UpdateIssues(item);
                        }
                    }

                    allIssues = CalculatorForParent(allIssues, item, userInfo);
                    CalculatorRoot(allIssues, self, userInfo);
                    CalculatorForScoreboard(self, userInfo);
                });
            }
        }

        private void UpdateBoardByDelete(RiskIssue obj, User userInfo)
        {
            var riskAssessmentScales = _uow.RiskAssessmentScales.Find(a => a.RiskIssueId == obj.ID && !a.Deleted && a.Status && a.DomainId == userInfo.DomainId).OrderBy(a => a.Point).ToList();
            var riskAssessmentScale = riskAssessmentScales.FirstOrDefault();
            if (riskAssessmentScale == null)
                return;


            var allApplyItems = _uow.ScoreBoard.IssueUpdateIncomplete(obj.ID, userInfo.DomainId).ToList();
            if (allApplyItems == null || !allApplyItems.Any())
                return;

            allApplyItems.ForEach(item =>
            {
                item.Deleted = true;

                item.ModifiedBy = userInfo.Id;
                item.LastModified = DateTime.Now;
                item = _uow.ScoreBoard.UpdateIssues(item);

                var self = _uow.ScoreBoard.FirstOrDefault(a => a.ID == item.ScoreBoardId);
                var allIssues = _uow.ScoreBoard.GetIssues(self.ID).ToList();

                allIssues = CalculatorForParent(allIssues, item, userInfo);
                CalculatorRoot(allIssues, self, userInfo);
                CalculatorForScoreboard(self, userInfo);
            });
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

            var result = _uow.RiskIssues.Search(obj, out int count).Select(o => _mapper.Map<RiskIssueDto>(o)).ToList();
            var roots = new List<RiskIssueDto>();
            result.ForEach(o =>
            {
                o.SubIssues.Clear();
                FillParent(o);
                var root = GetParent(o);
                if (!roots.Exists(a => a.ID == root.ID))
                    roots.Add(root);
            });

            roots.ForEach(o =>
            {
                o.ancestor = "|" + o.ID + "|";
                o.SubIssues = GetChilds(o, result);
                ClearCycle(o);
            });


            return Ok(new { code = "001", msg = "success", data = roots.OrderByDescending(o=>o.CreateDate), total = count });
        }
        [HttpGet("Parents")]
        public IActionResult GetParents()
        {
            if (HttpContext.Items["UserInfo"] is not User userInfo)
            {
                return Unauthorized();
            }
            var x = _uow.RiskIssues.Find(o => o.Status && o.DomainId == userInfo.DomainId && o.ParentId == null).ToList();
            var lst = x.Select(o => _mapper.Map<RiskIssueDto>(o)).ToList();

            return Ok(new { code = "001", msg = "success", data = lst, total = lst.Count });
        }


        [HttpPost]
        public IActionResult UpdateItem(RiskIssueDto obj)
        {
            var current_id = obj.ID;
            if (HttpContext.Items["UserInfo"] is not User userInfo)
            {
                return Unauthorized();
            }

            if (!obj.IsValid())
                return BadRequest(new { msg = "Invalid data!" });

            RiskIssue riskIssue = null;
            SystemCategory applyFor = null;
            ////UnitTypeView clsType = null;
            Formula formula = null;

            if (obj.Parent != null && obj.Parent.ID > 0)
            {
                riskIssue = _uow.RiskIssues.FirstOrDefault(o => o.ID == obj.Parent.ID);
                if (riskIssue != null)
                    obj.Parent = _mapper.Map<RiskIssueDto>(riskIssue);
                else return BadRequest(new { code = "0", msg = "invalid risk issue" });
            }
            if (obj.ApplyFor != null && obj.ApplyFor.ID > 0)
            {
                applyFor = _uow.SystemCategories.FirstOrDefault(o => o.ID == obj.ApplyFor.ID);
                if (applyFor != null)
                    obj.ApplyFor = _mapper.Map<SystemCategoryDto>(applyFor);
                else return BadRequest(new { code = "0", msg = "invalid apply-for" });
            }
            ////if (obj.ClassType != null && obj.ClassType.id > 0)
            ////{
            ////    clsType = _uow.SystemCategories.GetUnitTypes().FirstOrDefault(o => o.id == obj.ClassType.id);
            ////    if (clsType != null)
            ////        obj.ClassType = clsType;
            ////    else return BadRequest(new { code = "0", msg = "invalid class type" });
            ////}

            if (applyFor != null && applyFor.Code == "DV")
            {
                var unitTypesQr = _uow.SystemCategories.GetUnitTypes();
                if (string.IsNullOrEmpty(obj.ClassType))
                {
                    var unitTypes = unitTypesQr.ToList();
                    obj.ClassType = string.Join(";", unitTypes.Select(o => o.id).ToArray());
                    obj.ClassTypeName = string.Join("; ", unitTypes.Select(o => o.name).ToArray());
                }
                else
                {
                    var arId = obj.ClassType.Split(';').Where(o => !string.IsNullOrEmpty(o) && int.TryParse(o, out _)).Select(o => int.Parse(o)).ToArray();
                    var unitTypes = unitTypesQr.Where(o => arId.Contains(o.id)).Select(o => o.name).ToList();
                    obj.ClassTypeName = string.Join("; ", unitTypes);
                }
            }

            if (obj.Formula != null && obj.Formula.ID > 0)
            {
                formula = _uow.Formulas.FirstOrDefault(o => o.ID == obj.Formula.ID);
                if (formula != null)
                    obj.Formula = _mapper.Map<FormulaDto>(formula);
                else return BadRequest(new { code = "0", msg = "invalid formula" });
            }
            if (!_uow.RiskIssues.CheckValidCode(obj.Code, obj.ID, userInfo.DomainId))
                return Ok(new { code = "003", msg = "", data = obj });

            if (riskIssue != null)
            {
                var childs = _uow.RiskIssues.GetAll().Where(o => o.ParentId == riskIssue.ID && !o.Deleted && o.ID != obj.ID);
                var total = childs.Sum(o => o.Proportion ?? 0);
                total += obj.Proportion ?? 0;
                if (total > 100)
                    return Ok(new { code = "101", msg = "", data = obj });
            }
            else
            {
                if (!obj.Proportion.HasValue || obj.Proportion <= 0)
                    return Ok(new { code = "109", msg = "", data = obj });

                var qrApplyFor = _uow.RiskIssues.Find(o => !o.Deleted && o.DomainId == userInfo.DomainId
                && o.Status && o.ID != obj.ID
                && (o.ParentId == null || o.ParentId == 0));
                if (applyFor != null)
                {
                    qrApplyFor = qrApplyFor.Where(o => o.ApplyFor == applyFor.ID);
                    if (applyFor.Code == "DV")
                    {
                        qrApplyFor = qrApplyFor.Where(o => o.ClassType.Contains(obj.ClassType));
                    }
                }
                var parentApplyFor = qrApplyFor.ToList();

                var total = 0;
                parentApplyFor.ForEach(o => total += (o.Proportion ?? 0));
                total += obj.Proportion ?? 0;

                if (total > 100)
                    return Ok(new { code = "101", msg = "", data = obj });
            }



            var scoreBoardIds = new List<int>();
            if (obj.ID > 0)
            {
                var ex = _uow.RiskIssues.FirstOrDefault(o => o.ID == obj.ID);
                if (ex.MethodId != obj.MethodId || (obj.ApplyFor != null && ex.ApplyFor != obj.ApplyFor.ID))
                {
                    var issueIncomplete = _uow.ScoreBoard.IssueUpdateIncomplete(ex.ID, userInfo.DomainId).ToList();
                    issueIncomplete.ForEach(o =>
                    {
                        o.ModifiedBy = userInfo.Id;
                        o.LastModified = DateTime.Now;
                        o.Deleted = true;
                        o = _uow.ScoreBoard.UpdateIssues(o);
                        if (!scoreBoardIds.Contains(o.ScoreBoardId))
                            scoreBoardIds.Add(o.ScoreBoardId);
                    });

                    var riskAssessmentScales = _uow.RiskAssessmentScales.Find(o => !o.Deleted && o.RiskIssueId == obj.ID).ToList();

                    if (riskAssessmentScales.Any())
                    {
                        riskAssessmentScales.ForEach(o =>
                        {
                            o.ModifiedBy = userInfo.Id;
                            o.LastModified = DateTime.Now;
                            o.Deleted = true;
                            _uow.RiskAssessmentScales.Update(o);
                        });
                    }
                }

                ex = obj.Map(ex);
                ex.Proportion = obj.Proportion;
                ex.MethodId = obj.MethodId;
                if (riskIssue != null)
                    ex.ParentId = riskIssue.ID;
                if (applyFor != null)
                {
                    ex.ApplyFor = applyFor.ID;
                    ex.ApplyForName = applyFor.Name;
                }
                if (formula != null)
                {
                    ex.Formula = formula.ID;
                    ex.FormulaName = formula.Name;
                }
                ex.ClassType = obj.ClassType;
                ex.ClassTypeName = obj.ClassTypeName;

                ex.DomainId = userInfo.DomainId;
                ex.ModifiedBy = userInfo.Id;
                ex.LastModified = DateTime.Now;

                ex = _uow.RiskIssues.Update(ex);
                obj = _mapper.Map<RiskIssueDto>(ex);

                ReCalculatorForScoreboard(ex, userInfo);

                if (scoreBoardIds.Count > 0)
                {
                    scoreBoardIds.ForEach(o =>
                    {
                        var self = _uow.ScoreBoard.FirstOrDefault(a => a.ID == o && !a.Deleted && a.DomainId == userInfo.DomainId && a.Status);
                        if (self != null)
                        {
                            base.RefreshScoreBoard(self, userInfo, applyFor, ex.ClassType);
                        }
                    });
                }
            }
            else
            {
                var item = _mapper.Map<RiskIssue>(obj);
                item.UserCreate = userInfo.Id;
                item.CreateDate = DateTime.Now;
                item.DomainId = userInfo.DomainId;
                _uow.RiskIssues.Add(item);
                obj = _mapper.Map<RiskIssueDto>(item);
            }

            return Ok(new { code = "001", msg = "success", data = obj, id = current_id });
        }

        private void ActiveByParent(RiskIssue c, bool status, User userInfo)
        {
            c.Status = status;
            c.ModifiedBy = userInfo.Id;
            c.LastModified = DateTime.Now;
            var childs = _uow.RiskIssues.Find(o => o.ParentId == c.ID && o.Status == !status);
            foreach (var cd in childs)
            {
                ActiveByParent(cd, status, userInfo);
            }
            _uow.RiskIssues.Update(c);
        }

        [HttpPost("Delete")]
        public IActionResult DeleteItem(RiskIssueDto obj)
        {
            if (HttpContext.Items["UserInfo"] is not User userInfo)
            {
                return Unauthorized();
            }
            var item = _uow.RiskIssues.FirstOrDefault(o => o.ID == obj.ID);
            if (item == null)
                return NotFound();
            ////if (item.UserCreate != userInfo.Id)
            ////{
            ////    _logger.LogError($"{userInfo.UserName} delete 'business activity' {item.ID}: don't have permission!");
            ////    return BadRequest();
            ////}
            var child = _uow.RiskIssues.Find(o => o.ParentId == obj.ID && !o.Deleted);
            if (child != null && child.Any())
                return Ok(new { code = "008" });

            var riskAssessments = _uow.RiskAssessmentScales.Find(o => o.RiskIssueId == item.ID && !o.Deleted).Any();
            if (riskAssessments)
            {
                return Ok(new { code = "010" });
            }
            var checkscoreboard = _uow.RiskIssues.CheckScoreIssue(item.ID);
            if (checkscoreboard)
            {
                return Ok(new { code = "010" });
            }
            if (child != null && child.Any())
                return Ok(new { code = "007" });

            item.Deleted = true;
            item.ModifiedBy = userInfo.Id;
            item.LastModified = DateTime.Now;
            _uow.RiskIssues.Update(item);

            UpdateBoardByDelete(item, userInfo);

            return Ok(new { code = "001", msg = "success", data = obj });
        }

        [HttpGet("{id}")]
        public IActionResult GetItem(int id)
        {
            var userInfo = HttpContext.Items["UserInfo"] as User;
            var item = _uow.RiskIssues.FirstOrDefault(o => o.ID == id);
            if (item == null || !_uow.RiskIssues.CheckPermission(item, userInfo.DomainId))
                return NotFound();
            var result = _mapper.Map<RiskIssueDto>(item);

            if (item.Formula.HasValue && item.Formula.Value > 0)
            {
                var form = _uow.Formulas.FirstOrDefault(o => o.ID == item.Formula);
                if (form != null && result.Formula != null)
                    result.Formula.Code = form.Code;
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
