using CustomerSupport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupport.Infrastructure.Data.Configurations
{
    public sealed class TicketAssignmentConfiguration
        : IEntityTypeConfiguration<TicketAssignment>
    {
        public void Configure(EntityTypeBuilder<TicketAssignment> builder)
        {
            builder.ToTable("TicketAssignments");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.SupportTicketId);

            builder.HasIndex(x => x.AssignedToUserId);

            builder.HasIndex(x => x.AssignedByUserId);

            builder.HasOne(x => x.SupportTicket)
                .WithMany(x => x.Assignments)
                .HasForeignKey(x => x.SupportTicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.AssignedToUser)
                .WithMany()
                .HasForeignKey(x => x.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AssignedByUser)
                .WithMany()
                .HasForeignKey(x => x.AssignedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
