using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EasyBlog.SharedLibrary.Response;
namespace EasyBlog.SharedLibrary.Interface
{
    public interface IGenericInterface<T> where T : class
    {
        Task<ApiResponse> CreateAsync(T entity);
        Task<ApiResponse> UpdateAsync(T entity);
        Task<ApiResponse> DeleteAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);
        Task<T> GetByAsync(Expression<Func<T, bool>> predicate);
    }
}
