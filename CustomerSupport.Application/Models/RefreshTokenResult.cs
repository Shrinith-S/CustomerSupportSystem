namespace CustomerSupport.Application.Models
{
    public class RefreshTokenResult
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
