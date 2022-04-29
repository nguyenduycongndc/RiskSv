using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiskAssessment.Entity.CommonModel;
using RiskAssessment.Entity.DbEntities;

namespace RiskAssessment.DAO.Repositories
{
    public interface IAuditFacilitiesRepository : IRepository<AuditFacility>
    {
        IQueryable<AuditFacility> Search(AuditFacilitySearch obj, out int count);
    }
    internal class AuditFacilitiesRepository : Repository<AuditFacility>, IAuditFacilitiesRepository
    {
        public AuditFacilitiesRepository(AuditSysContext dbContext)
           : base(dbContext)
        {
        }
        public IQueryable<AuditFacility> Search(AuditFacilitySearch obj, out int count)
        {
            obj.Key = (obj.Key + "").Trim().ToUpper();
            obj.Code = (obj.Code + "").Trim().ToUpper();

            var lst = this.DbSet.Where(o => !o.Deleted && o.DomainId == obj.DomainId
            && (obj.Status == -1 || o.Status == (obj.Status == 1))
            && (o.Code.ToUpper().Contains(obj.Code))
            && (o.Name.ToUpper().Contains(obj.Key))
            && (obj.UserCreate == null || o.UserCreate == obj.UserCreate)
            && (obj.ObjectId == 0 || o.ObjectClassId == obj.ObjectId)
            && (obj.ParentId == 0 || o.ParentId == obj.ParentId)
            )
            .OrderBy(o => o.ParentId)
            .OrderByDescending(o => o.CreateDate);

            count = lst.Count();

            if (obj.StartNumber >= 0 && obj.PageSize > 0)
                return lst.Skip(obj.StartNumber).Take(obj.PageSize);
            return lst;
        }
    }
}
