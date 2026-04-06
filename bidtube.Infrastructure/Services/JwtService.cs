using bidtube.Application.Common.Contracts;
using bidtube.Infrastructure.Settings;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;
        public JwtService(JwtSettings jwtSettings) {
            _jwtSettings = jwtSettings;
        }
        public async Task<int> GetUserIdFromRefreshToken(string token)
        {
            var jsonWebTokenHandler = new JsonWebTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.RefreshTokenKey)),
                ClockSkew = TimeSpan.Zero
            };
            var tokenValidationResult = await jsonWebTokenHandler.ValidateTokenAsync(token, validationParameters);
            if (!tokenValidationResult.IsValid)
            {
                return 0;
            } 
            else
            {
                if (tokenValidationResult.Claims.TryGetValue(ClaimTypes.NameIdentifier, out var userIdObj))
                {
                    int user_id = 0;
                    if (int.TryParse(userIdObj.ToString(), out user_id))
                    {
                        return user_id;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }
        public async Task<int> GetUserIdFromAccessToken(string token)
        {
            var jsonWebTokenHandler = new JsonWebTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.AccessTokenKey)),
                ClockSkew = TimeSpan.Zero
            };
            var tokenValidationResult = await jsonWebTokenHandler.ValidateTokenAsync(token, validationParameters);
            if (!tokenValidationResult.IsValid)
            {
                return 0;
            }
            else
            {
                if (tokenValidationResult.Claims.TryGetValue(ClaimTypes.NameIdentifier, out var userIdObj))
                {
                    int user_id = 0;
                    if (int.TryParse(userIdObj.ToString(), out user_id))
                    {
                        return user_id;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }
        public async Task<bool> ValidateRefreshToken(string token, string mail)
        {
            var jsonWebTokenHandler = new JsonWebTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.RefreshTokenKey)),
                ClockSkew = TimeSpan.Zero
            };
            var tokenValidationResult = await jsonWebTokenHandler.ValidateTokenAsync(token, validationParameters);
            if (!tokenValidationResult.IsValid)
            {
                return false;
            } 
            else
            {
                if (tokenValidationResult.Claims.TryGetValue(ClaimTypes.Email, out var mail_obj))
                {
                    var mail_token = mail_obj.ToString();
                    if (mail_token != null && mail_token.Equals(mail))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        public async Task<bool> ValidateAccessToken(string token, string mail)
        {
            var jsonWebTokenHandler = new JsonWebTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.AccessTokenKey)),
                ClockSkew = TimeSpan.Zero
            };
            var tokenValidationResult = await jsonWebTokenHandler.ValidateTokenAsync(token, validationParameters);
            if (!tokenValidationResult.IsValid)
            {
                return false;
            }
            else
            {
                if (tokenValidationResult.Claims.TryGetValue(ClaimTypes.Email, out var mail_obj))
                {
                    var mail_token = mail_obj.ToString();
                    if (mail_token != null && mail_token.Equals(mail))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else //Znači da token nekako ne sadrži IP a to nije moguće.
                {
                    return false;
                }
            }
        }
        public string GenerateAccessToken(List<Claim> claims)
        {
            var jsonWebTokenHandler = new JsonWebTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.AccessTokenKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMilliseconds(_jwtSettings.AccessTokenTTL),
                SigningCredentials = creds
            };
            jsonWebTokenHandler.SetDefaultTimesOnTokenCreation = false;
            var token = jsonWebTokenHandler.CreateToken(tokenDescriptor);
            return token;
        }
        public string GenerateRefreshToken(List<Claim> claims)
        {
            var jsonWebTokenHandler = new JsonWebTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.RefreshTokenKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenTTL),
                SigningCredentials = creds
            };
            jsonWebTokenHandler.SetDefaultTimesOnTokenCreation = false;
            var token = jsonWebTokenHandler.CreateToken(tokenDescriptor);
            return token;
        }
    }
}
