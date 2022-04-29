using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiskAssessment.Entity.DbEntities;

namespace RiskAssessment.DAO.Repositories
{
    public interface IAuditActivitiesRepository : IRepository<AuditActivity>
    {
    }
    internal class AuditActivitiesRepository : Repository<AuditActivity>, IAuditActivitiesRepository
    {
        public AuditActivitiesRepository(AuditSysContext dbContext)
           : base(dbContext)
        {
        }
    }
}
