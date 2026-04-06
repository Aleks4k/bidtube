using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Common.Contracts
{
    public interface IJwtService
    {
        Task<bool> ValidateAccessToken(string token, string mail);
        Task<bool> ValidateRefreshToken(string token, string mail);
        string GenerateAccessToken(List<Claim> claims);
        string GenerateRefreshToken(List<Claim> claims);
        Task<int> GetUserIdFromRefreshToken(string token);
        Task<int> GetUserIdFromAccessToken(string token);
    }
}
