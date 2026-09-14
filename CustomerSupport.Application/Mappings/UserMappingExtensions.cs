using CustomerSupport.Application.DTOs.Auth;
using CustomerSupport.Domain.Entities;

namespace CustomerSupport.Application.Mappings
{
    public static class UserMappingExtensions
    {
        public static UserResponseDTO ToResponseDTO(this User user)
        {
            return new UserResponseDTO
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.RoleId.ToString(),
                IsActive = user.IsActive
            };
        }
    }
}
