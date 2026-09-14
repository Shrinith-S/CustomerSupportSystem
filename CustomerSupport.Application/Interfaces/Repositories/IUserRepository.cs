using CustomerSupport.Domain.Entities;
namespace CustomerSupport.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PhoneNumberExistsAsync(string phoneNumber);
        Task AddAsync(User user);
        Task SaveChangesAsync();
    }
}
