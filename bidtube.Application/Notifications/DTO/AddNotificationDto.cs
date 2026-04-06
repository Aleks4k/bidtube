using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Notifications.DTO
{
    public class AddNotificationDto
    {
        public int user_id { get; set; }
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string? action { get; set; }
    }
}
