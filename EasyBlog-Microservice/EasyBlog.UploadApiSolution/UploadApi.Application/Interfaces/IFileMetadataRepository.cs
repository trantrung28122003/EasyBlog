using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UploadApi.Domain.Entites;

namespace UploadApi.Application.Interfaces
{
    public interface IFileMetadataRepository 
    {
        Task CreateAsync(FileMetadata fileMetadata);
        Task<FileMetadata?> GetByIdAsync(Guid id);
    }
}
