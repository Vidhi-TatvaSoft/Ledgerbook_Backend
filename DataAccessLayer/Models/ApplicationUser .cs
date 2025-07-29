using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Models;

public class ApplicationUser  : IdentityUser<int>
{

    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    public int? ProfileAttachmentId { get; set; }

    public bool IsUserRegistered { get; set; }

    public string? VerificationToken { get; set; }

    public bool IsEmailVerified { get; set; } = false;

    public DateTime CreatedAt { get; set; }
    public int? UpdatedById { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? DeletedById { get; set; }
    public DateTime? DeletedAt { get; set; }

    [ForeignKey("ProfileAttachmentId")]
    public virtual Attachment Attachments { get; set; } = null!;

    public static implicit operator IdentityUser(ApplicationUser v)
    {
        throw new NotImplementedException();
    }

}
