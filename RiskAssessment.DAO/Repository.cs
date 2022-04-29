using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RiskAssessment.Entity.CommonModel;
using RiskAssessment.Entity.DbEntities;


namespace RiskAssessment.DAO
{
    public interface IRepository<T> where T : BaseEntity
    {
        bool CheckPermission(T t, int domainId);
        IQueryable<T> GetAll();
        bool CheckValidCode(string code, int id, int domainId);
        IQueryable<T> Find(Expression<Func<T, bool>> predicate);
        IQueryable<T> Search(ModelSearch obj, out int count);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
        void Delete(T entity);

        T Add(T entity);

        void AddWithoutSave(T entity);
        T Update(T entity);

        int Save();
    }
    class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected AuditSysContext DbContext;
        protected DbSet<T> DbSet;

        public Repository(DbContext dbContext)
        {
            if (dbContext == null)
            {
                throw new Exception("EFRepository::initialize::dbContext::Canot null");
            }
            this.DbContext = (AuditSysContext)dbContext;
            this.DbSet = this.DbContext.Set<T>();
        }
        public bool CheckPermission(T t, int domainId) => t.DomainId == domainId;

        public bool CheckValidCode(string code, int id, int domainId)
        {
            code = (code + "").Trim().ToUpper();
            if (string.IsNullOrEmpty(code))
                return true;
            var byCode = this.DbSet.Where(o => o.DomainId == domainId && !o.Deleted && (o.Code).ToUpper().Equals(code)).FirstOrDefault();
            if (byCode == null)
                return true;
            return byCode.ID == id;
        }
        public IQueryable<T> GetAll()
        {
            return this.DbSet;
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return this.DbSet.Where(predicate);
        }
        public IQueryable<T> Search(ModelSearch obj, out int count)
        {
            obj.Key = (obj.Key + "").Trim().ToUpper();
            obj.Code = (obj.Code + "").Trim().ToUpper();
            var lst = this.DbSet.Where(o => !o.Deleted && o.DomainId == obj.DomainId
            && (obj.Status == -1 || o.Status == (obj.Status == 1))
            && (o.Code.ToUpper().Contains(obj.Code))
            && (o.Name.ToUpper().Contains(obj.Key))
            && (obj.UserCreate == null || o.UserCreate == obj.UserCreate)).OrderByDescending(o => o.CreateDate).Select(o => o);

            count = lst.Count();

            if (obj.StartNumber >= 0 && obj.PageSize > 0)
                return lst.Skip(obj.StartNumber).Take(obj.PageSize);
            return lst;
        }
        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public void Delete(T entity)
        {
            this.DbContext.Entry(entity).State = EntityState.Deleted;
            Save();
        }

        public T Add(T entity)
        {
            this.DbContext.Entry(entity).State = EntityState.Added;
            Save();
            return entity;
        }

        public void AddWithoutSave(T entity)
        {
            this.DbContext.Entry(entity).State = EntityState.Added;
        }

        public T Update(T entity)
        {
            this.DbContext.Entry(entity).State = EntityState.Modified;
            Save();
            return entity;
        }

        public int Save()
        {
            return DbContext.SaveChanges();
        }
    }
}
