using CustomerSupport.Application.Interfaces.Repositories;
using CustomerSupport.Domain.Entities;
using CustomerSupport.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerSupport.Infrastructure.Repositories
{
    public sealed class TicketCategoryRepository : ITicketCategoryRepository
    {
        private readonly CustomerSupportDbContext _context;

        public TicketCategoryRepository(CustomerSupportDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<TicketCategory>> GetAllAsync(bool activeOnly)
        {
            IQueryable<TicketCategory> query = _context.TicketCategories.AsNoTracking();

            if (activeOnly)
            {
                query = query.Where(x => x.IsActive);
            }

            return await query
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public async Task<TicketCategory?> GetByIdAsync(int id, bool trackChanges)
        {
            IQueryable<TicketCategory> query =
                _context.TicketCategories;

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            return await query
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeId)
        {
            return await _context.TicketCategories
                .AnyAsync(
                    x =>
                        x.Name == name &&
                        (!excludeId.HasValue ||
                         x.Id != excludeId.Value));
        }

        public async Task AddAsync(TicketCategory category)
        {
            await _context.TicketCategories.AddAsync(category);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
