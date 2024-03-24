using DatabaseApp.Domain.Models;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Commands.DeleteUserWeatherSubscription;

public class DeleteUserWeatherSubscriptionCommand
{
    public required Location Location { get; set; }
}