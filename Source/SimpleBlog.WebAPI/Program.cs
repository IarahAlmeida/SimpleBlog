using SimpleBlog.WebAPI.Extensions;

namespace SimpleBlog.WebAPI;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).ConfigureWebApplicationBuilder();

        var app = builder.Build().ConfigureWebApplication();

        app.Run();
    }
}
