using System.ComponentModel.DataAnnotations;

namespace Domain.RequestModel
{
    public class FileSaveRequest
    {
        public string SessionToken { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty;
        public string FolderName { get; set; } = string.Empty;
        public string FileBase64 { get; set; } = string.Empty;
    }


    public class FileSaveRequestV2
    {
        [Required]
        public string webToken { get; set; } = string.Empty;
        [Required]
        public string fileName { get; set; } = string.Empty;
        [Required]
        public string fileExtension { get; set; } = string.Empty;
        [Required]
        public string folderName { get; set; } = string.Empty;
        [Required]
        public string fileBase64 { get; set; } = string.Empty;
    }


    public class RasieComplaintFileSaveRequest : FileSaveRequestV2
    {
        public string retailerCode { get; set; } = string.Empty;
    }
}
