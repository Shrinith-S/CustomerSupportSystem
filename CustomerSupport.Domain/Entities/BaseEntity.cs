namespace CustomerSupport.Domain.Entities
{
    public abstract class BaseEntity<TId>
    {
        public TId Id { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
