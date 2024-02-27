using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace TMS.Business.Repository
{
    public class Repository<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        private readonly DbContext _dbContext;
        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get all records
        /// </summary>
        /// <returns>IQueryable</returns>
        public IQueryable<TEntity> GetAll()
        {
            return _dbContext.Set<TEntity>();
        }

        /// <summary>
        /// Get single record
        /// </summary>
        /// <param name="id">DB primary key</param>
        /// <returns>Entity</returns>
        public async Task<TEntity> Get(int id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        /// <summary>
        /// Add new record in database
        /// </summary>
        /// <param name="entity">Record</param>
        /// <param name="isSaveChanges">Save db changes or not</param>
        public async Task Add(TEntity entity, bool isSaveChanges = true)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
            if (isSaveChanges)
            {
                await Save();
            }
        }

        /// <summary>
        /// Update record in database
        /// </summary>
        /// <param name="entity">Record</param>
        /// <param name="isSaveChanges">Save db changes or not</param>
        public async Task Change(TEntity entity, bool isSaveChanges = true)
        {
            _dbContext.Set<TEntity>().Update(entity);
            if (isSaveChanges)
            {
                await Save();
            }
        }

        /// <summary>
        /// Delete record from database
        /// </summary>
        /// <param name="id">DB primary key</param>
        /// <param name="isSaveChanges">Save db changes or not</param>
        public async Task Delete(int id, bool isSaveChanges = true)
        {
            var entity = await Get(id);
            if (entity != null)
            {
                _dbContext.Set<TEntity>().Remove(entity);
                if (isSaveChanges)
                {
                    await Save();
                }
            }
        }

        public async Task RemoveRange(List<TEntity> entities, bool isSaveChanges = true)
        {
            if (entities != null && entities.Count > 0)
            {
                _dbContext.Set<TEntity>().RemoveRange(entities);
                if (isSaveChanges)
                {
                    await Save();
                }
            }
        }


        /// <summary>
        /// Save database changes
        /// </summary>
        public async Task Save()
        {
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Add list to database
        /// </summary>
        /// <param name="entity">List of entities</param>
        /// <param name="isSaveChanges">Save db changes or not</param>
        public async Task AddRange(IList<TEntity> entity, bool isSaveChanges = true)
        {
            await _dbContext.Set<TEntity>().AddRangeAsync(entity);
            if (isSaveChanges)
            {
                await Save();
            }
        }

        /// <summary>
        /// Update list to database
        /// </summary>
        /// <param name="entity">List of entities</param>
        /// <param name="isSaveChanges">Save db changes or not</param>
        public async Task UpdateRange(IList<TEntity> entity, bool isSaveChanges = true)
        {
            _dbContext.Set<TEntity>().UpdateRange(entity);
            if (isSaveChanges)
            {
                await Save();
            }
        }

        /// <summary>
        /// Get all records
        /// </summary>
        /// <param name="filter">Lemda expression</param>
        /// <returns>IQueryable</returns>
        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null)
        {
            return _dbContext.Set<TEntity>().Where(filter);
        }
    }
}