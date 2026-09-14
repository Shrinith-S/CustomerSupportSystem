using System.ComponentModel.DataAnnotations;

namespace CustomerSupport.Application.DTOs.Products
{
    public sealed class UpdateProductRequestDTO
    {
        [Required]
        [StringLength(150, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}
