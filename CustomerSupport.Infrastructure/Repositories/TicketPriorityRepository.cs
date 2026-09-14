using CustomerSupport.Application.Interfaces.Repositories;
using CustomerSupport.Domain.Entities;
using CustomerSupport.Domain.Enums;
using CustomerSupport.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerSupport.Infrastructure.Repositories
{
    public sealed class TicketPriorityRepository : ITicketPriorityRepository
    {
        private readonly CustomerSupportDbContext _context;

        public TicketPriorityRepository(CustomerSupportDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<TicketPriority>> GetAllAsync(bool activeOnly)
        {
            IQueryable<TicketPriority> query =
                _context.TicketPriorities.AsNoTracking();

            if (activeOnly)
            {
                query = query.Where(x => x.IsActive);
            }

            return await query
                .OrderBy(x => x.SortOrder)
                .ToListAsync();
        }

        public async Task<TicketPriority?> GetByIdAsync(TicketPriorityType id)
        {
            return await _context.TicketPriorities
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
