using System.Linq.Expressions;

namespace TMS.Business.Repository
{
    public interface IRepositoryBase<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll();
        Task<TEntity> Get(int id);
        Task Add(TEntity entity, bool isSaveChanges = true);
        Task Change(TEntity entity, bool isSaveChanges = true);
        Task Delete(int id, bool isSaveChanges = true);
        Task RemoveRange(List<TEntity> entities, bool isSaveChanges = true);
        Task Save();
        Task AddRange(IList<TEntity> entity, bool isSaveChanges = true);
        Task UpdateRange(IList<TEntity> entity, bool isSaveChanges = true);
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null);
    }

}
