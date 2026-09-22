using CustomerSupport.Domain.Entities;
namespace CustomerSupport.Application.Interfaces.Authentication
{
    public interface IPasswordService
    {
        string HashPassword(User user, string password);
        bool VerifyPassword(User user, string passwordHash, string providedPassword);
    }
}
