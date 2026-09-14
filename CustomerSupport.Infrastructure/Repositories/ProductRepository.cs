using CustomerSupport.Application.Interfaces.Repositories;
using CustomerSupport.Domain.Entities;
using CustomerSupport.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerSupport.Infrastructure.Repositories
{
    public sealed class ProductRepository : IProductRepository
    {
        private readonly CustomerSupportDbContext _context;

        public ProductRepository(CustomerSupportDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<Product>> GetAllAsync(bool activeOnly)
        {
            IQueryable<Product> query = _context.Products.AsNoTracking();

            if (activeOnly)
            {
                query = query.Where(x => x.IsActive);
            }

            return await query
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id, bool trackChanges)
        {
            IQueryable<Product> query = _context.Products;

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeId)
        {
            return await _context.Products
                .AnyAsync(
                    x =>
                        x.Name == name &&
                        (!excludeId.HasValue ||
                         x.Id != excludeId.Value));
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
