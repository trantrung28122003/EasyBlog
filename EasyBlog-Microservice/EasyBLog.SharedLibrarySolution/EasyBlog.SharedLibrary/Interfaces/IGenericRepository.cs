using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using EasyBlog.SharedLibrary.Response;
namespace EasyBlog.SharedLibrary.Interface
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<List<TEntity>> GetAllAsync();
        Task<List<TEntity>> GetAllActiveAsync();
        Task<List<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity?> GetByIdAsync(Guid id);
        Task CreateAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task SoftDeleteAsync(Guid id);
    }
}
