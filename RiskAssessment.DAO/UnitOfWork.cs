using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiskAssessment.Entity.DbEntities;
using RiskAssessment.Entity;
using Microsoft.EntityFrameworkCore;
using RiskAssessment.DAO.Repositories;
namespace RiskAssessment.DAO
{
    public interface IUnitOfWork : IDisposable
    {
        IBussinessActivitiesRepository BsActRepo { get; }
        ISystemCategoriesRepository SystemCategories { get; }
        IAuditFacilitiesRepository AuditFacilities { get; }
        IAuditCycleRepository AuditCycle { get; }
        IAuditProcessRepository AuditProcess { get; }
        IAuditActivitiesRepository AuditActivities { get; }
        IRatingScalesRepository RatingScales { get; }
        IRiskAssessmentScalesRepository RiskAssessmentScales { get; }
        IRiskIssuesRepository RiskIssues { get; }
        IFormulasRepository Formulas { get; }
        IAssessmentStagesRepository AssessmentStages { get; }
        IScoreBoardRepository ScoreBoard { get; }
        IAssessmentResultsRepository AssessmentResults { get; }
        ITransaction BeginTransaction();
    }
    public class UnitOfWork : IUnitOfWork
    {
        #region init
        private AuditSysContext _context;

        public AuditSysContext Context
        {
            get { return this._context; }
            set { this._context = value; }
        }

        public UnitOfWork(AuditSysContext cn)
        {
            this._context = cn;

            InitializeContext();
        }
        protected void InitializeContext()
        {
            _context.ChangeTracker.LazyLoadingEnabled = true;
            RiskAssessment.Entity.DbInitializer.Initialize(_context);
        }

        #endregion

        #region Repositories
        public IBussinessActivitiesRepository BsActRepo { get { return new BussinessActivitiesRepository(_context); } }
        public ISystemCategoriesRepository SystemCategories { get { return new SystemCategoriesRepository(_context); } }
        public IAuditFacilitiesRepository AuditFacilities { get { return new AuditFacilitiesRepository(_context); } }
        public IAuditCycleRepository AuditCycle { get { return new AuditCycleRepository(_context); } }
        public IAuditProcessRepository AuditProcess { get { return new AuditProcessRepository(_context); } }
        public IAuditActivitiesRepository AuditActivities { get { return new AuditActivitiesRepository(_context); } }
        public IRatingScalesRepository RatingScales { get { return new RatingScalesRepository(_context); } }
        public IRiskAssessmentScalesRepository RiskAssessmentScales { get { return new RiskAssessmentScalesRepository(_context); } }
        public IRiskIssuesRepository RiskIssues { get { return new RiskIssuesRepository(_context); } }
        public IFormulasRepository Formulas { get { return new FormulasRepository(_context); } }
        public IAssessmentStagesRepository AssessmentStages { get { return new AssessmentStagesRepository(_context); } }
        public IScoreBoardRepository ScoreBoard { get { return new ScoreBoardRepository(_context); } }
        public IAssessmentResultsRepository AssessmentResults { get { return new AssessmentResultsRepository(_context); } }
        #endregion
        //transaction
        public ITransaction BeginTransaction()
        {
            return new Transaction(this);
        }
        public void EndTransaction(ITransaction transaction)
        {
            if (transaction != null)
            {
                (transaction as IDisposable).Dispose();
                transaction = null;
            }
        }
        public void Save()
        {
            _context.SaveChanges();
        }

        //Dispposable
        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this._disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
