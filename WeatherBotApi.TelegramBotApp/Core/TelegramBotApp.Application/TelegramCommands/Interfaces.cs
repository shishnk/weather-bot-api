using FluentResults;
using TelegramBotApp.Messaging;

namespace TelegramBotApp.Application.TelegramCommands;

public interface ITelegramCommand
{
    string Command { get; }
    
    Task<Result<string>> Execute(string command, string value, IEventBus bus, CancellationToken cancellationToken);
}