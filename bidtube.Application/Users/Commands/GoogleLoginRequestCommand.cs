using bidtube.Application.Common.Contracts;
using bidtube.Application.Users.Contracts;
using bidtube.Application.Users.DTO;
using bidtube.Application.Users.Validators;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace bidtube.Application.Users.Commands
{
    public class GoogleLoginRequestCommandValidator : AbstractValidator<GoogleLoginRequestCommand>
    {
        public GoogleLoginRequestCommandValidator(IGoogleAuthService googleAuthService)
        {
            RuleFor(x => x.User).SetValidator(new GoogleLoginRequestDtoValidator(googleAuthService));
        }
    }
    public class GoogleLoginRequestCommand : IRequest<UserDetailsDto>
    {
        public required GoogleLoginRequestDto User { get; set; }
        public GoogleLoginRequestCommand() {}
        public class GoogleLoginRequestCommandHandler : IRequestHandler<GoogleLoginRequestCommand, UserDetailsDto>
        {
            private readonly IGoogleAuthService _googleAuthService;
            private readonly IJwtService _jwtService;
            private readonly IUser _userRepo;
            public GoogleLoginRequestCommandHandler(IGoogleAuthService googleAuthService, IJwtService jwtService, IUser userRepository)
            {
                _googleAuthService = googleAuthService;
                _jwtService = jwtService;
                _userRepo = userRepository;
            }
            public async Task<UserDetailsDto> Handle(GoogleLoginRequestCommand request, CancellationToken cancellationToken)
            {
                var payload = await _googleAuthService.GetGooglePayload(request.User.Token);
                UserCreateDto _user = new UserCreateDto
                {
                    mail = payload.Email,
                    username = payload.Email
                };
                UserDetailsDto? result;
                if (await _userRepo.IsUserRegistered(_user))
                {
                    result = await _userRepo.GetUserByEmail(_user.mail);
                    var password = await _userRepo.GetUserPasswordByEmail(result!.mail); //user neće biti null jer imamo IsUserRegistered.
                    if (password.Contains("temppasswordnottrue"))
                    {
                        result.firstTime = true; //Nije završio registraciju.
                    } 
                    else
                    {
                        result.firstTime = false;
                    }                  
                }
                else //Insert user.
                {
                    _user.password = "temppasswordnottrue";
                    result = await _userRepo.RegisterUser(_user, cancellationToken);
                    result.firstTime = true;
                }
                var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Email, result.mail),
                            new Claim(ClaimTypes.NameIdentifier, result.ID.ToString())
                        };
                result.access = _jwtService.GenerateAccessToken(claims);
                result.refresh = _jwtService.GenerateRefreshToken(claims);
                return result;
            }
        }
    }
}
