using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UploadApi.Application.DTOs.Responses;
using UploadApi.Domain.Entites;

namespace UploadApi.Application.DTOs.Conversions
{
    public static class FileMetadataConversion
    {
        public static FileMetadataResponse? FromEntity(FileMetadata? fileMetadata)
        {
            if (fileMetadata is null)
                return null;

            return ConvertToFileMetadataDTO(fileMetadata);
        }

        private static FileMetadataResponse ConvertToFileMetadataDTO(FileMetadata fileMetadata) => new()
        {
            Id = fileMetadata.Id.ToString(),
            FileName = fileMetadata.FileName,
            FileUrl = fileMetadata.FileUrl,
            FileSize = fileMetadata.FileSize,
            FileType = fileMetadata.FileType.ToString(),
            DateCreate = fileMetadata.DateCreate
        };

    }
}
