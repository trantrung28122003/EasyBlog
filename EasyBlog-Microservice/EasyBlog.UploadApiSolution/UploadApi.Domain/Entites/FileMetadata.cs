using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UploadApi.Domain.Enums;

namespace UploadApi.Domain.Entites
{
    public class FileMetadata
    {
        public Guid Id { get; set; }
        public string? FileName { get; set; }
        public required string FileUrl { get; set; }
        public long? FileSize { get; set; }
        public FileType FileType { get; set; }
        public DateTime DateCreate { get; set; } = DateTime.UtcNow;
        public required string ChangeBy { get; set; }
    }
}
