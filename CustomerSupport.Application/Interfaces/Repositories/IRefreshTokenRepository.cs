using CustomerSupport.Domain.Entities;
namespace CustomerSupport.Application.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
        Task AddAsync(RefreshToken refreshToken);
        Task SaveChangesAsync();
    }
}
