using InfrastructureLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Bill> Bill { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}