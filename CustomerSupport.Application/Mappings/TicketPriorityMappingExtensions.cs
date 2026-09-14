using CustomerSupport.Application.DTOs.MasterData;
using CustomerSupport.Domain.Entities;

namespace CustomerSupport.Application.Mappings
{
    public static class TicketPriorityMappingExtensions
    {
        public static TicketPriorityResponseDTO ToResponseDTO(this TicketPriority priority)
        {
            return new TicketPriorityResponseDTO
            {
                Id = priority.Id,
                Name = priority.Name,
                Description = priority.Description,
                SortOrder = priority.SortOrder,
                IsActive = priority.IsActive
            };
        }
    }
}
