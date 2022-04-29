using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiskAssessment.Entity.CommonModel;
using RiskAssessment.Entity.DbEntities;
using RiskAssessment.Entity.ViewModel;

namespace RiskAssessment.DAO.Repositories
{
    public interface IAssessmentResultsRepository : IRepository<AssessmentResult>
    {
        IQueryable<AssessmentResultView> Search(AssessmentResultSearch obj);

        AssessmentResultView GetResult(AssessmentResultSearch obj);
        List<int> GetIdBoardsForResult(AssessmentResultSearch obj);
        IQueryable<ScopeSearchResultView> SearchScope(ScopeSearch obj, int? type);
        bool CheckBoardCompleteForItem(string applyFor, int objId, int domainId);
    }
    internal class AssessmentResultsRepository : Repository<AssessmentResult>, IAssessmentResultsRepository
    {
        public AssessmentResultsRepository(AuditSysContext dbContext)
           : base(dbContext)
        {
        }

        public IQueryable<AssessmentResultView> Search(AssessmentResultSearch obj)
        {

            obj.Key = (obj.Key + "").Trim().ToUpper();
            var risklevel = obj.RiskLevel.Equals("----Chọn----") ? null : obj.RiskLevel;
            return from board in DbContext.ScoreBoards
                   join result in DbContext.AssessmentResults on board.ID equals result.ScoreBoardId
                   join reason in (from a in DbContext.SystemCategories where a.Status && a.DomainId == obj.DomainId && a.ParentGroup == "LyDoKiemToan" select a) on result.AuditReason equals reason.ID into lrs
                   from rs in lrs.DefaultIfEmpty()
                   join apply in (from a in DbContext.SystemCategories where a.Status && a.DomainId == obj.DomainId && a.ParentGroup == "DoiTuongApDung" select a)
                   on board.ApplyFor equals apply.Code into lApply
                   from oApply in lApply.DefaultIfEmpty()
                   where board.DomainId == obj.DomainId && board.Year == obj.Year && board.Deleted != true
                    && (obj.BoardId == null || board.ID == obj.BoardId)
                    && ((obj.Stage != 3 && board.Stage == obj.Stage) || (obj.Stage == 3 && board.StageValue == obj.StageValue))
                    && (obj.StageState == null || result.StageStatus == obj.StageState)
                    && (obj.ApplyFor == "" || board.ApplyFor == obj.ApplyFor)
                    && (board.ObjectName.ToUpper().Contains(obj.Key) || board.ObjectCode.ToUpper().Contains(obj.Key))
                    && (risklevel == null || board.RiskLevel == risklevel)
                    && (obj.audit == null || (obj.audit == 0 && !result.Audit) || (obj.audit == 1 && result.Audit))
                   //let lastrisklevel = DbContext.ScoreBoards.Where(recentboard => recentboard.DomainId == obj.DomainId && recentboard.Deleted != true && recentboard.ApplyFor == board.ApplyFor && recentboard.ObjectId == board.ObjectId && recentboard.ID != board.ID).OrderByDescending(p => p.Year).ThenBy(p => p.Stage == 1 ? 1 : p.Stage == 3 && p.StageValue == 4 ? 2 : p.Stage == 3 && p.StageValue == 3 ? 3 : p.Stage == 2 ? 4 : p.Stage == 3 && p.StageValue == 2 ? 5 : p.Stage == 3 && p.StageValue == 1 ? 6 : 7).FirstOrDefault()
                   select new AssessmentResultView()
                   {
                       ID = board.ID,
                       ObjectId = board.ObjectId,
                       ObjectCode = board.ObjectCode,
                       ObjectName = board.ObjectName,
                       ApplyFor = oApply == null ? "" : oApply.Name,
                       Point = board.Point,
                       RiskLevel = board.RiskLevel,
                       RiskLevelChange = result.RiskLevelChange,
                       RiskLevelChangeName = result.RiskLevelChangeName,
                       LastRiskLevel = result.LastRiskLevel,
                       LastAudit = result.LastAudit == null ? "" : result.LastAudit.Value.ToString("MM/yyyy"),
                       AuditCycle = board.AuditCycleName,
                       Audit = result.Audit,
                       AuditReasonId = result.AuditReason,
                       AuditReason = rs == null ? "" : rs.Name,
                       StageStatus = result.StageStatus,
                       AssessmentStatus = result.AssessmentStatus,
                       ApplyForId = board.ApplyFor
                   };
        }
        public IQueryable<ScopeSearchResultView> SearchScope(ScopeSearch obj, int? type)
        {
            var keyprocess = obj.keyprocess;
            if (type == 2 || type == 3)
            {
                keyprocess = "";
            }
            obj.keyprocess = (obj.keyprocess + "").Trim().ToUpper();
            obj.keyfacility = (obj.keyfacility + "").Trim().ToUpper();
            obj.keyactive = (obj.keyactive + "").Trim().ToUpper();
            return from board in DbContext.ScoreBoards
                   join result in DbContext.AssessmentResults on board.ID equals result.ScoreBoardId
                   join reason in (from a in DbContext.SystemCategories where a.Status && a.DomainId == obj.DomainId && a.ParentGroup == "LyDoKiemToan" select a) on result.AuditReason equals reason.ID into lrs
                   from rs in lrs.DefaultIfEmpty()
                   join apply in (from a in DbContext.SystemCategories where a.Status && a.DomainId == obj.DomainId && a.ParentGroup == "DoiTuongApDung" select a)
                   on board.ApplyFor equals apply.Code into lApply
                   from oApply in lApply.DefaultIfEmpty()
                   where board.DomainId == obj.DomainId && board.Year == obj.Year && board.Deleted != true && result.Audit == true
                    && ((obj.Stage != 3 && board.Stage == obj.Stage) || (obj.Stage == 3 && board.StageValue == obj.StageValue))
                    && (obj.StageState == null || result.StageStatus == obj.StageState)
                    && (obj.ApplyFor == "" || board.ApplyFor == obj.ApplyFor)
                    && (keyprocess == "" || board.ObjectName.ToUpper().Contains(keyprocess) || board.ObjectCode.ToUpper().Contains(keyprocess))
                    && (obj.keyfacility == "" || board.ObjectName.ToUpper().Contains(obj.keyfacility) || board.ObjectCode.ToUpper().Contains(obj.keyfacility))
                    && (obj.keyactive == "" || board.ObjectName.ToUpper().Contains(obj.keyactive) || board.ObjectCode.ToUpper().Contains(obj.keyactive))
                   select new ScopeSearchResultView()
                   {
                       ID = board.ID,
                       ObjectCode = board.ObjectCode,
                       ObjectName = board.ObjectName,
                       ObjectId = board.ObjectId,
                       ApplyFor = oApply == null ? "" : oApply.Name,
                       Point = board.Point,
                       RiskLevel = board.RiskLevel,
                       RiskLevelChange = result.RiskLevelChange,
                       RiskLevelChangeName = result.RiskLevelChangeName,
                       IsLevelChange = result.RiskLevel != result.RiskLevelChange,
                       LastRiskLevel = result.LastRiskLevel,
                       LastAudit = result.LastAudit == null ? "" : result.LastAudit.Value.ToString("MM/yyyy"),
                       AuditCycle = board.AuditCycleName,
                       Audit = result.Audit,
                       AuditReasonId = result.AuditReason,
                       AuditReason = rs == null ? "" : rs.Name,
                       StageStatus = result.StageStatus,
                       AssessmentStatus = result.AssessmentStatus
                   };
        }

        public AssessmentResultView GetResult(AssessmentResultSearch obj)
        {
            var lst = from board in DbContext.ScoreBoards
                      join result in DbContext.AssessmentResults on board.ID equals result.ScoreBoardId
                      join reason in (from a in DbContext.SystemCategories where a.Status && a.DomainId == obj.DomainId && a.ParentGroup == "LyDoKiemToan" select a) on result.AuditReason equals reason.ID into lrs
                      from rs in lrs.DefaultIfEmpty()
                      join apply in (from a in DbContext.SystemCategories where a.Status && a.DomainId == obj.DomainId && a.ParentGroup == "DoiTuongApDung" select a)
                      on board.ApplyFor equals apply.Code into lApply
                      from oApply in lApply.DefaultIfEmpty()
                      where board.DomainId == obj.DomainId && board.ID == obj.BoardId && board.Deleted != true
                      select new AssessmentResultView()
                      {
                          ID = board.ID,
                          ObjectCode = board.ObjectCode,
                          ObjectName = board.ObjectName,
                          ApplyFor = oApply == null ? "" : oApply.Name,
                          Point = board.Point,
                          RiskLevel = board.RiskLevel,
                          RiskLevelChange = result.RiskLevelChange,
                          LastRiskLevel = result.LastRiskLevel,
                          LastAudit = result.LastAudit == null ? "" : result.LastAudit.Value.ToString("MM/yyyy"),
                          AuditCycle = "",
                          Audit = result.Audit,
                          AuditReasonId = result.AuditReason,
                          AuditReason = rs == null ? "" : rs.Name,
                          StageStatus = result.StageStatus,
                          Description = result.Description,
                          PassAuditReason = result.PassAuditReason
                      };

            return lst.FirstOrDefault();
        }
        public List<int> GetIdBoardsForResult(AssessmentResultSearch obj)
        {
            return (from board in DbContext.ScoreBoards
                    join result in DbContext.AssessmentResults on board.ID equals result.ScoreBoardId
                    where board.DomainId == obj.DomainId && board.Year == obj.Year && !board.Deleted
                     && (obj.BoardId == null || board.ID == obj.BoardId)
                     && ((obj.Stage != 3 && board.Stage == obj.Stage) || (obj.Stage == 3 && board.StageValue == obj.StageValue))

                    select board.ID).ToList();
        }
        public bool CheckBoardCompleteForItem(string applyFor, int objId, int domainId)
        {
            var lst = from board in DbContext.ScoreBoards
                      join result in DbContext.AssessmentResults on board.ID equals result.ScoreBoardId

                      where board.DomainId == domainId && !board.Deleted && !result.Deleted //&& result.StageStatus == 1
                      && board.ApplyFor == applyFor && board.ObjectId == objId
                      select board;

            return lst.Any();
        }
    }
}
