using CustomerSupport.Application.DTOs.Auth;

namespace CustomerSupport.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO request, string? ipAddress);
        Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request, string? ipAddress);
        Task<AuthResponseDTO> RefreshTokenAsync(RefreshTokenRequestDTO request, string? ipAddress);
        Task RevokeRefreshTokenAsync(RefreshTokenRequestDTO request, string? ipAddress);
    }
}
