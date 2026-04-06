using bidtube.Application.Common.Contracts;
using bidtube.Application.Users.Contracts;
using bidtube.Application.Users.DTO;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace bidtube.Application.Users.Validators
{
    public class UpdateUserDataDtoValidator : AbstractValidator<UpdateUserDataDto>
    {
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUser _userRepository;
        public UpdateUserDataDtoValidator(IJwtService jwtService, IHttpContextAccessor httpContextAccessor, IUser userRepository)
        {
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            RuleFor(x => x.mail).NotEmpty().WithMessage("Mail address is required.").MaximumLength(255).WithMessage("Mail address is too long.").EmailAddress().WithMessage("Mail adress is not in valid format.").MustAsync((x, cancellation) => checkMail(x)).WithMessage("You are not allowed to update this user data.").MustAsync((x, cancellation) => isMailNotRegistered(x)).WithMessage("This mail is not registered.");
            RuleFor(x => x.username).NotEmpty().WithMessage("Username is required.").MinimumLength(6).WithMessage("Username must be between 6 and 24 characters.").MaximumLength(24).WithMessage("Username must be between 6 and 24 characters.").Matches("^[^@]*$").WithMessage("Username cannot contain letter '@'.");
            RuleFor(x => x.date_of_birth).Must(UserLegalAgeValidator.IsLegal).WithMessage("User must be at least 16 years old.");
            RuleFor(x => x.phone).MaximumLength(25).WithMessage("Phone number can't be longer than 25 characters.");
            RuleFor(x => x).MustAsync((x, cancellation) => isNickOrPhoneInUseOnDifferentAcc(x)).WithMessage("Username or phone number is already in use on different account.");
        }
        private async Task<bool> isNickOrPhoneInUseOnDifferentAcc(UpdateUserDataDto user)
        {
            var task = await _userRepository.IsNickOrPhoneInUseOnDifferentAcc(user);
            return !task; //Vrćamo suprotno jer ovde zapravo false znači da je validacija propala.
        }
        private async Task<bool> isMailNotRegistered(string mail)
        {
            //Ovde zapravo gledamo da li mejl NIJE registrovan i ako nije odbijamo request. Možda čak i nepotrebna provera ali nije na odmet.
            var task = await _userRepository.IsMailRegistered(mail);
            return task;
        }
        private async Task<bool> checkMail(string mail)
        {
            //Već znamo da ima auth zato što je prošao kroz middleware i filter.
            var authorizationHeader = _httpContextAccessor.HttpContext!.Request.Headers["Authorization"].FirstOrDefault();
            var token = authorizationHeader?.Substring("Bearer ".Length).Trim();
            return await _jwtService.ValidateAccessToken(token!, mail);
        }
    }
}
