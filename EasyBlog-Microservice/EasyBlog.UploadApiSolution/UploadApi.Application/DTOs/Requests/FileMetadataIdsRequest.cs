using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadApi.Application.DTOs.Requests
{
    public class FileMetadataIdsRequest
    {
        public List<string> FileIds { get; set; } = new();
    }
}
