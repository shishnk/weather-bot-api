﻿using DatabaseApp.Application.UserWeatherSubscriptions.Queries.GetWeatherSubscriptions;
using DatabaseApp.Domain.Models;
using Mapster;

namespace DatabaseApp.Application.Common.Mapping;

public class RegisterMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UserWeatherSubscription, UserWeatherSubscriptionDto>()
            .Map(dest => dest.Location, src => src.WeatherDescription.Location);
    }
}