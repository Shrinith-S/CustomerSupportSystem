namespace CustomerSupport.Domain.Entities
{
    public sealed class FAQ : BaseEntity<int>
    {
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public int? ProductId { get; set; }
        public bool IsActive { get; set; } = true;
        public Product? Product { get; set; }
    }
}
