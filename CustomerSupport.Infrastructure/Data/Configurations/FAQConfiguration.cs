using CustomerSupport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupport.Infrastructure.Data.Configurations
{
    public sealed class FAQConfiguration : IEntityTypeConfiguration<FAQ>
    {
        public void Configure(EntityTypeBuilder<FAQ> builder)
        {
            builder.ToTable("FAQs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Question)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.Answer)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.HasIndex(x => x.ProductId);

            builder.HasIndex(x => x.IsActive);

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
