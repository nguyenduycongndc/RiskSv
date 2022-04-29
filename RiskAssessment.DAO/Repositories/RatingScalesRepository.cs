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
    public interface IRatingScalesRepository : IRepository<RatingScale>
    {
        IQueryable<RatingScale> Search(RiskCycleSearch obj, out int count);
        bool HasConfig(RatingScaleDto obj);
    }
    internal class RatingScalesRepository : Repository<RatingScale>, IRatingScalesRepository
    {
        public RatingScalesRepository(AuditSysContext dbContext)
           : base(dbContext)
        {
        }
        public IQueryable<RatingScale> Search(RiskCycleSearch obj, out int count)
        {
            obj.Key = (obj.Key + "").Trim().ToUpper();
            obj.Code = (obj.Code + "").Trim().ToUpper();

            var lst = this.DbSet.Where(o => !o.Deleted && o.DomainId == obj.DomainId
            && (obj.Status == -1 || o.Status == (obj.Status == 1))
            && (o.Code.ToUpper().Contains(obj.Code))
            && (o.Name.ToUpper().Contains(obj.Key))
            && (obj.UserCreate == null || o.UserCreate == obj.UserCreate)
            && (obj.RiskLevel == null || o.RiskLevel == obj.RiskLevel)
            )
            .OrderByDescending(o => o.CreateDate);

            count = lst.Count();

            if (obj.StartNumber >= 0 && obj.PageSize > 0)
                return lst.Skip(obj.StartNumber).Take(obj.PageSize);
            return lst;
        }
        public bool HasConfig(RatingScaleDto obj)
        {
            var selfVal = obj.Min ?? 0;

            var ratingScales = this.DbSet.Where(o => o.DomainId == obj.DomainId && !o.Deleted && o.ID != obj.ID && o.ApplyFor == obj.ApplyFor).ToList();

            var min = obj.Min ?? 0;
            var max = obj.Max ?? 9999999999;

            foreach (var item in ratingScales)
            {
                if (item.Min >= min && (item.Max ?? item.Min) <= max)
                    return true;

                if (item.Min <= min)
                {
                    if ((item.Max ?? item.Min) > min)
                        return true;
                    if ((item.Max ?? item.Min) == min && item.MaxFunction == "lte" && obj.MinFunction == "gte")
                        return true;
                }
                if ((item.Max ?? item.Min) >= max)
                {
                    if (item.Min < max)
                        return true;
                    if (item.Min == max && item.MinFunction == "gte" && obj.MaxFunction == "lte")
                        return true;
                }

                if ((min > item.Min && min < (item.Max ?? item.Min)) || (max > item.Min && max < (item.Max ?? item.Min)))
                    return true;

            }

            return false;
        }
    }
}
