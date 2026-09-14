using CustomerSupport.Application.DTOs.Tickets;
using CustomerSupport.Domain.Entities;
using CustomerSupport.Domain.Enums;

namespace CustomerSupport.Application.Mappings
{
    public static class TicketMappingExtensions
    {
        public static SupportTicket ToEntity(
            this CreateTicketRequestDTO request,
            int customerId)
        {
            return new SupportTicket
            {
                CustomerId = customerId,
                ProductId = request.ProductId,
                CategoryId = request.CategoryId,
                PriorityId = request.PriorityId,
                StatusId = TicketStatusType.Open,
                Subject = request.Subject.Trim(),
                Description = request.Description.Trim(),
                CreatedBy = customerId
            };
        }

        public static void MapFrom(
            this SupportTicket ticket,
            UpdateTicketRequestDTO request,
            int updatedBy)
        {
            ticket.Subject = request.Subject.Trim();
            ticket.Description = request.Description.Trim();
            ticket.UpdatedAt = DateTime.UtcNow;
            ticket.UpdatedBy = updatedBy;
        }

        public static TicketSummaryResponseDTO ToSummaryDTO(this SupportTicket ticket)
        {
            return new TicketSummaryResponseDTO
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                Subject = ticket.Subject,
                CustomerName = $"{ticket.Customer.FirstName} {ticket.Customer.LastName}",
                ProductName = ticket.Product.Name,
                CategoryName = ticket.Category?.Name,
                PriorityId = ticket.PriorityId,
                PriorityName = ticket.Priority.Name,
                StatusId = ticket.StatusId,
                StatusName = ticket.Status.Name,
                AssignedToUserName =
                    ticket.AssignedToUser == null
                        ? null
                        : $"{ticket.AssignedToUser.FirstName} " +
                          $"{ticket.AssignedToUser.LastName}",

                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt
            };
        }
    }
}
