namespace TelegramBotApp.Messaging.IntegrationResponseContext.IntegrationResponses;

public class UniversalResponse(string message) : IResponseMessage
{
    public string Message => message;
    public bool IsEmpty => string.IsNullOrEmpty(message);
    
    public static UniversalResponse Empty => new(string.Empty);
}