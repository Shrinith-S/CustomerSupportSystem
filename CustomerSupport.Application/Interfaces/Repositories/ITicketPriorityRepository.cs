using CustomerSupport.Domain.Entities;
using CustomerSupport.Domain.Enums;
namespace CustomerSupport.Application.Interfaces.Repositories
{
    public interface ITicketPriorityRepository
    {
        Task<IReadOnlyCollection<TicketPriority>> GetAllAsync(bool activeOnly);
        Task<TicketPriority?> GetByIdAsync(TicketPriorityType id);
    }
}

