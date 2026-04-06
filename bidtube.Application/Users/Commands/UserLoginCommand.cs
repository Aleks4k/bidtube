using bidtube.Application.Common.Contracts;
using bidtube.Application.Users.Contracts;
using bidtube.Application.Users.DTO;
using bidtube.Application.Users.Validators;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace bidtube.Application.Users.Commands
{
    public class UserLoginCommandValidator : AbstractValidator<UserLoginCommand>
    {
        public UserLoginCommandValidator(IUser userRepository)
        {
            RuleFor(x => x.User).SetValidator(new UserLoginDtoValidator(userRepository));
        }
    }
    public class UserLoginCommand : IRequest<UserDetailsDto>
    {
        public required UserLoginDto User { get; set; }
        public UserLoginCommand() { }
        public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, UserDetailsDto>
        {
            private readonly IJwtService _jwtService;
            private readonly IUser _userRepo;
            public UserLoginCommandHandler(IJwtService jwtService, IUser userRepository)
            {
                _jwtService = jwtService;
                _userRepo = userRepository;
            }
            public async Task<UserDetailsDto> Handle(UserLoginCommand request, CancellationToken cancellationToken)
            {
                var user = await _userRepo.GetUserByEmail(request.User.mail);
                user!.firstTime = false;
                var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Email, user.mail),
                                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString())
                            };
                user.access = _jwtService.GenerateAccessToken(claims);
                user.refresh = _jwtService.GenerateRefreshToken(claims);
                return user;
            }
        }
    }
}
