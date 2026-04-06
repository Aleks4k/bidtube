using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Users.DTO
{
    public class UpdateUserPasswordDto
    {
        public string mail { get; set; } = string.Empty;
        public string current_password { get; set; } = string.Empty;
        public string new_password { get; set; } = string.Empty;
    }
}
