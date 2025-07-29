
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Models;

public class LedgerBookDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,int>
{
    // public DbSet<User> Users { get; set; }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permissions> Permissions { get; set; }
    public DbSet<RolePermissionMappings> RolePermissionMappings { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<ReferenceDataEntities> ReferenceDataEntities { get; set; }
    public DbSet<ReferenceDataValues> ReferenceDataValues { get; set; }
    public DbSet<Businesses> Businesses { get; set; }
    public DbSet<PersonalDetails> PersonalDetails { get; set; }
    public DbSet<UserBusinessMappings> UserBusinessMappings { get; set; }
    public DbSet<Parties> Parties { get; set; }
    public DbSet<LedgerTransactions> LedgerTransactions { get; set; }
    public DbSet<ExceptionLogs> ExceptionLogs{ get; set; }
    public DbSet<ActivityLogs> ActivityLogs { get; set; }


    public LedgerBookDbContext(DbContextOptions<LedgerBookDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Businesses>()
            .HasOne(b => b.BusinessCategory)
            .WithMany()
            .HasForeignKey(b => b.BusinessCategoryId)
            .OnDelete(DeleteBehavior.Restrict); // or DeleteBehavior.NoAction

        // Do the same for any other similar relationship
    }
}
