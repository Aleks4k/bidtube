using bidtube.Application.Common.Contracts;
using bidtube.Application.Notifications.Contracts;
using bidtube.Application.Notifications.DTO;
using bidtube.Application.Notifications.Mappers;
using bidtube.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace bidtube.Infrastructure.Repository
{
    public class NotificationRepository : INotification
    {
        private readonly AppDbContext _context;
        private readonly IHubSender _hubSender;
        public NotificationRepository(AppDbContext context, IHubSender hubSender)
        {
            _context = context;
            _hubSender = hubSender;
        }
        public async Task AddNotifications(List<AddNotificationDto> notifications)
        {
            //Treba da namestimo da ako nam hubsender vrati da je user_id aktivan trenutno rokamo mu poruku i označimo je odmah kao delivered pre snimanja u bazu.
            var notificationsToInsert = notifications.Select(x => x.ToEntity()).ToList();
            foreach(var notification in notificationsToInsert)
            {
                notification.date_of_notification = DateTime.UtcNow;
                notification.status = (byte)Domain.Enums.NotificationStatus.@new;
                var connectionId = _hubSender.GetConnectionIDByUserId(notification.user_id);
                if (!string.IsNullOrEmpty(connectionId))
                {
                    await _hubSender.SendNotificationToUser(connectionId, JsonConvert.SerializeObject(notification.ToDetailsDto()));
                    notification.status = (byte) Domain.Enums.NotificationStatus.delivered;
                }
            }
            await _context.Notifications.AddRangeAsync(notificationsToInsert);
            await _context.SaveChangesAsync();
        }
        public async Task<List<NotificationDto>> GetNotifications(int user_id, GetNotificationsQueryDto filter)
        {
            var query = _context.Notifications.AsNoTracking()
                .Where(x => x.user_id == user_id);
            if (filter.id > 0)
            {
                query = query.Where(x => x.ID < filter.id);
            }
            if (!filter.findAll)
            {
                query = query.Where(x => x.status == 1 || x.status == 2);
            }
            var response = await query
                .OrderByDescending(x => x.ID)
                .Take(10)
                .Select(x => x.ToDetailsDto())
                .ToListAsync();
            return response;
        }
        public async Task<UnreadNotificationsDto> GetUnreadCount(int user_id)
        {
            //Dobijamo sve notifikacije kojima je status new ili delivered.
            var notifications = await _context.Notifications.AsNoTracking().Where(x => x.user_id == user_id && (x.status == 1 || x.status == 2)).CountAsync();
            return new UnreadNotificationsDto { count = notifications };
        }
        public async Task<bool> IsUserNotificationOwner(int notification_id, int user_id)
        {
            var result = await _context.Notifications.AsNoTracking().Where(x => x.ID == notification_id && x.user_id == user_id).FirstOrDefaultAsync();
            return result != null;
        }
        public async Task<bool> IsNotificationAlreadyMarkedAsRead(int notification_id)
        {
            var result = await _context.Notifications.AsNoTracking().Where(x => x.ID == notification_id).FirstOrDefaultAsync();
            if(result == null)
            {
                return false;
            }
            return result.status == 3; //Ako je read vraćamo true.
        }
        public async Task MarkNotificationAsRead(int notification_id)
        {
            var result = await _context.Notifications.Where(x => x.ID == notification_id).FirstOrDefaultAsync();
            if(result != null)
            {
                result.date_of_read = DateTime.UtcNow;
                result.status = 3;
                await _context.SaveChangesAsync();
            }
        }
        public async Task MarkAllNotificationsAsRead(int user_id)
        {
            await _context.Notifications
                .Where(x => x.user_id == user_id && x.status != 3)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(n => n.status, 3));
        }
    }
}
