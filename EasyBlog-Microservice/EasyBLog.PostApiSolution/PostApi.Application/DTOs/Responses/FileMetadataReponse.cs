
namespace PostApi.Application.DTOs.Reponses
{
    public class FileMetadataReponse
    {
        public string Id { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string FileType { get; set; } = string.Empty;
        public DateTime DateCreate { get; set; }
    }
}
