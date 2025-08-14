using System.Threading.Tasks;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Constant;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace BusinessAcessLayer.Services;

public class AttachmentService : IAttachmentService
{
    private readonly LedgerBookDbContext _context;
    private readonly IGenericRepo _genericRepository;

    public AttachmentService(LedgerBookDbContext context, IGenericRepo genericRepo)
    {
        _context = context;
        _genericRepository = genericRepo;
    }

    public async Task<int> SaveAttachment(AttachmentViewModel attachmentVM, int userId)
    {
        Attachment attachment = new()
        {
            SourceType = EnumHelper.SourceType.Business,
            FileName = attachmentVM.FileName,
            FileExtensions = attachmentVM.FileExtension,
            Path = attachmentVM.BusinesLogoPath,
            CreatedAt = DateTime.UtcNow,
            CreatedById = userId
        };
        await _genericRepository.AddAsync<Attachment>(attachment);
        return attachment.Id;
    }
    
    public AttachmentViewModel GetAttachmentById(int attachmentId)
    {
        return _genericRepository.GetAll<Attachment>(a => a.Id == attachmentId && a.DeletedAt == null).Select(a => new AttachmentViewModel
        {
            AttachmentId = a.Id,
            FileName = a.FileName,
            FileExtension = a.FileExtensions,
            BusinesLogoPath = a.Path

        }).FirstOrDefault()!;
    }

    
}