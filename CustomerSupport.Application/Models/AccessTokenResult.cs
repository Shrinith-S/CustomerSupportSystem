namespace CustomerSupport.Application.Models
{
    public class AccessTokenResult
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
