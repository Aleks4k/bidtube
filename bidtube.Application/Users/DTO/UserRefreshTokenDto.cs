using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Users.DTO
{
    public class UserRefreshTokenDto
    {
        public string token { get; set; } = string.Empty;
        public string mail { get; set; } = string.Empty;
    }
}
