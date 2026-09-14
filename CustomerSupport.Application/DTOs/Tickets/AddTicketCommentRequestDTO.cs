using System.ComponentModel.DataAnnotations;
namespace CustomerSupport.Application.DTOs.Tickets
{
    public sealed class AddTicketCommentRequestDTO
    {
        [Required]
        [StringLength(2000, MinimumLength = 1)]
        public string Content { get; set; } = string.Empty;

        public bool IsInternal { get; set; }
    }
}
