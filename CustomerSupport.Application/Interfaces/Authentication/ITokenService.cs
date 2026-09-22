using CustomerSupport.Application.Models;
using CustomerSupport.Domain.Entities;

namespace CustomerSupport.Application.Interfaces.Authentication
{
    public interface ITokenService
    {
        AccessTokenResult CreateAccessToken(User user);
        RefreshTokenResult CreateRefreshToken();
        string ComputeRefreshTokenHash(string refreshToken);
    }
}
