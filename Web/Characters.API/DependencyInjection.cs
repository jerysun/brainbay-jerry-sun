using Characters.Commons.Exceptions.Handler;

namespace Characters.API;

public static class DependencyInjection
{
    public static IServiceCollection AddAPIServices(this IServiceCollection services)
    {
        services.AddExceptionHandler<CustomExceptionHandler>();
        return services;
    }

    public static WebApplication UseAPIServices(this WebApplication app)
    {
        app.UseExceptionHandler(options => { });
        return app;
    }
}