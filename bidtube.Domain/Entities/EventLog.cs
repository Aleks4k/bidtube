using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Domain.Entities
{
    public class EventLog
    {
        public int ID { get; set; }
        public int? error_code { get; set; }
        public string? error_message { get; set; }
        public DateTime error_timestamp { get; set; }
    }
}
