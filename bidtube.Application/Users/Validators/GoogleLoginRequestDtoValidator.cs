using bidtube.Application.Common.Contracts;
using bidtube.Application.Users.DTO;
using FluentValidation;

namespace bidtube.Application.Users.Validators
{
    public class GoogleLoginRequestDtoValidator : AbstractValidator<GoogleLoginRequestDto>
    {
        private readonly IGoogleAuthService _googleAuthService;
        public GoogleLoginRequestDtoValidator(IGoogleAuthService googleAuthService)
        {
            _googleAuthService = googleAuthService;
            RuleFor(x => x.Token).NotEmpty().WithMessage("Provided token is not valid.").MustAsync((x, cancellation) => validateGoogleToken(x)).WithMessage("Google token is not valid.");
        }
        private async Task<bool> validateGoogleToken(string token)
        {
            return await _googleAuthService.ValidateGoogleToken(token);
        }
    }
}
