using CustomerSupport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupport.Infrastructure.Data.Configurations
{
    public sealed class TicketHistoryConfiguration
        : IEntityTypeConfiguration<TicketHistory>
    {
        public void Configure(EntityTypeBuilder<TicketHistory> builder)
        {
            builder.ToTable("TicketHistories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Action)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.FieldName)
                .HasMaxLength(100);

            builder.Property(x => x.OldValue)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.NewValue)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.Remarks)
                .HasMaxLength(1000);

            builder.HasIndex(x => x.SupportTicketId);

            builder.HasOne(x => x.SupportTicket)
                .WithMany(x => x.History)
                .HasForeignKey(x => x.SupportTicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ChangedByUser)
                .WithMany()
                .HasForeignKey(x => x.ChangedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
