using CustomerSupport.Application.DTOs.Products;

namespace CustomerSupport.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<IReadOnlyCollection<ProductResponseDTO>> GetAllAsync(bool activeOnly);
        Task<ProductResponseDTO> GetByIdAsync(int id);
        Task<ProductResponseDTO> CreateAsync(CreateProductRequestDTO request, int userId);
        Task<ProductResponseDTO> UpdateAsync(int id, UpdateProductRequestDTO request, int userId);
    }
}
