namespace TelegramBotApp.Domain.Responses;

public class UniversalResponse(string message)
{
    public string Message => message;
}