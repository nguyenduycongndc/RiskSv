using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiskAssessment.Entity.CommonModel;
using RiskAssessment.Entity.DbEntities;
using RiskAssessment.Entity.DTO;

namespace RiskAssessment.DAO.Repositories
{
    public interface IRiskAssessmentScalesRepository : IRepository<RiskAssessmentScale>
    {
        IQueryable<RiskAssessmentScale> Search(RiskIssueSearch obj, out int count);
        bool HasConfig(RiskAssessmentScaleDto dto);
    }
    internal class RiskAssessmentScalesRepository : Repository<RiskAssessmentScale>, IRiskAssessmentScalesRepository
    {
        public RiskAssessmentScalesRepository(AuditSysContext dbContext)
           : base(dbContext)
        {
        }

        public bool HasConfig(RiskAssessmentScaleDto dto)
        {
            var byIssue = DbSet.Where(o => o.DomainId == dto.DomainId && !o.Deleted && o.RiskIssueId == dto.RiskIssue.ID && o.ID != dto.ID).ToList();

            if (dto.RiskIssue.MethodId == 1)
                return byIssue.Any(o => o.Point == dto.Point);

            var min = dto.MinValue ?? 0;
            var max = dto.MaxValue ?? 0;

            foreach (var item in byIssue)
            {
                if (item.MinValue >= min && item.MaxValue <= max)
                    return true;

                if (item.MinValue <= min)
                {
                    if (item.MaxValue > min)
                        return true;
                    if (item.MaxValue == min && item.UpperConditionName == "<=" && dto.MinCondition.Name == ">=")
                        return true;
                }
                if (item.MaxValue >= max)
                {
                    if (item.MinValue < max)
                        return true;
                    if (item.MinValue == max && item.LowerConditionName == ">=" && dto.MaxCondition.Name == "<=")
                        return true;
                }

                if ((min > item.MinValue && min < item.MaxValue) || (max > item.MinValue && max < item.MaxValue))
                    return true;
            }

            return false;
        }
        public IQueryable<RiskAssessmentScale> Search(RiskIssueSearch obj, out int count)
        {
            obj.Key = (obj.Key + "").Trim().ToUpper();
            obj.Code = (obj.Code + "").Trim().ToUpper();

            var lst = this.DbSet.Where(o => !o.Deleted && o.DomainId == obj.DomainId
            && (obj.Status == -1 || o.Status == (obj.Status == 1))
            && (obj.MethodId == -1 || o.RiskIssueCodeMethod == obj.MethodId)
            && (obj.Point == 0 || o.Point == obj.Point)
            //&& (o.RiskIssueCode.ToUpper().Contains(obj.Code))
            && (o.RiskIssueName.ToUpper().Contains(obj.Key) || o.RiskIssueCode.ToUpper().Contains(obj.Key))
            && (obj.UserCreate == null || o.UserCreate == obj.UserCreate)
            )
            .OrderByDescending(o => o.CreateDate);

            count = lst.Count();

            if (obj.StartNumber >= 0 && obj.PageSize > 0)
                return lst.Skip(obj.StartNumber).Take(obj.PageSize);
            return lst;
        }
    }
}
