using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApi.Application.DTOs.Requests
{
    public class FileMetadataIdsRequest
    {
        public List<string> FileIds { get; set; } = new();
    }
}
