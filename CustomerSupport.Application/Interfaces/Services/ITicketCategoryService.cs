using CustomerSupport.Application.DTOs.Categories;

namespace CustomerSupport.Application.Interfaces.Services
{
    public interface ITicketCategoryService
    {
        Task<IReadOnlyCollection<TicketCategoryResponseDTO>> GetAllAsync(bool activeOnly);
        Task<TicketCategoryResponseDTO> GetByIdAsync(int id);
        Task<TicketCategoryResponseDTO> CreateAsync(CreateTicketCategoryRequestDTO request, int userId);
        Task<TicketCategoryResponseDTO> UpdateAsync(int id, UpdateTicketCategoryRequestDTO request, int userId);
    }
}
