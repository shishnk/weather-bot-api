﻿namespace DatabaseApp.Application.UserWeatherSubscriptions;

// ReSharper disable once ClassNeverInstantiated.Global
public class UserWeatherSubscriptionDto
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required string Location { get; set; }
    public int UserTelegramId { get; set; }
    public TimeSpan ResendInterval { get; set; }
}