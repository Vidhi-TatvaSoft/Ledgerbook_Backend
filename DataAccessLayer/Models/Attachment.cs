using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccessLayer.Constant;

namespace DataAccessLayer.Models;


public class Attachment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public EnumHelper.SourceType SourceType { get; set; }

    [Required]
    [StringLength(500)]
    public string FileName { get; set; }

    [Required]
    [StringLength(50)]
    public string FileExtensions { get; set; }

    [Required]
    public string Path { get; set; }

    public int CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? DeletedById { get; set; }
    public DateTime? DeletedAt { get; set; }
}
