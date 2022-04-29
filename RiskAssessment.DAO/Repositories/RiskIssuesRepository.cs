using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiskAssessment.Entity.CommonModel;
using RiskAssessment.Entity.DbEntities;

namespace RiskAssessment.DAO.Repositories
{
    public interface IRiskIssuesRepository : IRepository<RiskIssue>
    {
        IQueryable<RiskIssue> Search(RiskIssueSearch obj, out int count);
        bool CheckScoreIssue(int issueid);
    }
    internal class RiskIssuesRepository : Repository<RiskIssue>, IRiskIssuesRepository
    {
        public RiskIssuesRepository(AuditSysContext dbContext)
           : base(dbContext)
        {
        }
        public IQueryable<RiskIssue> Search(RiskIssueSearch obj, out int count)
        {
            obj.Key = (obj.Key + "").Replace(" ", "").Trim().ToUpper();
            obj.Code = (obj.Code + "").Replace(" ", "").Trim().ToUpper();
            obj.ClassType = (obj.ClassType + "").Trim().ToUpper();
            var arClassType = new List<string>();
            if (!string.IsNullOrEmpty(obj.ClassType))
            {
                arClassType = obj.ClassType.Split(';').Where(a => !string.IsNullOrEmpty(a)).Select(o => o).ToList();
            }

            IQueryable<RiskIssue> lst = null;

            if (obj.ClassType == "" || arClassType.Count == 0)
                lst = this.DbSet.Where(o => !o.Deleted && o.DomainId == obj.DomainId
                    && (obj.Status == -1 || o.Status == (obj.Status == 1))
                    && (o.Code.Replace(" ", "").ToUpper().Contains(obj.Code))
                    && (o.Name.Replace(" ", "").ToUpper().Contains(obj.Key))
                    && (obj.UserCreate == null || o.UserCreate == obj.UserCreate)
                    && (obj.ApplyFor == null || o.ApplyFor == obj.ApplyFor)
                    && (obj.MethodId == null || o.MethodId == obj.MethodId)
                    )
                    .OrderByDescending(o => o.CreateDate);
            else
            {
                var fir = arClassType[0];
                lst = this.DbSet.Where(o => !o.Deleted && o.DomainId == obj.DomainId
                    && (obj.Status == -1 || o.Status == (obj.Status == 1))
                    && (o.Code.ToUpper().Contains(obj.Code))
                    && (o.Name.ToUpper().Contains(obj.Key))
                    && (obj.UserCreate == null || o.UserCreate == obj.UserCreate)
                    && (obj.ApplyFor == null || o.ApplyFor == obj.ApplyFor)
                    && o.ClassType.Contains(fir)
                    && (obj.MethodId == null || o.MethodId == obj.MethodId)
                    );

                for (int i = 1; i < arClassType.Count; i++)
                {
                    var val = arClassType[i];
                    var subLst = this.DbSet.Where(o => !o.Deleted && o.DomainId == obj.DomainId
                        && (obj.Status == -1 || o.Status == (obj.Status == 1))
                        && (o.Code.ToUpper().Contains(obj.Code))
                        && (o.Name.ToUpper().Contains(obj.Key))
                        && (obj.UserCreate == null || o.UserCreate == obj.UserCreate)
                        && (obj.ApplyFor == null || o.ApplyFor == obj.ApplyFor)
                        && o.ClassType.Contains(val)
                        && (obj.MethodId == null || o.MethodId == obj.MethodId)
                        );

                    lst = lst.Union(subLst);
                }

            }

            lst = lst.OrderByDescending(o => o.CreateDate);

            count = lst.Count();

            if (obj.StartNumber >= 0 && obj.PageSize > 0)
                return lst.Skip(obj.StartNumber).Take(obj.PageSize);
            return lst;
        }


        public bool CheckScoreIssue(int issueid)
        {
            return DbContext.ScoreBoardIssues.Any(p => p.IssueId == issueid);
        }
    }
}
