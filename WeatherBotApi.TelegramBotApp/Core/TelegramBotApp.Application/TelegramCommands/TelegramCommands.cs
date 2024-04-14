using System.CommandLine;
using System.CommandLine.Invocation;
using System.Composition;
using System.Text;
using FluentResults;
using TelegramBotApp.Application.Factories;
using TelegramBotApp.Caching.Caching;
using TelegramBotApp.Domain.Models;
using TelegramBotApp.Messaging;
using TelegramBotApp.Messaging.Common;
using TelegramBotApp.Messaging.IntegrationContext.UserSubscriptionEvents;
using TelegramBotApp.Messaging.IntegrationContext.WeatherForecastIntegrationEvents;
using TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponses;

// ReSharper disable UnusedType.Global

namespace TelegramBotApp.Application.TelegramCommands;

[Export(typeof(ITelegramCommand))]
[ExportMetadata(nameof(Command), "/help")]
[ExportMetadata(nameof(Description), "- get help")]
public class HelpTelegramCommand : ITelegramCommand
{
    public string Command => "/help";
    public string Description => "- get help";
    public IEnumerable<Argument<string>> Arguments => Enumerable.Empty<Argument<string>>();

    public Task<Result<string>> Execute(InvocationContext context,
        long telegramId,
        Func<string, Argument<string>> getArgument,
        IEventBus bus, ICacheService cacheService, IResendMessageService messageService,
        CancellationToken cancellationToken)
    {
        var message = new StringBuilder();
        message.AppendLine("Hello! I'm a weather bot. I can provide you with the current weather in your city.");
        message.AppendLine("You can use the following commands:");

        foreach (var command in TelegramCommandFactory.GetCommands())
        {
            message.AppendLine(command);
        }

        return Task.FromResult(Result.Ok(message.ToString()));
    }
}

[Export(typeof(ITelegramCommand))]
[ExportMetadata(nameof(Command), "/weather")]
[ExportMetadata(nameof(Description), "<city> - get the current weather in the specified city")]
public class WeatherTelegramCommand : ITelegramCommand
{
    public string Command => "/weather";
    public string Description => "<city> - get the current weather in the specified city";

    public IEnumerable<Argument<string>> Arguments
    {
        get { yield return new("city", "City name"); }
    }

    public async Task<Result<string>> Execute(InvocationContext context,
        long telegramId,
        Func<string, Argument<string>> getArgument,
        IEventBus bus, ICacheService cacheService, IResendMessageService messageService,
        CancellationToken cancellationToken)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var city = context.ParseResult.GetValueForArgument(getArgument("city"));
        var response = await bus.Publish(new WeatherForecastRequestIntegrationEvent(city), nameof(UniversalResponse),
            cts.Token); // TODO: fix cancellation token

        return !response.IsEmpty
            ? Result.Ok(response.Message)
            : Result.Fail("An error occurred while processing the request");
    }
}

[Export(typeof(ITelegramCommand))]
[ExportMetadata(nameof(Command), "/createSubscription")]
[ExportMetadata(nameof(Description), "<location> <resendInterval (example: 00:30)> - create a subscription")]
public class CreateSubscriptionTelegramCommand : ITelegramCommand
{
    public string Command => "/createSubscription";
    public string Description => "<location> <resendInterval (example: 00:30)> - create a subscription";

    public IEnumerable<Argument<string>> Arguments
    {
        get
        {
            yield return new("location", "Location");
            yield return new("resendInterval", "Resend interval");
        }
    }

    public async Task<Result<string>> Execute(InvocationContext context,
        long telegramId,
        Func<string, Argument<string>> getArgument,
        IEventBus bus, ICacheService cacheService,
        IResendMessageService messageService,
        CancellationToken cancellationToken)
    {
        var location = context.ParseResult.GetValueForArgument(getArgument("location"));
        var intervalMemory = context.ParseResult.GetValueForArgument(getArgument("resendInterval")).AsMemory();
        var colonIndex = intervalMemory.Span.IndexOf(':');

        // TODO: avoid code duplication, using constants
        if (colonIndex == -1)
        {
            return "Invalid format for resend interval. Please use the format hh:mm".ToResult();
        }

        var hoursMemory = intervalMemory[..colonIndex];
        var minutesMemory = intervalMemory[(colonIndex + 1)..];

        if (!int.TryParse(hoursMemory.Span, out var hours) || !int.TryParse(minutesMemory.Span, out var minutes))
        {
            return "Invalid format for resend interval. Please use the format hh:mm".ToResult();
        }

        var resendInterval = new TimeSpan(hours, minutes, 0);

        if (resendInterval < TimeSpan.FromMinutes(30)) // hardcode
        {
            return "Resend interval should be at least 30 minutes".ToResult(); // TODO: add validation, result fail
        }

        messageService.AddOrUpdateResendProcess(telegramId, location, resendInterval);

        var response = await bus.Publish(new CreatedUserSubscriptionIntegrationEvent
            {
                TelegramId = telegramId,
                ResendInterval = resendInterval,
                City = location
            },
            nameof(UniversalResponse), CancellationToken.None); // TODO: fix cancellation token

        return !response.IsEmpty
            ? Result.Ok(response.Message)
            : Result.Fail("An error occurred while processing the request");
    }
}

[Export(typeof(ITelegramCommand))]
[ExportMetadata(nameof(Command), "/getSubscriptions")]
[ExportMetadata(nameof(Description), "- get all subscriptions")]
public class GetUserWeatherSubscriptionsTelegramCommand : ITelegramCommand
{
    public string Command => "/getSubscriptions";
    public string Description => "- get all subscriptions";

    public IEnumerable<Argument<string>> Arguments => Enumerable.Empty<Argument<string>>();

    public async Task<Result<string>> Execute(InvocationContext context,
        long telegramId,
        Func<string, Argument<string>> getArgument,
        IEventBus bus, ICacheService cacheService, IResendMessageService messageService,
        CancellationToken cancellationToken)
    {
        var allSubscriptions =
            await cacheService.GetAsync<List<UserSubscriptionInfo>>("allSubscriptions",
                CancellationToken.None); // TODO: fix cancellation token

        if (allSubscriptions == null) return Result.Fail("Bad internal state");

        var message = new StringBuilder();
        var userSubscriptions = allSubscriptions.Where(s => s.TelegramId == telegramId).ToList();

        for (var i = 0; i < userSubscriptions.Count; i++)
        {
            var subscription = userSubscriptions[i];
            message.AppendLine(
                $"{i + 1}) Location: {subscription.Location}, resend interval: {subscription.ResendInterval.ToString(@"hh\:mm")}");
        }

        return message.Length > 0
            ? Result.Ok(message.ToString())
            : Result.Ok("No subscriptions found");
    }
}

[Export(typeof(ITelegramCommand))]
[ExportMetadata(nameof(Command), "/updateSubscription")]
[ExportMetadata(nameof(Description), "<location> <resendInterval (example: 00:30)> - update a subscription")]
public class UpdateSubscriptionTelegramCommand : ITelegramCommand
{
    public string Command => "/updateSubscription";
    public string Description => "<location> <resendInterval (example: 00:30)> - update a subscription";

    public IEnumerable<Argument<string>> Arguments
    {
        get
        {
            yield return new("location", "Location");
            yield return new("resendInterval", "Resend interval");
        }
    }

    public async Task<Result<string>> Execute(InvocationContext context,
        long telegramId,
        Func<string, Argument<string>> getArgument,
        IEventBus bus, ICacheService cacheService, IResendMessageService messageService,
        CancellationToken cancellationToken)
    {
        var location = context.ParseResult.GetValueForArgument(getArgument("location"));
        var intervalMemory = context.ParseResult.GetValueForArgument(getArgument("resendInterval")).AsMemory();
        var colonIndex = intervalMemory.Span.IndexOf(':');

        // TODO: avoid code duplication, using constants
        if (colonIndex == -1)
        {
            return "Invalid format for resend interval. Please use the format hh:mm".ToResult();
        }

        var hoursMemory = intervalMemory[..colonIndex];
        var minutesMemory = intervalMemory[(colonIndex + 1)..];

        if (!int.TryParse(hoursMemory.Span, out var hours) || !int.TryParse(minutesMemory.Span, out var minutes))
        {
            return "Invalid format for resend interval. Please use the format hh:mm".ToResult();
        }

        var resendInterval = new TimeSpan(hours, minutes, 0);

        if (resendInterval < TimeSpan.FromMinutes(30)) // hardcode
        {
            return "Resend interval should be at least 30 minutes".ToResult(); // TODO: add validation, result fail
        }

        messageService.AddOrUpdateResendProcess(telegramId, location, resendInterval);

        var response = await bus.Publish(new UpdatedUserSubscriptionIntegrationEvent
        {
            TelegramUserId = telegramId,
            Location = location,
            ResendInterval = resendInterval
        }, nameof(UniversalResponse), CancellationToken.None); // TODO: fix cancellation token

        return !response.IsEmpty
            ? Result.Ok(response.Message)
            : Result.Fail("An error occurred while processing the request");
    }
}

[Export(typeof(ITelegramCommand))]
[ExportMetadata(nameof(Command), "/deleteSubscription")]
[ExportMetadata(nameof(Description), "<location> - delete a subscription")]
public class DeleteSubscriptionTelegramCommand : ITelegramCommand
{
    public string Command => "/deleteSubscription";
    public string Description => "<location> - delete a subscription";

    public IEnumerable<Argument<string>> Arguments
    {
        get { yield return new("location", "Location"); }
    }

    public async Task<Result<string>> Execute(InvocationContext context,
        long telegramId,
        Func<string, Argument<string>> getArgument,
        IEventBus bus, ICacheService cacheService, IResendMessageService messageService,
        CancellationToken cancellationToken)
    {
        var location = context.ParseResult.GetValueForArgument(getArgument("location"));

        messageService.RemoveResendProcess(telegramId, location);

        var response = await bus.Publish(new DeletedUserSubscriptionIntegrationEvent
            {
                TelegramUserId = telegramId,
                Location = location
            },
            nameof(UniversalResponse), CancellationToken.None); // TODO: fix cancellation token

        return !response.IsEmpty
            ? Result.Ok(response.Message)
            : Result.Fail("An error occurred while processing the request");
    }
}

/*[Export(typeof(ITelegramCommand))] TODO: create command
[ExportMetadata(nameof(Command), "/deleteAllSubscriptions")]
[ExportMetadata(nameof(Description), "delete all subscriptions")]*/