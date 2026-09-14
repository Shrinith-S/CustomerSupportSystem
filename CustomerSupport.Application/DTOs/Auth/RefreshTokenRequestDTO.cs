using System.ComponentModel.DataAnnotations;

namespace CustomerSupport.Application.DTOs.Auth
{
    public sealed class RefreshTokenRequestDTO
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
