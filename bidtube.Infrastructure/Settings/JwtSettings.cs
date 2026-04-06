using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Infrastructure.Settings
{
    public class JwtSettings
    {
        public string AccessTokenKey { get; set; } = string.Empty;
        public string RefreshTokenKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int AccessTokenTTL { get; set; }
        public int RefreshTokenTTL { get; set; }
    }
}
