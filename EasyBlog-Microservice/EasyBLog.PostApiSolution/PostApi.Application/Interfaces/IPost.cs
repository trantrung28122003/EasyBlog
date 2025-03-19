using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyBlog.SharedLibrary.Interface;
using PostApi.Domain.Entities;

namespace PostApi.Application.Interfaces
{
    public interface IPost : IGenericInterface<Post>
    {
    }
}
