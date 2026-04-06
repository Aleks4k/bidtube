using bidtube.Application.Common.Contracts;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Notifications.Commands
{
    public class ReadAllNotificationsCommand : IRequest<Unit>
    {
        public ReadAllNotificationsCommand() { }
        public class ReadAllNotificationsCommandHandler : IRequestHandler<ReadAllNotificationsCommand, Unit>
        {
            private readonly Contracts.INotification _notificationRepository;
            private readonly IJwtService _jwtService;
            private readonly IHttpContextAccessor _httpContextAccessor;
            public ReadAllNotificationsCommandHandler(Contracts.INotification notificationRepository, IJwtService jwtService, IHttpContextAccessor httpContextAccessor)
            {
                _notificationRepository = notificationRepository;
                _jwtService = jwtService;
                _httpContextAccessor = httpContextAccessor;
            }
            public async Task<Unit> Handle(ReadAllNotificationsCommand request, CancellationToken cancellationToken)
            {
                var authorizationHeader = _httpContextAccessor.HttpContext!.Request.Headers["Authorization"].FirstOrDefault();
                var token = authorizationHeader?.Substring("Bearer ".Length).Trim();
                var user_id = await _jwtService.GetUserIdFromAccessToken(token!);
                if(user_id != 0)
                {
                    await _notificationRepository.MarkAllNotificationsAsRead(user_id);
                }
                return Unit.Value;
            }
        }
    }
}
