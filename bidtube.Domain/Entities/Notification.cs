using bidtube.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Domain.Entities
{
    public class Notification
    {
        public int ID { get; set; }
        public int user_id { get; set; }
        public User? User { get; set; }
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string? action { get; set; }
        public byte status { get; set; }
        public DateTime date_of_notification { get; set; }
        public DateTime? date_of_read { get; set; }
    }
}
