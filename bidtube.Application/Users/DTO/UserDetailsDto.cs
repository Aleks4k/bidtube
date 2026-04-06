using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Users.DTO
{
    public class UserDetailsDto
    {
        public string username { get; set; } = string.Empty;
        public string mail { get; set; } = string.Empty;
        public string access { get; set; } = string.Empty;
        public string refresh { get; set; } = string.Empty;
        public bool firstTime { get; set; }
        public int ID { get; set; }
    }
}
