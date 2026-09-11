using CustomerSupport.Infrastructure.Extensions;
namespace CustomerSupport.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var connectionString =
                builder.Configuration.GetConnectionString("CustomerSupportDBConnection")
                ?? throw new InvalidOperationException("CustomerSupportDBConnection is not configured.");

            builder.Services.AddInfrastructureServices(connectionString);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.MapControllers();

            app.Run();
        }
    }
}
