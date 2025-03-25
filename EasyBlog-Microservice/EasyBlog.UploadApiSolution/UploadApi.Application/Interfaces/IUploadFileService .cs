using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace UploadApi.Application.Interfaces
{
    public interface IUploadFileService
    {
        Task<string> UploadFileAsync(IFormFile file);
    }
}
