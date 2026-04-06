using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Notifications.DTO
{
    public class GetNotificationsQueryDto
    {
        public int id { get; set; }
        public bool findAll { get; set; }
    }
}
