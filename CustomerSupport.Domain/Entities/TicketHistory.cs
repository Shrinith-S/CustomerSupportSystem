namespace CustomerSupport.Domain.Entities
{
    public sealed class TicketHistory : BaseEntity<int>
    {
        public int SupportTicketId { get; set; }
        public int ChangedByUserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? FieldName { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? Remarks { get; set; }
        public SupportTicket SupportTicket { get; set; } = null!;
        public User ChangedByUser { get; set; } = null!;
    }
}
