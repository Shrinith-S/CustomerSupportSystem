using CustomerSupport.Application.DTOs.MasterData;

namespace CustomerSupport.Application.Interfaces.Services
{
    public interface IMasterDataService
    {
        Task<IReadOnlyCollection<TicketPriorityResponseDTO>> GetTicketPrioritiesAsync();
        Task<IReadOnlyCollection<TicketStatusResponseDTO>> GetTicketStatusesAsync();
    }
}
