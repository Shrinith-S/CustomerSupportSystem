namespace CustomerSupport.Domain.Entities
{
    public sealed class TicketAttachment : BaseEntity<int>
    {
        public int SupportTicketId { get; set; }
        public int UploadedByUserId { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSizeInBytes { get; set; }
        public string RelativePath { get; set; } = string.Empty;
        public SupportTicket SupportTicket { get; set; } = null!;
        public User UploadedByUser { get; set; } = null!;
    }
}
