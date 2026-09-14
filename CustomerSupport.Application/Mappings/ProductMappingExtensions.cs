using CustomerSupport.Application.DTOs.Products;
using CustomerSupport.Domain.Entities;

namespace CustomerSupport.Application.Mappings
{
    public static class ProductMappingExtensions
    {
        public static Product ToEntity(
            this CreateProductRequestDTO request,
            int createdBy)
        {
            return new Product
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                IsActive = true,
                CreatedBy = createdBy
            };
        }

        public static void MapFrom(
            this Product product,
            UpdateProductRequestDTO request,
            int updatedBy)
        {
            product.Name = request.Name.Trim();
            product.Description = request.Description?.Trim();
            product.IsActive = request.IsActive;
            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedBy = updatedBy;
        }

        public static ProductResponseDTO ToResponseDTO(this Product product)
        {
            return new ProductResponseDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }
    }
}
