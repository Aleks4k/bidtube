using bidtube.Application.Users.Contracts;
using bidtube.Application.Users.DTO;
using FluentValidation;

namespace bidtube.Application.Users.Validators
{
    public class UserLoginDtoValidator : AbstractValidator<UserLoginDto>
    {
        private readonly IUser _userRepository;
        public UserLoginDtoValidator(IUser userRepository)
        {
            _userRepository = userRepository;
            RuleFor(x => x.mail).NotEmpty().WithMessage("Mail address is required.").MaximumLength(255).WithMessage("Mail address is too long.").EmailAddress().WithMessage("Mail adress is not in valid format.");
            RuleFor(x => x.password).NotEmpty().WithMessage("Password is required.").Length(128).WithMessage("Password is not valid.");
            RuleFor(x => x).MustAsync((x, cancellation) => isLoginCorrect(x)).WithMessage("Incorrect email or password. Please check your credentials and try again.");
        }
        private async Task<bool> isLoginCorrect(UserLoginDto user)
        {
            var result = await this._userRepository.IsLoginCorrect(user);
            return result;
        }
    }
}
