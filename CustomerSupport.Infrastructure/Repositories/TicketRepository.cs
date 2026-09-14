using CustomerSupport.Application.Interfaces.Repositories;
using CustomerSupport.Domain.Entities;
using CustomerSupport.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerSupport.Infrastructure.Repositories
{
    public sealed class TicketRepository : ITicketRepository
    {
        private readonly CustomerSupportDbContext _context;

        public TicketRepository(CustomerSupportDbContext context)
        {
            _context = context;
        }

        public async Task<SupportTicket?> GetByIdAsync(int id, bool trackChanges)
        {
            IQueryable<SupportTicket> query = _context.SupportTickets;

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            return await query
                .Include(x => x.Customer)
                .Include(x => x.Product)
                .Include(x => x.Category)
                .Include(x => x.Priority)
                .Include(x => x.Status)
                .Include(x => x.AssignedToUser)
                .Include(x => x.Comments)
                    .ThenInclude(x => x.User)
                .Include(x => x.History)
                    .ThenInclude(x => x.ChangedByUser)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(SupportTicket ticket)
        {
            await _context.SupportTickets.AddAsync(ticket);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
