﻿using FluentResults;
using MediatR;

namespace DatabaseApp.Application.Users.Commands.CreateUser;

public class CreateUserCommand : IRequest<int>, IRequest<Result<int>>
{
    public required int TelegramId { get; init; }
    public required string Username { get; init; }
    public required string MobileNumber { get; init; }
    public required DateTime RegisteredAt { get; init; }
}