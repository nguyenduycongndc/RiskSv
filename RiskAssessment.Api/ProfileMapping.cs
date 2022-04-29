using AutoMapper;
using RiskAssessment.Entity.DbEntities;
using RiskAssessment.Entity.DTO;
using RiskAssessment.Entity.ViewModel;
using System.Collections.Generic;
using System.Text.Json;

namespace RiskAssessment.Api
{
    public class ProfileMapping
        : Profile
    {
        public ProfileMapping()
        {
            //AuditActivity
            CreateMap<AuditActivity, AuditActivityDto>();
            CreateMap<AuditActivityDto, AuditActivity>();
            //AuditCycle
            CreateMap<AuditCycle, AuditCycleDto>();
            CreateMap<AuditCycleDto, AuditCycle>();
            //AuditFacility
            CreateMap<AuditFacility, AuditFacilityDto>()
                .ForMember(d => d.Parent, s => s.MapFrom(o => o.ParentId == null ? null : new AuditFacilityDto() { ID = o.ParentId.Value }));
            CreateMap<AuditFacilityDto, AuditFacility>()
                .ForMember(d => d.ParentId, opt => opt.MapFrom(c => c.Parent == null ? (int?)null : c.Parent.ID));
            //AuditProcess
            CreateMap<AuditProcess, AuditProcessDto>();
            CreateMap<AuditProcessDto, AuditProcess>();
            //BussinessActivity
            CreateMap<BussinessActivity, BussinessActivityDto>()
                .ForMember(d => d.Parent, opt => opt.MapFrom(c => c.ParentId == null || c.ParentId == 0 ? null : new BussinessActivityDto() { ID = c.ParentId.Value }));
            CreateMap<BussinessActivityDto, BussinessActivity>()
                .ForMember(d => d.ParentId, opt => opt.MapFrom(c => c.Parent == null || c.Parent.ID == 0 ? (int?)null : c.Parent.ID));
            //RatingScale
            CreateMap<RatingScale, RatingScaleDto>();
            CreateMap<RatingScaleDto, RatingScale>();
            //Formula
            CreateMap<Formula, FormulaDto>();
            CreateMap<FormulaDto, Formula>();
            //RiskAssessmentScale
            CreateMap<RiskAssessmentScale, RiskAssessmentScaleDto>()
                .ForMember(d => d.RiskIssue, opt => opt.MapFrom(s => s.RiskIssueId == null || s.RiskIssueId == 0 ? null
                : new RiskIssueDto() { ID = s.RiskIssueId.Value, Name = s.RiskIssueName, Code = s.RiskIssueCode, MethodId = s.RiskIssueCodeMethod ?? 0 }))
                .ForMember(d => d.MinCondition, opt => opt.MapFrom(s => s.LowerCondition == null || s.LowerCondition == 0 ? null
                : new SystemCategoryDto() { ID = s.LowerCondition.Value, Name = s.LowerConditionName }))
                .ForMember(d => d.MaxCondition, opt => opt.MapFrom(s => s.UpperCondition == null || s.UpperCondition == 0 ? null
                : new SystemCategoryDto() { ID = s.UpperCondition.Value, Name = s.UpperConditionName }));
            CreateMap<RiskAssessmentScaleDto, RiskAssessmentScale>()
                .ForMember(d => d.RiskIssueId, opt => opt.MapFrom(s => s.RiskIssue == null || s.RiskIssue.ID == 0 ? (int?)null : s.RiskIssue.ID))
                .ForMember(d => d.RiskIssueCode, opt => opt.MapFrom(s => s.RiskIssue == null || s.RiskIssue.ID == 0 ? "" : s.RiskIssue.Code))
                .ForMember(d => d.RiskIssueName, opt => opt.MapFrom(s => s.RiskIssue == null || s.RiskIssue.ID == 0 ? "" : s.RiskIssue.Name))
                .ForMember(d => d.RiskIssueCodeMethod, opt => opt.MapFrom(s => s.RiskIssue == null || s.RiskIssue.ID == 0 ? (int?)null : s.RiskIssue.MethodId))
                .ForMember(d => d.LowerCondition, opt => opt.MapFrom(s => s.MinCondition == null || s.MinCondition.ID == 0 ? (int?)null
                    : s.MinCondition.ID))
                .ForMember(d => d.UpperCondition, opt => opt.MapFrom(s => s.MaxCondition == null || s.MaxCondition.ID == 0 ? (int?)null
                    : s.MaxCondition.ID))
                .ForMember(d => d.LowerConditionName, opt => opt.MapFrom(s => s.MinCondition == null || s.MinCondition.ID == 0 ? ""
                    : s.MinCondition.Name))
                .ForMember(d => d.UpperConditionName, opt => opt.MapFrom(s => s.MaxCondition == null || s.MaxCondition.ID == 0 ? ""
                    : s.MaxCondition.Name));
            //RiskIssue
            CreateMap<RiskIssue, RiskIssueDto>()
                .ForMember(d => d.Parent, opt => opt.MapFrom(c => c.ParentId == null || c.ParentId == 0 ? null
                : new RiskIssueDto() { ID = c.ParentId.Value }))
                .ForMember(d => d.ApplyFor, opt => opt.MapFrom(c => c.ApplyFor == null || c.ApplyFor == 0 ? null
                : new SystemCategoryDto() { ID = c.ApplyFor.Value, Name = c.ApplyForName }))
                //.ForMember(d => d.ClassType, opt => opt.MapFrom(c => c.ClassType == null || c.ClassType == 0 ? null
                //: new UnitTypeView() { id = c.ClassType.Value, name = c.ClassTypeName }))
                .ForMember(d => d.Formula, opt => opt.MapFrom(c => c.Formula == null || c.Formula == 0 ? null
                : new FormulaDto() { ID = c.Formula.Value, Name = c.FormulaName }));
            CreateMap<RiskIssueDto, RiskIssue>()
                .ForMember(d => d.ParentId, opt => opt.MapFrom(c => c.Parent == null ? (int?)null : c.Parent.ID))
                .ForMember(d => d.ApplyFor, opt => opt.MapFrom(c => c.ApplyFor == null ? (int?)null : c.ApplyFor.ID))
                //.ForMember(d => d.ClassType, opt => opt.MapFrom(c => c.ClassType == null ? (int?)null : c.ClassType.id))
                .ForMember(d => d.Formula, opt => opt.MapFrom(c => c.Formula == null ? (int?)null : c.Formula.ID))

                .ForMember(d => d.ApplyForName, opt => opt.MapFrom(c => c.ApplyFor == null ? null : c.ApplyFor.Name))
                //.ForMember(d => d.ClassTypeName, opt => opt.MapFrom(c => c.ClassType == null ? null : c.ClassType.name))
                .ForMember(d => d.FormulaName, opt => opt.MapFrom(c => c.Formula == null ? null : c.Formula.Name));
            //SystemCategory
            CreateMap<SystemCategory, SystemCategoryDto>();
            CreateMap<SystemCategoryDto, SystemCategory>();
            //AssessmentStage
            CreateMap<AssessmentStage, AssessmentStageDto>();
            CreateMap<AssessmentStageDto, AssessmentStage>();
            //ScoreBoard
            CreateMap<ScoreBoard, ScoreBoardDto>();
            CreateMap<ScoreBoardDto, ScoreBoard>();
            //ScoreBoardIssue
            CreateMap<ScoreBoardIssue, ScoreBoardIssueDto>();
            CreateMap<ScoreBoardIssueDto, ScoreBoardIssue>();
            //AssessmentResult
            CreateMap<AssessmentResult, AssessmentResultDto>();
            CreateMap<AssessmentResultDto, AssessmentResult>();
            //Auditworkscope
            CreateMap<AuditWorkScope, AuditWorkScopeDto>();
            CreateMap<AuditWorkScopeDto, AuditWorkScope>();
            //Reportauditwork
            CreateMap<ReportAuditWork, ReportAuditWorkDto>();
            CreateMap<ReportAuditWorkDto, ReportAuditWork>();
            //Reportauditwork
            CreateMap<AuditWork, AuditWorkDto>();
            CreateMap<AuditWorkDto, AuditWork>();
            //ScoreBoardFile
            CreateMap<ScoreBoardFile, ScoreBoardDto>();
            CreateMap<ScoreBoardDto, ScoreBoardFile>();
        }

    }
}
