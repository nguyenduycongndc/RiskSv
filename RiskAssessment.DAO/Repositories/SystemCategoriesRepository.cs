using System.Linq;
using RiskAssessment.Entity.DbEntities;
using RiskAssessment.Entity.ViewModel;

namespace RiskAssessment.DAO.Repositories
{
    public interface ISystemCategoriesRepository : IRepository<SystemCategory>
    {
        IQueryable<CatRiskLevel> GetRiskLevelCategory();
        IQueryable<UserView> GetUsersActive();
        IQueryable<UnitTypeView> GetUnitTypes();
        string GetUserFullName(int id);
    }
    internal class SystemCategoriesRepository : Repository<SystemCategory>, ISystemCategoriesRepository
    {
        public SystemCategoriesRepository(AuditSysContext dbContext)
           : base(dbContext)
        {
        }
        public IQueryable<CatRiskLevel> GetRiskLevelCategory()
        {
            return DbContext.Set<CatRiskLevel>().Where(o => o.Status != null && o.Status == true && o.IsDeleted == false);
        }
        public IQueryable<UserView> GetUsersActive()
        {
            return from u in DbContext.Users
                   join ds in DbContext.AuditFacilities on u.department_id equals ds.ID into ld
                   from d in ld.DefaultIfEmpty()
                   where u.is_active != null && u.is_active.Value
                   && u.is_deleted != null && !u.is_deleted.Value
                   select new UserView()
                   {
                       full_name = u.full_name,
                       email = u.email,
                       department_name = d == null ? "" : d.Name
                   };
        }
        public string GetUserFullName(int id)
        {
            var obj = DbContext.Users.FirstOrDefault(o => o.id == id);
            return obj == null ? "" : obj.full_name;
        }
        public IQueryable<UnitTypeView> GetUnitTypes()
        {
            return from u in DbContext.UnitTypes
                   where u.is_deleted == false && u.status == true
                   select new UnitTypeView()
                   {
                       id = u.id,
                       name = u.name
                   };
        }
    }
}
