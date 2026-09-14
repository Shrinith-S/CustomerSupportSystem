namespace CustomerSupport.Application.DTOs.Tickets
{
    public sealed class TicketHistoryResponseDTO
    {
        public int Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? FieldName { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? Remarks { get; set; }
        public string ChangedByUserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
