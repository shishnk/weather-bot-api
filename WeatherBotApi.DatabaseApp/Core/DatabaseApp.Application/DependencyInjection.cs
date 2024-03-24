using DatabaseApp.Application.Common.Mapping;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace DatabaseApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var config = new TypeAdapterConfig();
        new RegisterMapper().Register(config);

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}