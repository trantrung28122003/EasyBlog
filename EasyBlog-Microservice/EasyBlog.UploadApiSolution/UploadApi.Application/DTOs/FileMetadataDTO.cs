using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadApi.Application.DTOs
{
    public class FileMetadataDTO
    {
        public required string Id { get; set; }
        public string? FileName { get; set; }
        public required string FileUrl { get; set; }
        public long? FileSize { get; set; }
        public required string FileType { get; set; }
        public DateTime DateCreate { get; set; }
    }
}
