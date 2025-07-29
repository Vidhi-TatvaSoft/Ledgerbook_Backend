using DataAccessLayer.ViewModels;

namespace BusinessAcessLayer.Interface;

public interface IAttachmentService
{
    Task<int> SaveAttachment(AttachmentViewModel attachmentVM, int userId);

    AttachmentViewModel GetAttachmentById(int attachmentId);
}