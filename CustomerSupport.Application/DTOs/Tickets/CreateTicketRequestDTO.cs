using CustomerSupport.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CustomerSupport.Application.DTOs.Tickets
{
    public sealed class CreateTicketRequestDTO
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int? CategoryId { get; set; }

        [EnumDataType(typeof(TicketPriorityType))]
        public TicketPriorityType PriorityId { get; set; } = TicketPriorityType.Medium;

        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [StringLength(5000, MinimumLength = 10)]
        public string Description { get; set; } = string.Empty;
    }
}
