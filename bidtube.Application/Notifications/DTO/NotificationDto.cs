using bidtube.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Notifications.DTO
{
    public class NotificationDto
    {
        public int ID { get; set; }
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string? action { get; set; }
        public NotificationStatus status { get; set; }
        public DateTime date_of_notification { get; set; }
    }
}
