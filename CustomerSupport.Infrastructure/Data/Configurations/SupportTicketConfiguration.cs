using CustomerSupport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupport.Infrastructure.Data.Configurations
{
    public sealed class SupportTicketConfiguration
        : IEntityTypeConfiguration<SupportTicket>
    {
        public void Configure(EntityTypeBuilder<SupportTicket> builder)
        {
            builder.ToTable("SupportTickets");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TicketNumber)
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.Subject)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(x => x.PriorityId)
                .HasConversion<int>();

            builder.Property(x => x.StatusId)
                .HasConversion<int>();

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasIndex(x => x.TicketNumber)
                .IsUnique();

            builder.HasIndex(x => x.CustomerId);

            builder.HasIndex(x => x.ProductId);

            builder.HasIndex(x => x.CategoryId);

            builder.HasIndex(x => x.PriorityId);

            builder.HasIndex(x => x.StatusId);

            builder.HasIndex(x => x.AssignedToUserId);

            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Priority)
                .WithMany()
                .HasForeignKey(x => x.PriorityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Status)
                .WithMany()
                .HasForeignKey(x => x.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AssignedToUser)
                .WithMany()
                .HasForeignKey(x => x.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
