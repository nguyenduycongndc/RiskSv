using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiskAssessment.Entity.DbEntities;

namespace RiskAssessment.DAO.Repositories
{
    public interface IBussinessActivitiesRepository : IRepository<BussinessActivity>
    {
    }
    internal class BussinessActivitiesRepository : Repository<BussinessActivity>, IBussinessActivitiesRepository
    {
        public BussinessActivitiesRepository(AuditSysContext dbContext)
           : base(dbContext)
        {
        }
    }
}
