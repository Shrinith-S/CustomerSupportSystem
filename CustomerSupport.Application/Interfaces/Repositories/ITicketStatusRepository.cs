using CustomerSupport.Domain.Entities;
using CustomerSupport.Domain.Enums;
namespace CustomerSupport.Application.Interfaces.Repositories
{
    public interface ITicketStatusRepository
    {
        Task<IReadOnlyCollection<TicketStatus>> GetAllAsync(bool activeOnly);
        Task<TicketStatus?> GetByIdAsync(TicketStatusType id);
    }
}
