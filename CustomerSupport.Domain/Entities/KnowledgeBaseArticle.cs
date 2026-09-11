namespace CustomerSupport.Domain.Entities
{
    public sealed class KnowledgeBaseArticle : BaseEntity<int>
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int? ProductId { get; set; }
        public bool IsPublished { get; set; }
        public bool IsActive { get; set; } = true;
        public byte[] RowVersion { get; set; } = [];
        public Product? Product { get; set; }
    }
}
