namespace CustomerSupport.Domain.Entities
{
    public sealed class TicketComment : BaseEntity<int>
    {
        public int SupportTicketId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsInternal { get; set; }
        public SupportTicket SupportTicket { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
