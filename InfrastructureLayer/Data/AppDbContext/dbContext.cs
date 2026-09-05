using DomainLayer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<EmployeePermission> EmployeePermissions { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EmployeePermission>()
                .HasKey(ep => new { ep.UserId, ep.PermissionId });

            modelBuilder.Entity<EmployeePermission>()
                .HasOne(ep => ep.User)
                .WithMany()
                .HasForeignKey(ep => ep.UserId);

            modelBuilder.Entity<EmployeePermission>()
                .HasOne(ep => ep.Permission)
                .WithMany(p => p.EmployeePermissions)
                .HasForeignKey(ep => ep.PermissionId);
        }
    }
}