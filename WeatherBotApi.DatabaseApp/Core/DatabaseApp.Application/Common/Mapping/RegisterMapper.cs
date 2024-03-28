using DatabaseApp.Application.Users;
using DatabaseApp.Application.UserWeatherSubscriptions;
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
            .Map(dest => dest.TelegramId, src => src.TelegramId)
            .Map(dest => dest.MobileNumber, src => src.Metadata.MobileNumber)
            .Map(dest => dest.Username, src => src.Metadata.Username)
            .Map(dest => dest.RegisteredAt, src => src.RegisteredAt);
    }
}