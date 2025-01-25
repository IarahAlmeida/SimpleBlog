using Scalar.AspNetCore;
using SimpleBlog.WebAPI.Endpoints;

namespace SimpleBlog.WebAPI.Extensions;

internal static class WebApplicationExtension
{
    internal static WebApplication ConfigureWebApplication(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.MapOpenApi();

        if (app.Environment.IsDevelopment())
        {
            app.MapScalarApiReference();
        }

        app.MapPostsEndpoints();

        app.UseAntiforgery();

        app.UseCors();

        return app;
    }
}
