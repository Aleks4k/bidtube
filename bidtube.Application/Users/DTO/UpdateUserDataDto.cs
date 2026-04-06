using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Users.DTO
{
    public class UpdateUserDataDto
    {
        public string mail { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public DateTime? date_of_birth { get; set; }
        public string? phone { get; set; } = string.Empty;
        public bool msg_allowed_from_anyone { get; set; }
    }
}
