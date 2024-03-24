using DatabaseApp.Domain.Repositories;
using MapsterMapper;
using MediatR;

namespace DatabaseApp.Application.UserWeatherSubscriptions.Queries.GetWeatherSubscriptions;

// ReSharper disable once UnusedType.Global
public class GetUserWeatherSubscriptionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetUserWeatherSubscriptionsQuery, List<UserWeatherSubscriptionDto>>
{
    public async Task<List<UserWeatherSubscriptionDto>> Handle(GetUserWeatherSubscriptionsQuery request,
        CancellationToken cancellationToken) =>
        mapper.From(
                await unitOfWork.UserWeatherSubscriptionRepository.GetAllByUserId(request.UserId, cancellationToken))
            .AdaptToType<List<UserWeatherSubscriptionDto>>();
}