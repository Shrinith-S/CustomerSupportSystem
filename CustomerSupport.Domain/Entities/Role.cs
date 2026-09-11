using CustomerSupport.Domain.Enums;
namespace CustomerSupport.Domain.Entities
{
    public sealed class Role : BaseEntity<RoleType>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}