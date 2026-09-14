using CustomerSupport.Domain.Enums;

namespace CustomerSupport.Application.DTOs.Tickets
{
    public sealed class TicketSummaryResponseDTO
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public TicketPriorityType PriorityId { get; set; }
        public string PriorityName { get; set; } = string.Empty;
        public TicketStatusType StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string? AssignedToUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
