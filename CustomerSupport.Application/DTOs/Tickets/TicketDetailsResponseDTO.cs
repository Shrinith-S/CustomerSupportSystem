using CustomerSupport.Domain.Enums;

namespace CustomerSupport.Application.DTOs.Tickets
{
    public sealed class TicketDetailsResponseDTO
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public TicketPriorityType PriorityId { get; set; }
        public string PriorityName { get; set; } = string.Empty;
        public TicketStatusType StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public int? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<TicketCommentResponseDTO> Comments { get; set; } = [];
        public List<TicketHistoryResponseDTO> History { get; set; } = [];
    }
}
