using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UploadApi.Application.Interfaces;
using UploadApi.Domain.Entites;
using UploadApi.Infrastructure.Data;

namespace UploadApi.Infrastructure.Repositories
{
    public class FileMetadataRepository : IFileMetadataRepository
    {
        private readonly UploadDbContext _context;
        public FileMetadataRepository(UploadDbContext context) 
        {
            _context = context;
        }

        public async Task CreateAsync(FileMetadata fileMetadata)
        {
            _context.FileMetadatas.Add(fileMetadata);
            await _context.SaveChangesAsync();
        }

        public async Task<FileMetadata?> GetByIdAsync(Guid id)
        {
            return await _context.FileMetadatas.FindAsync(id);
        }
    }
}
