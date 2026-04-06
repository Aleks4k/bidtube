using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bidtube.Application.Notifications.DTO;
using bidtube.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace bidtube.Application.Notifications.Mappers
{
    [Mapper]
    public static partial class NotificationMapper
    {
        public static partial Notification ToEntity(this AddNotificationDto dto);
        public static partial NotificationDto ToDetailsDto(this Notification entity);
    }
}
