using CustomerSupport.Domain.Entities;
namespace CustomerSupport.Application.Interfaces.Repositories
{
    public interface ITicketCategoryRepository
    {
        Task<IReadOnlyCollection<TicketCategory>> GetAllAsync(bool activeOnly);
        Task<TicketCategory?> GetByIdAsync(int id, bool trackChanges);
        Task<bool> NameExistsAsync(string name, int? excludeId);
        Task AddAsync(TicketCategory category);
        Task SaveChangesAsync();
    }
}
