using CustomerSupport.Domain.Enums;
namespace CustomerSupport.Domain.Entities
{
    public sealed class SupportTicket : BaseEntity<int>
    {
        public string TicketNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int? CategoryId { get; set; }
        public TicketPriorityType PriorityId { get; set; }
        public TicketStatusType StatusId { get; set; }
        public int? AssignedToUserId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? ResolvedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public byte[] RowVersion { get; set; } = [];

        public User Customer { get; set; } = null!;
        public User? AssignedToUser { get; set; }
        public Product Product { get; set; } = null!;
        public TicketCategory? Category { get; set; }
        public TicketPriority Priority { get; set; } = null!;
        public TicketStatus Status { get; set; } = null!;

        public ICollection<TicketComment> Comments { get; set; }
            = new List<TicketComment>();

        public ICollection<TicketAssignment> Assignments { get; set; }
            = new List<TicketAssignment>();

        public ICollection<TicketAttachment> Attachments { get; set; }
            = new List<TicketAttachment>();

        public ICollection<TicketHistory> History { get; set; }
            = new List<TicketHistory>();
    }
}
