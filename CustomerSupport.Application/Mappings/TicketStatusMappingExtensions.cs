using CustomerSupport.Application.DTOs.MasterData;
using CustomerSupport.Domain.Entities;

namespace CustomerSupport.Application.Mappings
{
    public static class TicketStatusMappingExtensions
    {
        public static TicketStatusResponseDTO ToResponseDTO(this TicketStatus status)
        {
            return new TicketStatusResponseDTO
            {
                Id = status.Id,
                Name = status.Name,
                Description = status.Description,
                SortOrder = status.SortOrder,
                IsActive = status.IsActive
            };
        }
    }
}
