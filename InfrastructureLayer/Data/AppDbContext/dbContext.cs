using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DomainLayer.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Bill> Bills { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}