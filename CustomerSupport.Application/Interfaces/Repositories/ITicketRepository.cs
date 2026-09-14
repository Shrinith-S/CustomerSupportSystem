using CustomerSupport.Domain.Entities;
namespace CustomerSupport.Application.Interfaces.Repositories
{
    public interface ITicketRepository
    {
        Task<SupportTicket?> GetByIdAsync(int id, bool trackChanges);
        Task AddAsync(SupportTicket ticket);
        Task SaveChangesAsync();
    }
}
