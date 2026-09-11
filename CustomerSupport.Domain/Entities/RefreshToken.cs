namespace CustomerSupport.Domain.Entities
{
    public sealed class RefreshToken : BaseEntity<int>
    {
        public int UserId { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string? CreatedByIp { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReplacedByTokenHash { get; set; }
        public string? ReasonRevoked { get; set; }
        public User User { get; set; } = null!;
    }
}
