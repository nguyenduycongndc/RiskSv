using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiskAssessment.Entity.CommonModel;
using RiskAssessment.Entity.DbEntities;

namespace RiskAssessment.DAO.Repositories
{
    public interface IAssessmentStagesRepository : IRepository<AssessmentStage>
    {
        IQueryable<AssessmentStage> Search(AssessmentStageSearch obj, out int count);
    }
    internal class AssessmentStagesRepository : Repository<AssessmentStage>, IAssessmentStagesRepository
    {
        public AssessmentStagesRepository(AuditSysContext dbContext)
           : base(dbContext)
        {
        }
        public IQueryable<AssessmentStage> Search(AssessmentStageSearch obj, out int count)
        {
            obj.Key = (obj.Key + "").Trim().ToUpper();
            obj.Code = (obj.Code + "").Trim().ToUpper();

            var lst = this.DbSet.Where(o => !o.Deleted && o.DomainId == obj.DomainId && o.Status
            && (obj.Year == null || o.Year == obj.Year)
            && (obj.Stage == null || o.Stage == obj.Stage)
            && (obj.StageState == null || (o.StageState ?? 0) == obj.StageState)
            && (obj.StageValue == null || o.StageValue == obj.StageValue)
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
