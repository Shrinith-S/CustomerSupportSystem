using CustomerSupport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerSupport.Infrastructure.Data.Configurations
{
    public sealed class KnowledgeBaseArticleConfiguration
        : IEntityTypeConfiguration<KnowledgeBaseArticle>
    {
        public void Configure(EntityTypeBuilder<KnowledgeBaseArticle> builder)
        {
            builder.ToTable("KnowledgeBaseArticles");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(x => x.Content)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasIndex(x => x.ProductId);

            builder.HasIndex(x => x.IsPublished);

            builder.HasIndex(x => x.IsActive);

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
