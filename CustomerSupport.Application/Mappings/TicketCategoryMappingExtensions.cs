using CustomerSupport.Application.DTOs.Categories;
using CustomerSupport.Domain.Entities;

namespace CustomerSupport.Application.Mappings
{
    public static class TicketCategoryMappingExtensions
    {
        public static TicketCategory ToEntity(
            this CreateTicketCategoryRequestDTO request,
            int createdBy)
        {
            return new TicketCategory
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                IsActive = true,
                CreatedBy = createdBy
            };
        }

        public static void MapFrom(
            this TicketCategory category,
            UpdateTicketCategoryRequestDTO request,
            int updatedBy)
        {
            category.Name = request.Name.Trim();
            category.Description = request.Description?.Trim();
            category.IsActive = request.IsActive;
            category.UpdatedAt = DateTime.UtcNow;
            category.UpdatedBy = updatedBy;
        }

        public static TicketCategoryResponseDTO ToResponseDTO(this TicketCategory category)
        {
            return new TicketCategoryResponseDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive
            };
        }
    }
}