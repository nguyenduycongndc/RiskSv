namespace RiskAssessment.DAO.Repositories
{
using RiskAssessment.Entity.DbEntities;
    public interface IFormulasRepository : IRepository<Formula>
    {
    }
    internal class FormulasRepository : Repository<Formula>, IFormulasRepository
    {
        public FormulasRepository(AuditSysContext dbContext)
           : base(dbContext)
        {
        }
    }
}
