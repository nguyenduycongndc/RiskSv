using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiskAssessment.Entity.DbEntities;
using RiskAssessment.Entity.CommonModel;

namespace RiskAssessment.DAO.Repositories
{
    public interface IAuditProcessRepository : IRepository<AuditProcess>
    {
        IQueryable<AuditProcess> Search(AuditProcessSearch obj, out int count);
    }
    internal class AuditProcessRepository : Repository<AuditProcess>, IAuditProcessRepository
    {
        public AuditProcessRepository(AuditSysContext dbContext)
           : base(dbContext)
        {
        }
        public IQueryable<AuditProcess> Search(AuditProcessSearch obj, out int count)
        {
            obj.Key = (obj.Key + "").Trim().ToUpper();
            obj.Code = (obj.Code + "").Trim().ToUpper();

            var lst = this.DbSet.Where(o => !o.Deleted && o.DomainId == obj.DomainId
            && (obj.Status == -1 || o.Status == (obj.Status == 1))
            && (o.Code.ToUpper().Contains(obj.Code))
            && (o.Name.ToUpper().Contains(obj.Key))
            && (obj.UserCreate == null || o.UserCreate == obj.UserCreate)
            && (obj.FacilityId == 0 || o.FacilityId == obj.FacilityId)
            && (obj.ActivityId == 0 || o.ActivityId == obj.ActivityId)
            );

            if (obj.SortBy == "Activity")
            {
                if (obj.SortType == "asc")
                    lst = lst.OrderBy(o => o.ActivityName);
                else
                    lst = lst.OrderByDescending(o => o.ActivityName);
            }
            else if (obj.SortBy == "Facility")
            {
                if (obj.SortType == "asc")
                    lst = lst.OrderBy(o => o.FacilityName);
                else
                    lst = lst.OrderByDescending(o => o.FacilityName);
            }
            else if (obj.SortBy == "Code")
            {
                if (obj.SortType == "asc")
                    lst = lst.OrderBy(o => o.Code);
                else
                    lst = lst.OrderByDescending(o => o.Code);
            }
            else if (obj.SortBy == "Name")
            {
                if (obj.SortType == "asc")
                    lst = lst.OrderBy(o => o.Name);
                else
                    lst = lst.OrderByDescending(o => o.Name);
            }
            else lst = lst.OrderByDescending(o => o.ID);


            count = lst.Count();

            if (obj.StartNumber >= 0 && obj.PageSize > 0)
                return lst.Skip(obj.StartNumber).Take(obj.PageSize);
            return lst;
        }
    }
}
