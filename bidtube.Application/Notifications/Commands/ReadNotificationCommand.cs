using bidtube.Application.Common.Contracts;
using bidtube.Application.Notifications.Contracts;
using bidtube.Application.Notifications.DTO;
using bidtube.Application.Notifications.Validators;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Notifications.Commands
{
    public class ReadNotificationCommandValidator : AbstractValidator<ReadNotificationCommand>
    {
        public ReadNotificationCommandValidator(IJwtService _jwtService, IHttpContextAccessor _httpContextAccessor, Contracts.INotification _notificationRepository)
        {
            RuleFor(x => x.Notification).SetValidator(new ReadNotificationDtoValidatior(_jwtService, _httpContextAccessor, _notificationRepository));
        }
    }
    public class ReadNotificationCommand : IRequest<Unit>
    {
        public required ReadNotificationDto Notification { get; set; }
        public ReadNotificationCommand()
        {
        }
        public class ReadNotificationCommandHandler : IRequestHandler<ReadNotificationCommand, Unit>
        {
            private readonly Contracts.INotification _notificationRepository;
            public ReadNotificationCommandHandler(Contracts.INotification notificationRepository) {
                _notificationRepository = notificationRepository;
            }
            public async Task<Unit> Handle(ReadNotificationCommand request, CancellationToken cancellationToken)
            {
                await _notificationRepository.MarkNotificationAsRead(request.Notification.Id);
                return Unit.Value;
            }
        }
    }
}
