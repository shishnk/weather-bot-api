namespace TelegramBotApp.Messaging.IntegrationContext;

public abstract class IntegrationEvent
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public abstract string Name { get; }

    public void UpdateId(Guid id) => Id = id;
}