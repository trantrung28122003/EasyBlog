using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyBlog.SharedLibrary.Response
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }  
        public string Message { get; set; }
        public T? Results { get; set; }    
        public List<string>? Errors { get; set; } 

        public ApiResponse(bool isSuccess, string message, T? results = default, List<string>? errors = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            Results = results;
            Errors = errors;
        }
       
    }
}
