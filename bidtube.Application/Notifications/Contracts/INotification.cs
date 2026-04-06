using bidtube.Application.Notifications.DTO;
using bidtube.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Notifications.Contracts
{
    public interface INotification
    {
        Task AddNotifications(List<AddNotificationDto> notification);
        Task<UnreadNotificationsDto> GetUnreadCount(int user_id);
        Task<List<NotificationDto>> GetNotifications(int user_id, GetNotificationsQueryDto filter);
        Task<bool> IsUserNotificationOwner(int notification_id, int user_id);
        Task<bool> IsNotificationAlreadyMarkedAsRead(int notification_id);
        Task MarkNotificationAsRead(int notification_id);
        Task MarkAllNotificationsAsRead(int user_id);
    }
}
