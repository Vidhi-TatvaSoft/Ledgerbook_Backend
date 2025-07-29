using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models;

public class ExceptionLogs
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string ExceptionUrl { get; set; }
    [Required]
    public string ExcceptionMessage { get; set; }
    [Required]
    public string InnerException { get; set; }
    public int? UserId { get; set; } = null;
    [Required]
    public DateTime ExceptionAt { get; set; }
}
