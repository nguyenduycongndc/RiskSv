namespace RiskAssessment.DAO.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using RiskAssessment.Entity.DbEntities;
    using RiskAssessment.Entity.ViewModel;

    public interface IScoreBoardRepository : IRepository<ScoreBoard>
    {
        IQueryable<ScoreBoardIssue> GetIssues(int board);
        List<ScoreBoardIssue> AddIssues(List<ScoreBoardIssue> issues);
        ScoreBoardIssue UpdateIssues(ScoreBoardIssue issue);
        IQueryable<ScoreBoardIssue> IssueUpdateIncomplete(int issueId, int domainId);
        IQueryable<ScoreBoard> GetByApply(int domainId, string applyFor);
        IQueryable<ProcessResultView> GetprocessBoard();
        IQueryable<RecentScoreBoard> GetRecentScoreBoard(int ScoreBoardId, int ObjectId, int DomainId, string ApplyForId, int Year, int Take);
        List<RecentAudit> GetRecentAudit(string ApplyForId, int ObjectId, int year, int Take);
        List<ScoreBoardFile> GetFile(int scoreBoardId);
        ScoreBoardFile GetFileById(int id);

        void UpdateFileStatus();
        bool CheckRiskIssue(string code);
    }
    class ScoreBoardRepository : Repository<ScoreBoard>, IScoreBoardRepository
    {
        public ScoreBoardRepository(AuditSysContext dbContext)
           : base(dbContext)
        {
        }

        public IQueryable<ScoreBoardIssue> GetIssues(int board)
        {
            return DbContext.Set<ScoreBoardIssue>().Where(o => !o.Deleted && o.ScoreBoardId == board);
        }
        public IQueryable<ScoreBoardIssue> IssueUpdateIncomplete(int issueId, int domainId)
        {
            return from i in DbContext.ScoreBoardIssues
                   join b in DbContext.ScoreBoards on i.ScoreBoardId equals b.ID
                   join a in DbContext.AssessmentResults on b.ID equals a.ScoreBoardId
                   join s in DbContext.AssessmentStages on b.AssessmentStageId equals s.ID
                   where !b.Deleted && !i.Deleted && !a.Deleted && !s.Deleted
                   && b.DomainId == domainId && s.DomainId == domainId
                   && i.IssueId == issueId && b.CurrentStatus != 1 && a.StageStatus != 1 && s.StageState != 1
                   select i;
        }
        public IQueryable<ScoreBoard> GetByApply(int domainId, string applyFor)
        {
            return from b in DbContext.ScoreBoards
                   join a in DbContext.AssessmentResults on b.ID equals a.ScoreBoardId
                   join s in DbContext.AssessmentStages on b.AssessmentStageId equals s.ID
                   where !b.Deleted && !a.Deleted && !s.Deleted
                   && b.DomainId == domainId && s.DomainId == domainId
                   && b.ApplyFor == applyFor && b.CurrentStatus != 1 && a.StageStatus != 1 && s.StageState != 1
                   select b;
        }
        public List<ScoreBoardIssue> AddIssues(List<ScoreBoardIssue> issues)
        {
            foreach (var entity in issues)
            {
                this.DbContext.Entry(entity).State = EntityState.Added;
            }
            Save();
            return issues;
        }
        public ScoreBoardIssue UpdateIssues(ScoreBoardIssue issue)
        {
            this.DbContext.Entry(issue).State = EntityState.Modified;
            Save();
            return issue;
        }
        public IQueryable<ProcessResultView> GetprocessBoard()
        {
            return from i in DbContext.AuditProcess
                   join b in DbContext.ScoreBoards on i.ID equals b.ObjectId
                   where !b.Deleted && !i.Deleted
                   select new ProcessResultView()
                   {
                       ID = b.ID,
                       ProcessCode = i.Code,
                       ProcessName = i.Name,
                       ProcessId = i.ID,
                       ActivityId = i.ActivityId,
                       ActivityName = i.ActivityName,
                       FacilityId = i.FacilityId,
                       FacilityName = i.FacilityName,
                   };
        }

        public IQueryable<RecentScoreBoard> GetRecentScoreBoard(int ScoreBoardId, int ObjectId, int DomainId, string ApplyForId, int Year, int Take)
        {
            var lastboard = (from a in DbContext.ScoreBoards
                             join b in DbContext.AssessmentResults on a.ID equals b.ScoreBoardId
                             where /*a.Status && a.CurrentStatus ==1 &&*/ a.DomainId == DomainId && a.Deleted != true && a.ApplyFor == ApplyForId && a.ObjectId == ObjectId && a.ID != ScoreBoardId && a.Year <= Year
                             select new RecentScoreBoard
                             {
                                 assessment_risklevel = string.IsNullOrEmpty(b.RiskLevelChangeName) ? a.RiskLevel : b.RiskLevelChangeName,
                                 Year = a.Year,
                                 Stage = a.Stage,
                                 StageValue = a.StageValue,
                             })?.OrderByDescending(p => p.Year)?.ThenBy(p => p.Stage == 1 ? 1 : p.Stage == 3 && p.StageValue == 4 ? 2 : p.Stage == 3 && p.StageValue == 3 ? 3 : p.Stage == 2 ? 4 : p.Stage == 3 && p.StageValue == 2 ? 5 : p.Stage == 3 && p.StageValue == 1 ? 6 : 7).Take(Take);
            return lastboard;
        }

        public List<RecentAudit> GetRecentAudit(string ApplyForId, int ObjectId, int year, int take)
        {
            var recentaudit = (from a in DbContext.AuditWorkScopes
                               join b in DbContext.ReportAuditWorks on a.auditwork_id equals b.auditwork_id
                               join c in DbContext.ApproveFunction.Where(p => p.FunctionCode == "M_RAW") on b.id equals c.ItemId
                               where c.StatusCode == "3.1" && (ApplyForId == "QT" ? a.auditprocess_id == ObjectId : ApplyForId == "DV" ? a.auditfacilities_id == ObjectId : a.bussinessactivities_id == ObjectId) && a.year <= year && a.is_deleted == false && b.is_deleted == false
                               select new
                               {
                                   auditid = b.auditwork_id,
                                   auditcode = b.auditwork_code,
                                   endDateField = b.end_date_field,
                                   year = b.year,
                                   facility = a.auditfacilities_id,
                               }).AsEnumerable().GroupBy(p => p.year).OrderByDescending(p => p.Key).Select(p => new RecentAudit
                               {
                                   auditid = p.OrderByDescending(x => x.endDateField).FirstOrDefault().auditid,
                                   year = p.OrderByDescending(x => x.endDateField).FirstOrDefault().year ?? "",
                                   audit_code = p.OrderByDescending(x => x.endDateField).FirstOrDefault().auditcode ?? "",
                                   Time = p.OrderByDescending(x => x.endDateField).FirstOrDefault().endDateField.HasValue ? p.OrderByDescending(x => x.endDateField).FirstOrDefault().endDateField.Value.ToString("MM/yyyy") : "",
                                   facility_id = p.OrderBy(x => x.endDateField).FirstOrDefault().facility,
                               }).Take(take)?.ToList() ?? new List<RecentAudit>();
            recentaudit.ForEach(item =>
            {
                var lastrating = DbContext.AuditWorkScopes.FirstOrDefault(p => p.auditfacilities_id == item.facility_id && p.auditwork_id == item.auditid && p.audit_rating_level_report.HasValue)?.audit_rating_level_report ?? 0;
                item.audit_rating_level_report = lastrating;
            });
            return recentaudit;
        }

        public List<ScoreBoardFile> GetFile(int scoreBoardId)
        {
            var files = DbContext.ScoreBoardFile.Where(x => x.ScoreBoardId == scoreBoardId && !(x.IsDelete ?? false));
            return files.ToList();
        }

        public ScoreBoardFile GetFileById(int id)
        {
            var file = DbContext.ScoreBoardFile.FirstOrDefault(x => x.ID == id);
            return file;
        }

        public void UpdateFileStatus()
        {
            DbContext.SaveChanges();
        }

        public bool CheckRiskIssue(string code)
        {
            var check = DbContext.ScoreBoardIssues.Any(p => p.Code == code);
            return check;
        }
    }
}
