using CustomerSupport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupport.Infrastructure.Data.Configurations
{
    public sealed class TicketAttachmentConfiguration
        : IEntityTypeConfiguration<TicketAttachment>
    {
        public void Configure(EntityTypeBuilder<TicketAttachment> builder)
        {
            builder.ToTable("TicketAttachments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OriginalFileName)
                .HasMaxLength(260)
                .IsRequired();

            builder.Property(x => x.StoredFileName)
                .HasMaxLength(260)
                .IsRequired();

            builder.Property(x => x.ContentType)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.RelativePath)
                .HasMaxLength(500)
                .IsRequired();

            builder.HasIndex(x => x.StoredFileName)
                .IsUnique();

            builder.HasIndex(x => x.SupportTicketId);

            builder.HasOne(x => x.SupportTicket)
                .WithMany(x => x.Attachments)
                .HasForeignKey(x => x.SupportTicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.UploadedByUser)
                .WithMany()
                .HasForeignKey(x => x.UploadedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
