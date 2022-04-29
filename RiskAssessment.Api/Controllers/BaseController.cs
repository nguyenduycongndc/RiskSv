using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RiskAssessment.Api.Attributes;
using RiskAssessment.Api.Common;
using RiskAssessment.Api.Models;
using RiskAssessment.DAO;
using RiskAssessment.Entity.DbEntities;
using Microsoft.Extensions.Configuration;
using System.IO;
using StackExchange.Redis;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using RiskAssessment.Entity.ViewModel;
using System.Text.Json;
using System.Text;

namespace RiskAssessment.Api.Controllers
{
    //[BaseAuthorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [BaseAuthorize]
    //[AllowAnonymous]
    public class BaseController : ControllerBase
    {
        protected readonly ILoggerManager _logger;
        protected readonly IUnitOfWork _uow;
        protected readonly IMapper _mapper;
        protected readonly IDatabase _redis;
        protected readonly IConfiguration _config;

        public BaseController(ILoggerManager logger, IUnitOfWork uow, IMapper mp, IDatabase iDb, IConfiguration config) : base()
        {
            _logger = logger;
            _uow = uow;
            _mapper = mp;
            _redis = iDb;
            _config = config;
        }
        protected string GenCodeDownload(string fileName)
        {
            var pathToSave = _config["Upload:ImportTemplates"];
            var fullPath = Path.Combine(pathToSave, fileName);

            if (!System.IO.File.Exists(fullPath))
                return "";

            var code = Guid.NewGuid().ToString().Replace("-", "");

            _redis.StringSet(code, fileName);
            return code;
        }
        protected string GetTempPath(string code, out string fileName)
        {
            fileName = _redis.StringGet(code);

            if (string.IsNullOrEmpty(fileName))
                return "";

            var pathToSave = _config["Upload:ImportTemplates"];
            var fullPath = Path.Combine(pathToSave, fileName);

            if (!System.IO.File.Exists(fullPath))
                return "";

            return fullPath;
        }

        protected List<ScoreBoardIssue> CalculatorForParent(List<ScoreBoardIssue> allIssues, ScoreBoardIssue item, User userInfo)
        {
            var parent = allIssues.Where(o => o.IssueId == item.IssueParentId && item.IsApply != false).FirstOrDefault();
            var formulas = _uow.Formulas.Find(o => !o.Deleted && o.Status && o.DomainId == userInfo.DomainId).ToList();

            while (parent != null)
            {
                var fml = formulas.FirstOrDefault(o => o.ID == parent.FormulaId);

                if (fml == null)
                    break;
                var childs = allIssues.Where(o => o.IssueParentId == parent.IssueId && item.IsApply != false).ToList();
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
                        val += (double)(o.Point ?? 0) * ((double)propor / 100);
                    });
                }
                else if (fml.Code == "TRUNGBINHCONG")
                {
                    childs.ForEach(o =>
                    {
                        val += (double)(o.Point ?? 0);
                    });
                    val = val / childs.Count;
                }

                parent.Point = Math.Round(val * 100, 0, MidpointRounding.AwayFromZero) / 100;

                parent.ModifiedBy = userInfo.Id;
                parent.LastModified = DateTime.Now;

                parent = _uow.ScoreBoard.UpdateIssues(parent);

                parent = allIssues.Where(o => o.IssueId == parent.IssueParentId && item.IsApply != false).FirstOrDefault();
            }
            return allIssues;
        }

        protected void CalculatorRoot(List<ScoreBoardIssue> data, ScoreBoard self, User userInfo)
        {
            var issueIds = data.Select(o => o.IssueId).Distinct().ToList();

            var inRoot = data.Where(o => o.IssueParentId == null || o.IssueParentId == 0 || !issueIds.Contains(o.IssueParentId ?? 0)).ToList();
            if (inRoot == null || inRoot.Count == 0)
                return;

            double val = 0;

            inRoot.ForEach(o =>
            {
                var propor = o.ProportionModify == null || o.ProportionModify == 0 ? (o.Proportion ?? 0) : o.ProportionModify;
                val += (double)(o.Point ?? 0) * ((double)propor / 100);
            });
            self.Point = Math.Round(val * 100, 0, MidpointRounding.AwayFromZero) / 100;
            self.ModifiedBy = userInfo.Id;
            self.LastModified = DateTime.Now;
            self = _uow.ScoreBoard.Update(self);
        }

        protected string CalculatorForScoreboard(ScoreBoard self, User userInfo)
        {
            var selfVal = self.Point;
            var assessmentResult = _uow.AssessmentResults.FirstOrDefault(o => o.ScoreBoardId == self.ID);
            var allIssues = _uow.ScoreBoard.GetIssues(self.ID).Where(o => o.Status && o.IsApply != false).ToList();
            var finished = allIssues.All(o => o.Point > 0);
            var notStart = allIssues.All(o => o.Point == null || o.Point == 0);

            var check = notStart ? -1 : (finished && !string.IsNullOrEmpty(self.RiskLevel)) ? 1 : 0;
            self.StateInfo = check == -1 ? "Chưa bắt đầu" : check == 0 ? "Đang thực hiện" : "Đã hoàn thành";

            var ratingScales = _uow.RatingScales.Find(o => o.DomainId == userInfo.DomainId && !o.Deleted & o.Status && o.ApplyFor == self.ApplyFor).ToList(); //huybt bổ sung loại áp dụng cho thang điểm xếp hạng

            ratingScales = ratingScales.Where(o => o.Max == null || (o.MaxFunction == "lt" && o.Max > selfVal) || (o.MaxFunction == "lte" && o.Max >= selfVal)).ToList();

            ratingScales = ratingScales.Where(o => ((o.MinFunction == "gt" && o.Min < selfVal) || (o.MinFunction == "gte" && o.Min <= selfVal))).ToList();

            if (ratingScales == null || ratingScales.Count == 0)
            {
                if (assessmentResult != null)
                {
                    assessmentResult.AssessmentStatus = check;
                    assessmentResult.ModifiedBy = userInfo.Id;
                    assessmentResult.LastModified = DateTime.Now;
                    _uow.AssessmentResults.Update(assessmentResult);
                }
                self = _uow.ScoreBoard.Update(self);
                return "001";
            }    
               

            var ratingScale = ratingScales.FirstOrDefault();

            self.RiskLevel = ratingScale.RiskLevelName;
            self.RatingScaleId = ratingScale.ID;

            var auditCycle = _uow.AuditCycle.FirstOrDefault(o => o.DomainId == userInfo.DomainId && !o.Deleted && o.RatingPoint == ratingScale.RiskLevel && o.Status);
            if (auditCycle != null)
                self.AuditCycleName = auditCycle.Name;

            if (assessmentResult != null)
            {
                assessmentResult.AssessmentStatus = notStart ? -1 : (finished && !string.IsNullOrEmpty(self.RiskLevel)) ? 1 : 0;
                assessmentResult.ModifiedBy = userInfo.Id;
                assessmentResult.LastModified = DateTime.Now;

                self.StateInfo = assessmentResult.AssessmentStatus == -1 ? "Chưa bắt đầu" : assessmentResult.AssessmentStatus == 0 ? "Đang thực hiện" : "Đã hoàn thành";
                _uow.AssessmentResults.Update(assessmentResult);
            }
            self = _uow.ScoreBoard.Update(self);

            return "001";
        }

        public string GetCondition(List<RiskAssessmentScale> objs, int method)
        {
            StringBuilder condition = new StringBuilder();
            foreach (var obj in objs)
            {
                if (obj == null)
                {
                    continue;
                }
                if (method == 1)
                {
                    condition.Append(obj.Condition);
                    continue;
                }
                var min = "";
                var max = "";
                if (obj.MinValue.HasValue)
                    min = obj.MinValue + " " + (obj.LowerConditionName == ">" ? "<" : "<=");
                if (obj.MaxValue.HasValue)
                    max = obj.UpperConditionName + " " + obj.MaxValue;
                if (min != "" || max != "")
                {
                    var str = string.Format("{0} Giá trị {1}", min, max);
                    condition.Append(str.Trim());
                    condition.Append("\n");
                }
            }
            return condition.ToString();
        }

        protected List<ScoreBoardIssue> InitBoardIssues(List<RiskIssue> isues, List<RiskIssue> notExists, int boardId, User userInfo, string applyFor)
        {
            var boardIssues = new List<ScoreBoardIssue>();
            foreach (RiskIssue o in notExists)
            {
                var pointRange = new List<RiskPointRange>();

                if (o.MethodId == 0)
                {
                    var lst_riskAssessmentScales = _uow.RiskAssessmentScales.Find(a => a.RiskIssueId == o.ID && !a.Deleted && a.Status && a.DomainId == userInfo.DomainId).OrderBy(a => a.CreateDate).ToList();
                    var riskAssessmentScale = lst_riskAssessmentScales.FirstOrDefault();
                    if (riskAssessmentScale != null)
                    {
                        var val = riskAssessmentScale.MinValue ?? 0;
                        var max = riskAssessmentScale.MaxValue ?? val;
                        while (val <= max)
                        {
                            pointRange.Add(new RiskPointRange()
                            {
                                Point = val,
                                Condition = GetCondition(lst_riskAssessmentScales, o.MethodId),
                                Code = riskAssessmentScale.Code,
                                Name = riskAssessmentScale.Name,
                                Id = riskAssessmentScale.ID
                            });
                            val++;
                        }
                    }
                }
                else
                {
                    var riskAssessmentScales = _uow.RiskAssessmentScales.Find(a => a.RiskIssueId == o.ID && !a.Deleted && a.Status && a.DomainId == userInfo.DomainId).OrderBy(a => a.Point).ToList();

                    if (riskAssessmentScales != null && riskAssessmentScales.Any())
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
                }
                var hasChilds = isues.Exists(a => a.ParentId == o.ID);
                if (pointRange.Any() || hasChilds)
                {
                    var issue = new ScoreBoardIssue()
                    {
                        UserCreate = userInfo.Id,
                        CreateDate = DateTime.Now,
                        CreateDateIssue = o.CreateDate,
                        DomainId = userInfo.DomainId,
                        Status = true,
                        Code = o.Code,
                        Name = o.Name,
                        ScoreBoardId = boardId,
                        IssueId = o.ID,
                        IssueParentId = o.ParentId,
                        Proportion = o.Proportion,
                        ProportionModify = o.Proportion,
                        ApplyFor = applyFor,
                        MethodId = o.MethodId,
                        Formula = o.FormulaName,
                        FormulaId = o.Formula,
                        PointRange = JsonSerializer.Serialize(pointRange)
                    };
                    boardIssues.Add(issue);
                }
            }
            return boardIssues;
        }

        protected void RefreshScoreBoard(ScoreBoard self, User userInfo, SystemCategory cateApplyFor, string classType)
        {
            var selfData = _uow.ScoreBoard.GetIssues(self.ID).Where(o => o.Status && o.ApplyFor == self.ApplyFor).ToList();
            var iQRiskIssues = _uow.RiskIssues.Find(o => o.DomainId == userInfo.DomainId && o.Status && !o.Deleted && o.ApplyFor == cateApplyFor.ID);

            if (!string.IsNullOrEmpty(classType))
            {
                iQRiskIssues = iQRiskIssues.Where(o => o.ClassType.Contains(classType));
            }

            var riskIssues = iQRiskIssues.ToList();

            riskIssues = GetParents(riskIssues);
            var lstIdBigChange = new List<ScoreBoardIssue>();

            selfData.ForEach(o =>
            {
                var obj = riskIssues.FirstOrDefault(a => a.ID == o.IssueId);
                if (obj == null)
                {
                    o.Deleted = true;
                    o.ModifiedBy = userInfo.Id;
                    o.LastModified = DateTime.Now;
                    _uow.ScoreBoard.UpdateIssues(o);
                }
                else
                {
                    //var hasChange = o.Name != obj.Name
                    //                || o.Code != obj.Code
                    //                || o.IssueParentId != obj.ParentId
                    //                || o.Proportion != obj.Proportion
                    //                || o.MethodId != obj.MethodId
                    //                || o.Formula != obj.FormulaName
                    //                || o.FormulaId != obj.Formula;
                    var hasChange = true;
                    if (hasChange)
                    {
                        var riskAssessmentScales = _uow.RiskAssessmentScales.Find(a => a.RiskIssueId == obj.ID && !a.Deleted && a.Status && a.DomainId == userInfo.DomainId).OrderBy(a => a.Point).ToList();
                        var riskAssessmentScale = riskAssessmentScales.FirstOrDefault();
                        if (!o.CreateDateIssue.HasValue)
                        {
                            o.CreateDateIssue = obj.CreateDate;
                        }
                        o.Name = obj.Name;
                        o.Code = obj.Code;
                        o.IssueParentId = obj.ParentId;
                        if (o.ProportionModify == o.Proportion)
                            o.ProportionModify = obj.Proportion;
                        o.Proportion = obj.Proportion;

                        //if (o.MethodId != obj.MethodId || o.FormulaId != obj.Formula || o.Status != obj.Status)
                        //{
                        o.Formula = obj.FormulaName;
                        o.FormulaId = obj.Formula;
                        o.Status = obj.Status;

                        if (o.MethodId != obj.MethodId)
                        {
                            o.MethodId = obj.MethodId;
                            o.RiskValue = null;
                            o.Point = null;

                        }

                        var pointRange = new List<RiskPointRange>();
                        if (o.MethodId == 0)
                        {
                            if (riskAssessmentScale != null)
                            {
                                var val = riskAssessmentScale.MinValue ?? 0;
                                var max = riskAssessmentScale.MaxValue ?? val;
                                while (val <= max)
                                {
                                    pointRange.Add(new RiskPointRange()
                                    {
                                        Point = val,
                                        Condition = GetCondition(riskAssessmentScales, o.MethodId),
                                        Code = riskAssessmentScale.Code,
                                        Name = riskAssessmentScale.Name,
                                        Id = riskAssessmentScale.ID
                                    });
                                    val++;
                                }

                                if (o.RiskValue.HasValue && o.RiskValue.Value > 0)
                                {

                                    var x = riskAssessmentScales.FirstOrDefault(a => a.MinValue <= o.RiskValue && a.MaxValue >= o.RiskValue);
                                    if (x != null)
                                    {
                                        o.Point = x.Point;
                                    }
                                }
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

                        if (!lstIdBigChange.Exists(a => a.ID == o.ID))
                            lstIdBigChange.Add(o);
                        //}

                        o = _uow.ScoreBoard.UpdateIssues(o);
                    }
                }
            });
            var notExists = riskIssues.Where(o => !selfData.Exists(a => a.IssueId == o.ID)).ToList();
            var newIssues = InitBoardIssues(riskIssues, notExists, self.ID, userInfo, cateApplyFor.Code);
            if (newIssues.Any())
                _uow.ScoreBoard.AddIssues(newIssues);

            var allIssues = _uow.ScoreBoard.GetIssues(self.ID).Where(o => o.Status).ToList();

            if (lstIdBigChange.Any())
            {
                var formulas = _uow.Formulas.Find(o => !o.Deleted && o.Status && o.DomainId == userInfo.DomainId).ToList();
                lstIdBigChange.ForEach(item =>
                {
                    allIssues = CalculatorForParent(allIssues, item, userInfo);
                });
            }

            allIssues = allIssues.Where(o => o.IsApply != false).ToList();   
            CalculatorRoot(allIssues, self, userInfo);
            CalculatorForScoreboard(self, userInfo);
        }

        protected List<RiskIssue> GetParents(List<RiskIssue> riskIssues)
        {
            var allIds = riskIssues.Select(o => o.ID).Distinct().ToList();
            var parentIds = riskIssues.Where(o => o.ParentId != null && o.ParentId > 0).Select(o => o.ParentId.Value).Distinct().ToList();
            var anyNot = parentIds.Exists(o => !allIds.Contains(o));
            while (anyNot)
            {
                var not = parentIds.Where(o => !allIds.Contains(o)).Select(o => o).Distinct().ToList();
                if (not.Any())
                {
                    not.ForEach(o =>
                    {
                        var issue = _uow.RiskIssues.FirstOrDefault(a => a.ID == o);
                        if (issue != null)
                            riskIssues.Add(issue);
                    });

                    allIds = riskIssues.Select(o => o.ID).Distinct().ToList();
                    parentIds = riskIssues.Where(o => o.ParentId != null && o.ParentId > 0).Select(o => o.ParentId.Value).Distinct().ToList();
                    anyNot = parentIds.Exists(o => !allIds.Contains(o));
                }
                else
                {
                    anyNot = false;
                }
            }

            return riskIssues;
        }

        /// <summary>
        /// Hàm dùng chung này dùng để tìm độ sâu của node hiện tại so với node gốc mà không
        /// phải sử dụng kiểu dữ liệu cây, output sẽ là số biểu hiện độ sâu, mặc định là 1
        /// </summary>
        /// <param name="parentDic"></param>
        /// <param name="parent"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        protected int DepthLenghtFromRootLevel(Dictionary<int, int?> parentDic, int parent, int depth = 1)
        {
            if (parentDic.ContainsKey(parent))
            {
                if (parentDic[parent] != null)
                {
                    DepthLenghtFromRootLevel(parentDic, (int)parentDic[parent], ++depth);
                }
            }
            return depth;
        }
    }
}
