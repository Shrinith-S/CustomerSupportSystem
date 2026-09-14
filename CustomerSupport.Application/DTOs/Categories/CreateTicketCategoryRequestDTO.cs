using System.ComponentModel.DataAnnotations;

namespace CustomerSupport.Application.DTOs.Categories
{
    public sealed class CreateTicketCategoryRequestDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
