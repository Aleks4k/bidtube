using bidtube.Application.Common.Contracts;
using bidtube.Application.Notifications.Contracts;
using bidtube.Application.Notifications.DTO;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INotification = bidtube.Application.Notifications.Contracts.INotification;

namespace bidtube.Application.Notifications.Queries
{
    public class GetUnreadCountQuery : IRequest<UnreadNotificationsDto>
    {
        public GetUnreadCountQuery()
        {
            
        }
        public class GetUnreadCountQueryHandler : IRequestHandler<GetUnreadCountQuery, UnreadNotificationsDto>
        {
            private readonly INotification _notificationRepository;
            private readonly IJwtService _jwtService;
            private readonly IHttpContextAccessor _httpContextAccessor;
            public GetUnreadCountQueryHandler(INotification notificationRepository, IJwtService jwtService, IHttpContextAccessor httpContextAccessor)
            {
                _notificationRepository = notificationRepository;
                _jwtService = jwtService;
                _httpContextAccessor = httpContextAccessor;
            }
            public async Task<UnreadNotificationsDto> Handle(GetUnreadCountQuery request, CancellationToken cancellationToken)
            {
                var authorizationHeader = _httpContextAccessor.HttpContext!.Request.Headers["Authorization"].FirstOrDefault();
                var token = authorizationHeader?.Substring("Bearer ".Length).Trim();
                var user_id = await _jwtService.GetUserIdFromAccessToken(token!);
                if (user_id == 0) //Ne postoji korisnik, ovo ne bi trebalo da se ikada desi.
                    return new UnreadNotificationsDto { count = 0 };
                var response = await _notificationRepository.GetUnreadCount(user_id);
                return response;
            }
        }
    }
}
