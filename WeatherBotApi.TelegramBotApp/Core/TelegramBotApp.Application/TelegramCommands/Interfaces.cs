using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using FluentResults;
using TelegramBotApp.Application.Services;
using TelegramBotApp.Caching.Caching;
using TelegramBotApp.Domain.Models;
using TelegramBotApp.Messaging;

namespace TelegramBotApp.Application.TelegramCommands;

public interface ITelegramCommand
{
    string Command { get; }
    string Description { get; }
    IEnumerable<Argument<string>> Arguments { get; }

    Task<Result<string>> Execute(InvocationContext context,
        long telegramId,
        Func<string, Argument<string>> getArgument,
        IEventBus bus,
        ICacheService cacheService,
        IResendMessageService messageService,
        CancellationToken cancellationToken);
}

public interface ITelegramCommandValidator
{
    string Command { get; }

    ParseArgument<Result> GetValidator();
}