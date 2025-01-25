using SimpleBlog.Application;
using SimpleBlog.Infrastructure;

namespace SimpleBlog.WebAPI.Extensions;

internal static class WebApplicationBuilderExtension
{
    internal static WebApplicationBuilder ConfigureWebApplicationBuilder(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi();

        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.AddAntiforgery();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        return builder;
    }
}
