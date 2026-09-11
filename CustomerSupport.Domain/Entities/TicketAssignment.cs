namespace CustomerSupport.Domain.Entities
{
    public sealed class TicketAssignment : BaseEntity<int>
    {
        public int SupportTicketId { get; set; }
        public int AssignedToUserId { get; set; }
        public int AssignedByUserId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UnassignedAt { get; set; }
        public SupportTicket SupportTicket { get; set; } = null!;
        public User AssignedToUser { get; set; } = null!;
        public User AssignedByUser { get; set; } = null!;
    }
}
