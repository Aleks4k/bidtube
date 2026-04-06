using bidtube.Application.Common.Contracts;
using bidtube.Application.Notifications.Contracts;
using bidtube.Application.Notifications.DTO;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Notifications.Validators
{
    public class ReadNotificationDtoValidatior : AbstractValidator<ReadNotificationDto>
    {
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INotification _notifactionRepository;
        public ReadNotificationDtoValidatior(IJwtService jwtService, IHttpContextAccessor httpContextAccessor, INotification notificationRepository)
        {
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
            _notifactionRepository = notificationRepository;
            RuleFor(x => x.Id).NotEmpty().WithMessage("Invalid notification ID.").GreaterThan(0).WithMessage("Invalid notification ID.").MustAsync((x, cancellation) => isUserNotificationOwner(x)).WithMessage("Invalid notification ID.").MustAsync((x, cancellation) => isNotificationAlreadyMarkedAsRead(x)).WithMessage("Notification is already marked as read.");
        }
        private async Task<bool> isNotificationAlreadyMarkedAsRead(int notification_id)
        {
            var task = await _notifactionRepository.IsNotificationAlreadyMarkedAsRead(notification_id);
            return !task; //Moramo da vratimo negaciju jer ako je pročitana vraćamo true iz repo što znači da validator mora da vrati FALSE da bi validacija pala.
        }
        private async Task<bool> isUserNotificationOwner(int notification_id)
        {
            var authorizationHeader = _httpContextAccessor.HttpContext!.Request.Headers["Authorization"].FirstOrDefault();
            var token = authorizationHeader?.Substring("Bearer ".Length).Trim();
            var user_id = await _jwtService.GetUserIdFromAccessToken(token!);
            if (user_id == 0)
                return false;
            var task = await _notifactionRepository.IsUserNotificationOwner(notification_id, user_id);
            return task;
        }
    }
}
