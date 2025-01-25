using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleBlog.Domain.Aggregates.PostAggregate;
using SimpleBlog.Domain.Common;
using SimpleBlog.Infrastructure.Persistence.Contexts;
using SimpleBlog.Infrastructure.Repositories;

namespace SimpleBlog.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var schema = configuration.GetRequiredSection("DbSchema").Value;

        services.AddDbContextFactory<ApplicationDbContext>(options => options.UseNpgsql(connectionString, x => x.MigrationsHistoryTable("__EFMigrationsHistory", schema ?? "public")), ServiceLifetime.Scoped);

        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString, x => x.MigrationsHistoryTable("__EFMigrationsHistory", schema ?? "public")));

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IPostRepository, PostRepository>();

        // NOTE: the block below is for development only, in a production environment a better approach should be to
        // run the migrations on the devops pipeline
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();
        }

        return services;
    }
}
