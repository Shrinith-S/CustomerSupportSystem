using CustomerSupport.Application.Interfaces.Authentication;
using CustomerSupport.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CustomerSupport.Infrastructure.Authentication
{
    public sealed class PasswordService : IPasswordService
    {
        // PasswordHasher<User> provides Microsoft's standard password hashing mechanism.
        // We do not store or compare plain-text passwords.
        private readonly PasswordHasher<User> _passwordHasher = new();

        public string HashPassword(User user, string password)
        {
            // Convert the plain-text password into a secure password hash.
            // Only this generated hash should be stored in the database.
            return _passwordHasher.HashPassword(user, password);
        }

        public bool VerifyPassword(User user, string passwordHash, string providedPassword)
        {
            // Compare the password entered during login with the stored password hash.
            // PasswordHasher performs the secure verification internally.
            var result = _passwordHasher.VerifyHashedPassword(
                user,
                passwordHash,
                providedPassword);

            // Failed means the provided password does not match.
            // Success and SuccessRehashNeeded both indicate a valid password.
            return result != PasswordVerificationResult.Failed;
        }
    }
}
