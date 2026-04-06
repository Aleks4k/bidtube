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
    public class UserCreateCommandValidator : AbstractValidator<UserCreateCommand>
    {
        public UserCreateCommandValidator(IUser userRepository)
        {
            RuleFor(x => x.User).SetValidator(new UserCreateDtoValidator(userRepository));
        }
    }
    public class UserCreateCommand : IRequest<UserDetailsDto>
    {
        public required UserCreateDto User { get; set; }
        public UserCreateCommand(){}
        public class UserCreateCommandHandler : IRequestHandler<UserCreateCommand, UserDetailsDto>
        {
            private readonly IJwtService _jwtService;
            private readonly IUser _userRepo;
            public UserCreateCommandHandler(IJwtService jwtService, IUser userRepository)
            {
                _jwtService = jwtService;
                _userRepo = userRepository;
            }
            public async Task<UserDetailsDto> Handle(UserCreateCommand request, CancellationToken cancellationToken)
            {
                var user = await _userRepo.RegisterUser(request.User, cancellationToken);
                user.firstTime = false;
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
