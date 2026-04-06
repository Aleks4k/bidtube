using bidtube.Application.Common.Contracts;
using bidtube.Application.Users.DTO;
using bidtube.Domain.Entities;
using MediatR;
using Newtonsoft.Json;
using System.Security.Claims;

namespace bidtube.Application.Users.Queries
{
    public class UserRefreshTokenQuery : IRequest<string?>
    {
        public required UserRefreshTokenDto User { get; set; }
        public UserRefreshTokenQuery() { }
        public class UserRefreshTokenQueryHandler : IRequestHandler<UserRefreshTokenQuery, string?>
        {
            private readonly IJwtService _jwtService;
            public UserRefreshTokenQueryHandler(IJwtService jwtService)
            {
                _jwtService = jwtService;
            }
            public async Task<string?> Handle(UserRefreshTokenQuery request, CancellationToken cancellationToken)
            {
                var response = await _jwtService.ValidateRefreshToken(request.User.token, request.User.mail);
                if (!response)
                {
                    throw new UnauthorizedAccessException("Refresh token is not valid.");
                }
                else
                {
                    var user_id = await _jwtService.GetUserIdFromRefreshToken(request.User.token);
                    if(user_id == 0)
                    {
                        throw new UnauthorizedAccessException("Refresh token is not valid.");
                    }
                    var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Email, request.User.mail),
                                new Claim(ClaimTypes.NameIdentifier, user_id.ToString())
                            };
                    var token = _jwtService.GenerateAccessToken(claims);
                    return JsonConvert.SerializeObject(new { access = token });
                }
            }
        }
    }
}
