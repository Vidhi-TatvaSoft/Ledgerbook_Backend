using Microsoft.AspNetCore.Http;

namespace DataAccessLayer.ViewModels;

public class AttachmentViewModel
{
    public int AttachmentId { get; set; }
    public string FileName { get; set; }
    public string BusinesLogoPath { get; set; }
    public string FileExtension { get; set; }
}