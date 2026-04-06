using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Users.DTO
{
    public class UserCreateDto
    {
        public required string username { get; set; } = string.Empty;
        public string? password { get; set; }
        public required string mail { get; set; } = string.Empty;
        public DateTime? date_of_birth { get; set; }
        public string? phone { get; set; }
    }
}
