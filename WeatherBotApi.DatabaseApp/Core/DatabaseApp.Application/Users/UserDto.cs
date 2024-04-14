namespace DatabaseApp.Application.Users;

// ReSharper disable once ClassNeverInstantiated.Global
public class UserDto
{
    public required long TelegramId { get; set; }
    public required string Username { get; set; }
    public required string MobileNumber { get; set; }
    public DateTime RegisteredAt { get; set; }
}