using InfrastructureLayer;
using InfrastructureLayer.Data;

namespace ApplicationLayer.Repositories
{
    public class CustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

    }
} 

