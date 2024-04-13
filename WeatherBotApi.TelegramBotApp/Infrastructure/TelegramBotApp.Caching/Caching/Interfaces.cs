namespace TelegramBotApp.Caching.Caching;

public interface ICacheService
{
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
    public Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class;
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}