using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiskAssessment.Entity.DbEntities;

namespace RiskAssessment.Entity
{
    public static class DbInitializer
    {
        public static void Initialize(AuditSysContext context)
        {
            context.Database.EnsureCreated();
            context.SaveChanges();
        }
    }
}
