namespace DatabaseApp.Application.Users.Queries.GetUser;

public class UserDto
{
    public required string Username { get; set; }
    public required string MobileNumber { get; set; }
    public DateTime RegisteredAt { get; set; }
}