using System.Text.Json;
using Converters.JsonConverters;
using Microsoft.Extensions.DependencyInjection;
using WeatherApp.Application.Services;

namespace WeatherApp.Tests.ServiceFixtures;

// ReSharper disable once ClassNeverInstantiated.Global
public class WeatherServiceFixture : IDisposable
{
    private readonly IServiceScope _scope;
    private bool _disposed;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters = { new WeatherDescriptorJsonConverter() }
    };

    public WeatherServiceFixture()
    {
        var services = new ServiceCollection();

        services.AddTransient<IWeatherService, WeatherService>();
        services.AddHttpClient<IWeatherService, WeatherService>();
        services.AddSingleton(_jsonSerializerOptions);

        var serviceProvider = services.BuildServiceProvider();
        _scope = serviceProvider.CreateScope();
    }

    ~WeatherServiceFixture() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            // Free any other managed objects here.
            _scope.Dispose();
        }

        // Free any unmanaged objects here.
        _disposed = true;
    }

    public T GetService<T>() where T : notnull => _scope.ServiceProvider.GetRequiredService<T>();
}