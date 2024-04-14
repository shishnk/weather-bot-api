using DatabaseApp.Domain.Models;
using DatabaseApp.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DatabaseApp.Tests.BasicTestContext;

public abstract class IntegrationTestBase : IClassFixture<IntegrationWebAppFactory>, IAsyncLifetime
{
    private readonly IntegrationWebAppFactory _factory;
    protected readonly ISender _sender;
    protected readonly IUnitOfWork _unitOfWork;

    protected IntegrationTestBase(IntegrationWebAppFactory factory)
    {
        _factory = factory;
        var scope = factory.Services.CreateScope();

        _sender = scope.ServiceProvider.GetRequiredService<ISender>();
        _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _factory.ResetDatabaseAsync();

    protected async Task AddUsersToDatabase(params User[] users)
    {
        foreach (var user in users)
        {
            await _unitOfWork.UserRepository.AddAsync(user, CancellationToken.None);
        }

        await _unitOfWork.SaveDbChangesAsync(CancellationToken.None);
    }
    
    protected async Task AddWeatherSubscriptionsToDatabase(params UserWeatherSubscription[] weatherSubscriptions)
    {
        foreach (var weatherSubscription in weatherSubscriptions)
        {
            await _unitOfWork.UserWeatherSubscriptionRepository.AddAsync(weatherSubscription, CancellationToken.None);
        }

        await _unitOfWork.SaveDbChangesAsync(CancellationToken.None);
    }
}