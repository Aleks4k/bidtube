using bidtube.Application.Common.Contracts;
using bidtube.Application.Notifications.DTO;
using MediatR;
using Microsoft.AspNetCore.Http;
using INotification = bidtube.Application.Notifications.Contracts.INotification;

namespace bidtube.Application.Notifications.Queries
{
    public class GetNotificationsQuery: IRequest<List<NotificationDto>>
    {
        public required GetNotificationsQueryDto Notification { get; set; } //Za sad ga ne koristimo trebace nam.
        public GetNotificationsQuery()
        {
            
        }
        public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, List<NotificationDto>>
        {
            private readonly IJwtService _jwtService;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly INotification _notificationRepositroy;
            public GetNotificationsQueryHandler(IJwtService jwtService, IHttpContextAccessor httpContextAccessor, INotification notificationRepository)
            {
                _jwtService = jwtService;
                _httpContextAccessor = httpContextAccessor;
                _notificationRepositroy = notificationRepository;
            }
            public async Task<List<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
            {
                var authorizationHeader = _httpContextAccessor.HttpContext!.Request.Headers["Authorization"].FirstOrDefault();
                var token = authorizationHeader?.Substring("Bearer ".Length).Trim();
                var user_id = await _jwtService.GetUserIdFromAccessToken(token!);
                if (user_id == 0) //Ne postoji korisnik.
                    return new List<NotificationDto>();
                var response = await _notificationRepositroy.GetNotifications(user_id, request.Notification);
                return response;
            }
        }
    }
}
