using System.ComponentModel.DataAnnotations;

namespace CustomerSupport.Application.DTOs.Tickets
{
    public sealed class AssignTicketRequestDTO
    {
        [Range(1, int.MaxValue)]
        public int SupportExecutiveId { get; set; }
    }
}
