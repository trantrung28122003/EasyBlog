using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyBlog.SharedLibrary.Response
{
    public record class ApiResponse(bool Flag = false, string Message = null!);
}
