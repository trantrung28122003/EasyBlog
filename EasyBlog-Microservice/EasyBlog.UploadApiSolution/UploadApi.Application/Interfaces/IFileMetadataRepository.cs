using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyBlog.SharedLibrary.Response;
using UploadApi.Application.DTOs;
using UploadApi.Domain.Entites;

namespace UploadApi.Application.Interfaces
{
    public interface IFileMetadataRepository 
    {
        Task CreateAsync(FileMetadata fileMetadata);
        Task<FileMetadata?> GetByIdAsync(Guid id);
        Task<List<FileMetadata>?> GetFilesMetadataByIdsAsync(List<Guid> ids);

    }
}
