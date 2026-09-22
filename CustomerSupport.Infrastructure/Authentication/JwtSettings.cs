namespace CustomerSupport.Infrastructure.Authentication
{
    public sealed class JwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int AccessTokenMinutes { get; set; } = 30;
        public int RefreshTokenDays { get; set; } = 7;

        // Grace period allowed while validating the Access Token.
        // Example:
        // Access Token expires at 10:30 AM.
        // ClockSkewMinutes = 2.
        // The API will accept the token until approximately 10:32 AM.
        public int ClockSkewMinutes { get; set; } = 2;
    }
}
