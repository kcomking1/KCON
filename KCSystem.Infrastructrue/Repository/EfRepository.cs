using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KCSystem.Core.Entities;
using KCSystem.Core.Interface;
using KCSystem.Infrastructrue.Database;    

namespace KCSystem.Infrastructrue.Repository
{
    public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        #region ctor
        public EfRepository(KCDBContext dbContext)
        { 
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
        }

         
        #endregion

        #region fields
        public readonly DbSet<TEntity> _dbSet;
        public readonly KCDBContext _dbContext;
        #endregion

        #region query
        public TEntity GetByKey(int key)
        {
            return _dbSet.Find(key);
        }

        public async Task<TEntity> GetByKeyAsync(int key)
        {
            return await _dbSet.FindAsync(key);
        }

        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> expression)
        {
            return _dbSet.Where(expression);
        }

        public IQueryable<TEntity> QueryAll()
        {
            return _dbSet.AsQueryable();
        }

        public IQueryable<TEntity> QueryFromSql(string sql)
        {
            return _dbSet.FromSqlRaw(sql);
        }

        public IQueryable<TEntity> FromSqlInterpolated(FormattableString sql)
        {
            return _dbSet.FromSqlInterpolated(sql);
        }

         

        #endregion

        #region insert
        public void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
             
        }

        public void Insert(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public async Task InsertAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task InsertAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        #endregion

        #region update
        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public void Remove(Expression<Func<TEntity, bool>> expression)
        {
            var entities = _dbSet.AsNoTracking().Where(expression).ToList();
            _dbSet.RemoveRange(entities);
        }

        #endregion

        #region remove
        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public void Update(IEnumerable<TEntity> entities)
        {
            
            _dbSet.UpdateRange(entities);
        }

      
        #endregion

    }
}