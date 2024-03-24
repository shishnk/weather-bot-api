using DatabaseApp.Application.Users;
using DatabaseApp.Application.Users.Queries.GetUser;
using DatabaseApp.Application.UserWeatherSubscriptions;
using DatabaseApp.Application.UserWeatherSubscriptions.Queries.GetWeatherSubscriptions;
using DatabaseApp.Domain.Models;
using Mapster;

namespace DatabaseApp.Application.Common.Mapping;

public class RegisterMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserWeatherSubscription, UserWeatherSubscriptionDto>()
            .Map(dest => dest.Location, src => src.Location.Value);
        config.NewConfig<User, UserDto>()
            .RequireDestinationMemberSource(true);
    }
}