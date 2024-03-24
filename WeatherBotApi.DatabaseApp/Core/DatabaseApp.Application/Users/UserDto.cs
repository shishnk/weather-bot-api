using System.Diagnostics.CodeAnalysis;

namespace DatabaseApp.Application.Users;

// ReSharper disable once ClassNeverInstantiated.Global
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class UserDto
{
    public required int TelegramId { get; set; }
    public required string Username { get; set; }
    public required string MobileNumber { get; set; }
    public DateTime RegisteredAt { get; set; }
}