namespace TelegramBotApp.Domain.Models;

public interface IResponseMessage
{
    string Name { get; }
    string Value { get; }
}

public class Kek : IResponseMessage
{
    public string Name => "Kek";
    public required string Value { get; init; }
}