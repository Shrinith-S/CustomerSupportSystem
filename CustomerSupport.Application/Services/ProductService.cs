using CustomerSupport.Application.DTOs.Products;
using CustomerSupport.Application.Exceptions;
using CustomerSupport.Application.Interfaces.Repositories;
using CustomerSupport.Application.Interfaces.Services;
using CustomerSupport.Application.Mappings;
using Microsoft.Extensions.Logging;

namespace CustomerSupport.Application.Services
{
    public sealed class ProductService : IProductService
    {
        // Repository abstraction used for Product data-access operations.
        private readonly IProductRepository _productRepository;

        // Used to record important Product-related application events.
        private readonly ILogger<ProductService> _logger;

        // Dependencies are provided through ASP.NET Core Dependency Injection.
        public ProductService(
            IProductRepository productRepository,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<IReadOnlyCollection<ProductResponseDTO>> GetAllAsync(bool activeOnly)
        {
            // activeOnly determines whether we retrieve:
            // true  -> only active Products
            // false -> all Products, including inactive Products.
            _logger.LogInformation("Retrieving Products. ActiveOnly: {ActiveOnly}", activeOnly);

            // Ask the Repository to retrieve the Products.
            // The Service does not contain any EF Core query logic.
            var products = await _productRepository.GetAllAsync(activeOnly);

            _logger.LogInformation(
                "Retrieved {ProductCount} Products. ActiveOnly: {ActiveOnly}",
                products.Count,
                activeOnly);

            // Domain Entities should not be returned directly by the API.
            // Convert each Product Entity into ProductResponseDTO.
            return products
                .Select(product => product.ToResponseDTO())
                .ToList();
        }

        public async Task<ProductResponseDTO> GetByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving Product. ProductId: {ProductId}", id);

            // This operation only reads the Product.
            // trackChanges = false means EF Core does not need to track
            // this Entity because we are not going to modify it.
            // Disabling tracking improves performance for read-only queries.
            var product = await _productRepository.GetByIdAsync(id, trackChanges: false);

            // The requested Product does not exist.
            if (product is null)
            {
                _logger.LogWarning("Product was not found. ProductId: {ProductId}", id);

                // NotFoundException will be handled by GlobalExceptionHandler
                // and converted into HTTP 404 Not Found.
                throw new NotFoundException($"Product with ID {id} was not found.");
            }

            // Convert the Domain Entity into the Response DTO
            // before returning it to the API layer.
            return product.ToResponseDTO();
        }

        public async Task<ProductResponseDTO> CreateAsync(CreateProductRequestDTO request, int userId)
        {
            // Remove unnecessary leading and trailing spaces.
            // For example:
            // "  CRM Application  " becomes "CRM Application".
            // We use the normalized value when checking for duplicates.
            var name = request.Name.Trim();

            _logger.LogInformation("Creating Product. ProductName: {ProductName}, UserId: {UserId}", name, userId);

            // Check whether another Product already exists with the same name.
            // excludeId is null because this is a CREATE operation.
            // There is no current Product record to exclude.
            var exists = await _productRepository.NameExistsAsync(name, excludeId: null);

            if (exists)
            {
                _logger.LogWarning("Product creation rejected because the name already exists. ProductName: {ProductName}", name);

                // Duplicate Product names conflict with existing data.
                // ConflictException will be converted by
                // GlobalExceptionHandler into HTTP 409 Conflict.
                throw new ConflictException($"A Product with the name '{name}' already exists.");
            }

            // Convert the CreateProductRequestDTO into a Product Entity.
            // The mapping method also sets application-controlled values
            // such as IsActive and audit information using userId.
            var product = request.ToEntity(userId);

            // Add the new Product Entity to the DbContext through the Repository.
            await _productRepository.AddAsync(product);

            // Persist the Product in SQL Server.
            // After SaveChangesAsync(), database-generated values
            // such as Product.Id become available.
            await _productRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Product created successfully. ProductId: {ProductId}, ProductName: {ProductName}, UserId: {UserId}",
                product.Id,
                product.Name,
                userId);

            // Return the newly created Product as a Response DTO.
            return product.ToResponseDTO();
        }

        public async Task<ProductResponseDTO> UpdateAsync(int id, UpdateProductRequestDTO request, int userId)
        {
            _logger.LogInformation("Updating Product. ProductId: {ProductId}, UserId: {UserId}", id, userId);

            // Unlike GetByIdAsync(), this Entity will be modified.
            // Therefore, trackChanges = true is required so that
            // EF Core can detect the changes and update the database
            // when SaveChangesAsync() is called.
            var product = await _productRepository.GetByIdAsync(id, trackChanges: true);

            if (product is null)
            {
                _logger.LogWarning("Product update failed because the Product was not found. ProductId: {ProductId}", id);

                // The requested Product cannot be updated because it does not exist.
                throw new NotFoundException($"Product with ID {id} was not found.");
            }

            // Normalize the incoming Product name before checking for duplicates.
            var name = request.Name.Trim();

            // Check whether ANOTHER Product already uses this name.
            // id is passed as excludeId because the current Product
            // should not be considered a duplicate of itself.

            // Example:
            // Product 5 already has the name "CRM Application".
            // Updating Product 5 without changing its name should be valid.
            var exists = await _productRepository.NameExistsAsync(name, excludeId: id);

            if (exists)
            {
                _logger.LogWarning("Product update rejected because the name already exists. ProductId: {ProductId}, ProductName: {ProductName}", id, name);

                // Another Product already has the requested name.
                throw new ConflictException($"A Product with the name '{name}' already exists.");
            }

            // Copy only the allowed values from UpdateProductRequestDTO
            // into the existing tracked Product Entity.
            // The mapping method can also update audit values such as:
            // UpdatedAt
            // UpdatedBy
            product.MapFrom(request, userId);

            // Because the Product was loaded with tracking enabled,
            // EF Core already knows which Entity has changed.
            // We only need to save the changes.
            await _productRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Product updated successfully. ProductId: {ProductId}, UserId: {UserId}",
                product.Id,
                userId);

            // Return the updated Product as a Response DTO.
            return product.ToResponseDTO();
        }
    }
}
