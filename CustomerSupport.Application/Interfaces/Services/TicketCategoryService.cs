using CustomerSupport.Application.DTOs.Categories;
using CustomerSupport.Application.Exceptions;
using CustomerSupport.Application.Interfaces.Repositories;
using CustomerSupport.Application.Interfaces.Services;
using CustomerSupport.Application.Mappings;
using Microsoft.Extensions.Logging;

namespace CustomerSupport.Application.Services
{
    public sealed class TicketCategoryService : ITicketCategoryService
    {
        // Repository abstraction used for Ticket Category data-access operations.
        private readonly ITicketCategoryRepository _categoryRepository;

        // Used to record important Ticket Category operations and failures.
        private readonly ILogger<TicketCategoryService> _logger;

        // Dependencies are provided through ASP.NET Core Dependency Injection.
        public TicketCategoryService(
            ITicketCategoryRepository categoryRepository,
            ILogger<TicketCategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<IReadOnlyCollection<TicketCategoryResponseDTO>> GetAllAsync(bool activeOnly)
        {
            // activeOnly determines which records should be returned:
            // true  -> return only active Ticket Categories.
            // false -> return all Ticket Categories, including inactive ones.
            _logger.LogInformation("Retrieving Ticket Categories. ActiveOnly: {ActiveOnly}", activeOnly);

            // Ask the Repository to retrieve the data.
            // The Service does not contain EF Core query logic.
            var categories = await _categoryRepository.GetAllAsync(activeOnly);

            _logger.LogInformation(
                "Retrieved {CategoryCount} Ticket Categories. ActiveOnly: {ActiveOnly}",
                categories.Count,
                activeOnly);

            // Domain Entities should not be returned directly to the API client.
            // Convert each TicketCategory Entity into TicketCategoryResponseDTO.
            return categories
                .Select(category => category.ToResponseDTO())
                .ToList();
        }

        public async Task<TicketCategoryResponseDTO> GetByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving Ticket Category. CategoryId: {CategoryId}", id);

            // This operation only reads the Category.
            // trackChanges = false tells EF Core that it does not need
            // to track the Entity because we are not going to modify it.
            // This is more efficient for read-only operations.
            var category = await _categoryRepository.GetByIdAsync(id, trackChanges: false);

            // The requested Category does not exist.
            if (category is null)
            {
                _logger.LogWarning("Ticket Category was not found. CategoryId: {CategoryId}", id);

                // NotFoundException is handled by GlobalExceptionHandler
                // and converted into HTTP 404 Not Found.
                throw new NotFoundException($"Ticket Category with ID {id} was not found.");
            }

            // Convert the Domain Entity into the Response DTO
            // before returning it to the API layer.
            return category.ToResponseDTO();
        }

        public async Task<TicketCategoryResponseDTO> CreateAsync(CreateTicketCategoryRequestDTO request, int userId)
        {
            // Remove leading and trailing spaces from the Category name.
            // Example:
            // "  Login Issue  " becomes "Login Issue".
            // The normalized value is used when checking for duplicates.
            var name = request.Name.Trim();

            _logger.LogInformation(
                "Creating Ticket Category. CategoryName: {CategoryName}, UserId: {UserId}",
                name,
                userId);

            // Check whether another Category already exists with the same name.
            // excludeId is null because this is a CREATE operation.
            // There is no existing Category record to exclude.
            var exists = await _categoryRepository.NameExistsAsync(name, excludeId: null);

            if (exists)
            {
                _logger.LogWarning(
                    "Ticket Category creation rejected because the name already exists. CategoryName: {CategoryName}",
                    name);

                // Duplicate Category names conflict with existing application data.
                // ConflictException will be handled centrally
                // and converted into HTTP 409 Conflict.
                throw new ConflictException($"A Ticket Category with the name '{name}' already exists.");
            }

            // Convert the CreateTicketCategoryRequestDTO into a TicketCategory Domain Entity.
            // The mapping method can also set application-controlled
            // values such as IsActive and audit information.
            var category = request.ToEntity(userId);

            // Add the new Category Entity through the Repository.
            await _categoryRepository.AddAsync(category);

            // Persist the new Category in SQL Server.
            // After SaveChangesAsync(), database-generated values
            // such as Category.Id are available.
            await _categoryRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Ticket Category created successfully. CategoryId: {CategoryId}, CategoryName: {CategoryName}, UserId: {UserId}",
                category.Id,
                category.Name,
                userId);

            // Return the newly created Category as a Response DTO.
            return category.ToResponseDTO();
        }

        public async Task<TicketCategoryResponseDTO> UpdateAsync(int id, UpdateTicketCategoryRequestDTO request, int userId)
        {
            _logger.LogInformation(
                "Updating Ticket Category. CategoryId: {CategoryId}, UserId: {UserId}",
                id,
                userId);

            // This Entity will be modified, so tracking must be enabled.
            // trackChanges = true allows EF Core to detect
            // which properties are changed and generate the UPDATE statement
            // when SaveChangesAsync() is called.
            var category = await _categoryRepository.GetByIdAsync(id, trackChanges: true);

            if (category is null)
            {
                _logger.LogWarning("Ticket Category update failed because the Category was not found. CategoryId: {CategoryId}", id);

                // The requested Category cannot be updated because it does not exist.
                throw new NotFoundException($"Ticket Category with ID {id} was not found.");
            }

            // Normalize the new name before checking for duplicates.
            var name = request.Name.Trim();

            // Check whether ANOTHER Ticket Category already uses this name.
            // The current Category ID is passed as excludeId.
            // This prevents the Category from being treated as a duplicate
            // of itself when its name has not changed.

            // Example:
            // Category 5 is already named "Login Issue".
            // Updating Category 5 without changing the name should be valid.
            var exists = await _categoryRepository.NameExistsAsync(name, excludeId: id);

            if (exists)
            {
                _logger.LogWarning(
                    "Ticket Category update rejected because the name already exists. CategoryId: {CategoryId}, CategoryName: {CategoryName}",
                    id,
                    name);

                // Another Category already has the requested name.
                throw new ConflictException($"A Ticket Category with the name '{name}' already exists.");
            }

            // Copy only the allowed values from UpdateTicketCategoryRequestDTO
            // into the existing tracked TicketCategory Entity.
            // The mapping method also updates audit fields such as:
            // UpdatedAt
            // UpdatedBy
            category.MapFrom(request, userId);

            // Because the Category was loaded with tracking enabled,
            // EF Core already knows that the Entity has changed.
            // We only need to save the changes.
            await _categoryRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Ticket Category updated successfully. CategoryId: {CategoryId}, UserId: {UserId}",
                category.Id,
                userId);

            // Return the updated Category as a Response DTO.
            return category.ToResponseDTO();
        }
    }
}
