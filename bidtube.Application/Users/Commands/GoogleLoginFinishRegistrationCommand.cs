using bidtube.Application.Common.Contracts;
using bidtube.Application.Users.Contracts;
using bidtube.Application.Users.DTO;
using bidtube.Application.Users.Validators;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace bidtube.Application.Users.Commands
{
    public class GoogleLoginFinishRegistrationCommandValidator : AbstractValidator<GoogleLoginFinishRegistrationCommand>
    {
        public GoogleLoginFinishRegistrationCommandValidator(IUser userRepository, IJwtService jwtService, IHttpContextAccessor httpContextAccessor)
        {
            RuleFor(x => x.User).SetValidator(new GoogleLoginFinishRegistrationDtoValidator(userRepository, jwtService, httpContextAccessor));
        }
    }
    public class GoogleLoginFinishRegistrationCommand : IRequest<Unit>
    {
        public required GoogleLoginFinishRegistrationDto User { get; set; }
        public GoogleLoginFinishRegistrationCommand() { }
        public class GoogleLoginFinishRegistrationCommandHandler : IRequestHandler<GoogleLoginFinishRegistrationCommand, Unit>
        {
            private readonly IUser _userRepo;
            public GoogleLoginFinishRegistrationCommandHandler(IUser userRepository)
            {
                _userRepo = userRepository;
            }
            public async Task<Unit> Handle(GoogleLoginFinishRegistrationCommand request, CancellationToken cancellationToken)
            {
                await _userRepo.UpdateGoogleUserRegistrationData(request.User);
                return Unit.Value;
            }
        }
    }
}
