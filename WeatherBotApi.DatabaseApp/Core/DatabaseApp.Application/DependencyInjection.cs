using System.Reflection;
using DatabaseApp.Application.Common.Behaviors;
using DatabaseApp.Application.Common.Mapping;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
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
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}