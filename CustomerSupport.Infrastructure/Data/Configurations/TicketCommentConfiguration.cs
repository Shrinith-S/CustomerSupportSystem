using CustomerSupport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupport.Infrastructure.Data.Configurations
{
    public sealed class TicketCommentConfiguration
        : IEntityTypeConfiguration<TicketComment>
    {
        public void Configure(EntityTypeBuilder<TicketComment> builder)
        {
            builder.ToTable("TicketComments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Content)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.HasIndex(x => x.SupportTicketId);

            builder.HasIndex(x => x.UserId);

            builder.HasOne(x => x.SupportTicket)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.SupportTicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
