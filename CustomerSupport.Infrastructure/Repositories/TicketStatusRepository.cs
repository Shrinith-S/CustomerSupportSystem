using CustomerSupport.Application.Interfaces.Repositories;
using CustomerSupport.Domain.Entities;
using CustomerSupport.Domain.Enums;
using CustomerSupport.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerSupport.Infrastructure.Repositories
{
    public sealed class TicketStatusRepository : ITicketStatusRepository
    {
        private readonly CustomerSupportDbContext _context;

        public TicketStatusRepository(CustomerSupportDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<TicketStatus>> GetAllAsync(bool activeOnly)
        {
            IQueryable<TicketStatus> query = _context.TicketStatuses.AsNoTracking();

            if (activeOnly)
            {
                query = query.Where(x => x.IsActive);
            }

            return await query.OrderBy(x => x.SortOrder).ToListAsync();
        }

        public async Task<TicketStatus?> GetByIdAsync(TicketStatusType id)
        {
            return await _context.TicketStatuses
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
