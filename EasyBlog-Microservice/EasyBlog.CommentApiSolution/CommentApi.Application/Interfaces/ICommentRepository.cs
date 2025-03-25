using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommentApi.Domain.Entities;
using EasyBlog.SharedLibrary.Interface;

namespace CommentApi.Application.Interfaces
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {

    }
}
