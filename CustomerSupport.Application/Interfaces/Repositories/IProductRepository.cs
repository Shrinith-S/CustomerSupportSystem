using CustomerSupport.Domain.Entities;
namespace CustomerSupport.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<IReadOnlyCollection<Product>> GetAllAsync(bool activeOnly);
        Task<Product?> GetByIdAsync(int id, bool trackChanges);
        Task<bool> NameExistsAsync(string name, int? excludeId);
        Task AddAsync(Product product);
        Task SaveChangesAsync();
    }
}