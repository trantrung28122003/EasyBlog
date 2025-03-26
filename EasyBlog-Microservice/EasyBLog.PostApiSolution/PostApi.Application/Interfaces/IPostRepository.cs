using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyBlog.SharedLibrary.Interface;
using PostApi.Application.DTOs.Responses;
using PostApi.Domain.Entities;

namespace PostApi.Application.Interfaces
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        Task<List<PostResponse>> GetAllPagedAsync(int offset, int limit);
       
    }
}
