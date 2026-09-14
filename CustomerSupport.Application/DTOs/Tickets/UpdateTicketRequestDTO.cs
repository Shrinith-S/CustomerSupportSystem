using System.ComponentModel.DataAnnotations;

namespace CustomerSupport.Application.DTOs.Tickets
{
    public sealed class UpdateTicketRequestDTO
    {
        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [StringLength(5000, MinimumLength = 10)]
        public string Description { get; set; } = string.Empty;
    }
}
