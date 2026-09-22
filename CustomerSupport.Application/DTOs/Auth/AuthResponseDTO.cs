namespace CustomerSupport.Application.DTOs.Auth
{
    public sealed class AuthResponseDTO
    {
        public UserResponseDTO User { get; set; } = new();
        public string AccessToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiresAt { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiresAt { get; set; }
    }
}
