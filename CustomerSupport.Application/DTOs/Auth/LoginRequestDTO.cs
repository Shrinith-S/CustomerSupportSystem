using System.ComponentModel.DataAnnotations;
namespace CustomerSupport.Application.DTOs.Auth
{
    public sealed class LoginRequestDTO
    {
        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
