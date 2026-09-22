using CustomerSupport.Application.DTOs.MasterData;
using CustomerSupport.Application.Interfaces.Repositories;
using CustomerSupport.Application.Interfaces.Services;
using CustomerSupport.Application.Mappings;
using Microsoft.Extensions.Logging;

namespace CustomerSupport.Application.Services
{
    public sealed class MasterDataService : IMasterDataService
    {
        // Repository used to retrieve Ticket Priority master records.
        private readonly ITicketPriorityRepository _priorityRepository;

        // Repository used to retrieve Ticket Status master records.
        private readonly ITicketStatusRepository _statusRepository;

        // Used to record important master-data retrieval operations.
        private readonly ILogger<MasterDataService> _logger;

        // Dependencies are provided through Dependency Injection.
        public MasterDataService(
            ITicketPriorityRepository priorityRepository,
            ITicketStatusRepository statusRepository,
            ILogger<MasterDataService> logger)
        {
            _priorityRepository = priorityRepository;
            _statusRepository = statusRepository;
            _logger = logger;
        }

        public async Task<IReadOnlyCollection<TicketPriorityResponseDTO>> GetTicketPrioritiesAsync()
        {
            _logger.LogInformation("Retrieving active Ticket Priorities.");

            // Ticket Priority is application-controlled master data.
            // We pass activeOnly = true because API clients should
            // normally use only currently active Priority values
            // when creating or managing Support Tickets.
            var priorities = await _priorityRepository.GetAllAsync(activeOnly: true);

            _logger.LogInformation("Retrieved {PriorityCount} active Ticket Priorities.", priorities.Count);

            // Convert every TicketPriority Entity into
            // TicketPriorityResponseDTO before returning the result.
            return priorities
                .Select(priority => priority.ToResponseDTO())
                .ToList();
        }

        public async Task<IReadOnlyCollection<TicketStatusResponseDTO>> GetTicketStatusesAsync()
        {
            _logger.LogInformation("Retrieving active Ticket Statuses.");

            // Ticket Status is also application-controlled, read-only master data.
            // Only active Status values are exposed to API clients.
            // These values will later be used when displaying
            // or managing the Support Ticket workflow.
            var statuses = await _statusRepository.GetAllAsync(activeOnly: true);

            _logger.LogInformation("Retrieved {StatusCount} active Ticket Statuses.", statuses.Count);

            // Convert the Domain Entities into Response DTOs
            // before sending the data back to the API layer.
            return statuses
                .Select(status => status.ToResponseDTO())
                .ToList();
        }
    }
}
