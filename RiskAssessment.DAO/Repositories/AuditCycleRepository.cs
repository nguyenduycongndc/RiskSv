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
    public interface IAuditCycleRepository : IRepository<AuditCycle>
    {
        IQueryable<AuditCycle> Search(RiskCycleSearch obj, out int count);
        bool HasConfig(AuditCycleDto obj);
    }
    internal class AuditCycleRepository : Repository<AuditCycle>, IAuditCycleRepository
    {
        public AuditCycleRepository(AuditSysContext dbContext)
           : base(dbContext)
        {
        }

        public IQueryable<AuditCycle> Search(RiskCycleSearch obj, out int count)
        {
            obj.Key = (obj.Key + "").Trim().ToUpper();
            obj.Code = (obj.Code + "").Trim().ToUpper();

            var lst = this.DbSet.Where(o => !o.Deleted && o.DomainId == obj.DomainId
            && (obj.Status == -1 || o.Status == (obj.Status == 1))
            && (o.Code.ToUpper().Contains(obj.Code))
            && (o.Name.ToUpper().Contains(obj.Key))
            && (obj.UserCreate == null || o.UserCreate == obj.UserCreate)
            && (obj.RiskLevel == null || o.RatingPoint == obj.RiskLevel)
            )
            .OrderByDescending(o => o.CreateDate);

            count = lst.Count();

            if (obj.StartNumber >= 0 && obj.PageSize > 0)
                return lst.Skip(obj.StartNumber).Take(obj.PageSize);
            return lst;
        }

        public bool HasConfig(AuditCycleDto obj)
        {
            try
            {
                return this.DbSet.Any(p => (!p.Deleted) && (p.RatingPointName == obj.RatingPointName || (p.RatingPointName == obj.RatingPointName && p.Name == obj.Name) || p.Name == obj.Name) && p.ID != obj.ID );
            }
            catch (Exception ex)
            {
                return true;
            }
        }
    }
}
