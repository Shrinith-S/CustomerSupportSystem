using CustomerSupport.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CustomerSupport.Application.DTOs.Tickets
{
    public sealed class ChangeTicketStatusRequestDTO
    {
        [EnumDataType(typeof(TicketStatusType))]
        public TicketStatusType StatusId { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }
    }
}
