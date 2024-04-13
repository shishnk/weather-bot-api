using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DatabaseApp.Application.Common.Behaviors;

// ReSharper disable once UnusedType.Global
public class RequestLoggingBehavior<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : ResultBase
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        logger.LogInformation("Handling {RequestName}: {@Request}", requestName, request);

        var response = await next();

        if (response.IsSuccess)
        {
            logger.LogInformation("{Request} handled successfully", requestName);
        }
        else
        {
            foreach (var error in response.Errors)
            {
                logger.LogError("Error: {Error}", error.Message);
            }
        }

        return response;
    }
}