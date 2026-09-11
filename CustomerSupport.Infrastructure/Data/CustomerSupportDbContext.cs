using CustomerSupport.Domain.Entities;
using CustomerSupport.Domain.Enums;
using CustomerSupport.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CustomerSupport.Infrastructure.Data
{
    public sealed class CustomerSupportDbContext : DbContext
    {
        public CustomerSupportDbContext(DbContextOptions<CustomerSupportDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles => Set<Role>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<TicketCategory> TicketCategories => Set<TicketCategory>();
        public DbSet<TicketPriority> TicketPriorities => Set<TicketPriority>();
        public DbSet<TicketStatus> TicketStatuses => Set<TicketStatus>();
        public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
        public DbSet<TicketComment> TicketComments => Set<TicketComment>();
        public DbSet<TicketAssignment> TicketAssignments => Set<TicketAssignment>();
        public DbSet<TicketAttachment> TicketAttachments => Set<TicketAttachment>();
        public DbSet<TicketHistory> TicketHistories => Set<TicketHistory>();
        public DbSet<KnowledgeBaseArticle> KnowledgeBaseArticles => Set<KnowledgeBaseArticle>();
        public DbSet<FAQ> FAQs => Set<FAQ>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply Entity Configurations
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new TicketCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new TicketPriorityConfiguration());
            modelBuilder.ApplyConfiguration(new TicketStatusConfiguration());
            modelBuilder.ApplyConfiguration(new SupportTicketConfiguration());
            modelBuilder.ApplyConfiguration(new TicketCommentConfiguration());
            modelBuilder.ApplyConfiguration(new TicketAssignmentConfiguration());
            modelBuilder.ApplyConfiguration(new TicketAttachmentConfiguration());
            modelBuilder.ApplyConfiguration(new TicketHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new KnowledgeBaseArticleConfiguration());
            modelBuilder.ApplyConfiguration(new FAQConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());

            // Seed Master Data
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc);

            // Roles
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = RoleType.Customer,
                    Name = nameof(RoleType.Customer),
                    Description = "Customer who creates and manages support tickets.",
                    IsActive = true,
                    CreatedAt = seedDate
                },
                new Role
                {
                    Id = RoleType.SupportExecutive,
                    Name = nameof(RoleType.SupportExecutive),
                    Description = "Support executive who handles and resolves customer support tickets.",
                    IsActive = true,
                    CreatedAt = seedDate
                },
                new Role
                {
                    Id = RoleType.Administrator,
                    Name = nameof(RoleType.Administrator),
                    Description = "Administrator who manages the Customer Support System.",
                    IsActive = true,
                    CreatedAt = seedDate
                }
            );

            // Ticket Priorities
            modelBuilder.Entity<TicketPriority>().HasData(
                new TicketPriority
                {
                    Id = TicketPriorityType.Low,
                    Name = nameof(TicketPriorityType.Low),
                    Description = "Low-priority issue with minimal business impact.",
                    SortOrder = 1,
                    IsActive = true,
                    CreatedAt = seedDate
                },
                new TicketPriority
                {
                    Id = TicketPriorityType.Medium,
                    Name = nameof(TicketPriorityType.Medium),
                    Description = "Normal support issue requiring timely attention.",
                    SortOrder = 2,
                    IsActive = true,
                    CreatedAt = seedDate
                },
                new TicketPriority
                {
                    Id = TicketPriorityType.High,
                    Name = nameof(TicketPriorityType.High),
                    Description = "Important issue with significant business impact.",
                    SortOrder = 3,
                    IsActive = true,
                    CreatedAt = seedDate
                },
                new TicketPriority
                {
                    Id = TicketPriorityType.Critical,
                    Name = nameof(TicketPriorityType.Critical),
                    Description = "Critical issue requiring immediate attention.",
                    SortOrder = 4,
                    IsActive = true,
                    CreatedAt = seedDate
                }
            );

            // Ticket Statuses
            modelBuilder.Entity<TicketStatus>().HasData(
                new TicketStatus
                {
                    Id = TicketStatusType.Open,
                    Name = nameof(TicketStatusType.Open),
                    Description = "The ticket has been created and is awaiting action.",
                    SortOrder = 1,
                    IsActive = true,
                    CreatedAt = seedDate
                },
                new TicketStatus
                {
                    Id = TicketStatusType.Assigned,
                    Name = nameof(TicketStatusType.Assigned),
                    Description = "The ticket has been assigned to a support executive.",
                    SortOrder = 2,
                    IsActive = true,
                    CreatedAt = seedDate
                },
                new TicketStatus
                {
                    Id = TicketStatusType.InProgress,
                    Name = nameof(TicketStatusType.InProgress),
                    Description = "The support team is currently working on the ticket.",
                    SortOrder = 3,
                    IsActive = true,
                    CreatedAt = seedDate
                },
                new TicketStatus
                {
                    Id = TicketStatusType.Resolved,
                    Name = nameof(TicketStatusType.Resolved),
                    Description = "The reported issue has been resolved.",
                    SortOrder = 4,
                    IsActive = true,
                    CreatedAt = seedDate
                },
                new TicketStatus
                {
                    Id = TicketStatusType.Closed,
                    Name = nameof(TicketStatusType.Closed),
                    Description = "The support ticket has been completed and closed.",
                    SortOrder = 5,
                    IsActive = true,
                    CreatedAt = seedDate
                }
            );

            // Ticket Categories
            modelBuilder.Entity<TicketCategory>().HasData(
                new TicketCategory
                {
                    Id = 1,
                    Name = "Login Issue",
                    Description = "Problems related to login, password, or authentication.",
                    IsActive = true,
                    CreatedAt = seedDate
                },
                new TicketCategory
                {
                    Id = 2,
                    Name = "Payment Issue",
                    Description = "Problems related to payments or transactions.",
                    IsActive = true,
                    CreatedAt = seedDate
                },
                new TicketCategory
                {
                    Id = 3,
                    Name = "Technical Issue",
                    Description = "Technical problems related to a product or service.",
                    IsActive = true,
                    CreatedAt = seedDate
                },
                new TicketCategory
                {
                    Id = 4,
                    Name = "Billing Issue",
                    Description = "Problems related to billing, invoices, or charges.",
                    IsActive = true,
                    CreatedAt = seedDate
                },
                new TicketCategory
                {
                    Id = 5,
                    Name = "Account Issue",
                    Description = "Problems related to the customer's account.",
                    IsActive = true,
                    CreatedAt = seedDate
                },
                new TicketCategory
                {
                    Id = 6,
                    Name = "Feature Request",
                    Description = "Requests for new features or improvements.",
                    IsActive = true,
                    CreatedAt = seedDate
                },
                new TicketCategory
                {
                    Id = 7,
                    Name = "General Query",
                    Description = "General customer questions.",
                    IsActive = true,
                    CreatedAt = seedDate
                }
            );
        }
    }
}
