using bidtube.Application.Users.Contracts;
using bidtube.Application.Users.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Users.Validators
{
    public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
    {
        private readonly IUser _userRepository;
        public UserCreateDtoValidator(IUser userRepository)
        {
            _userRepository = userRepository;
            RuleFor(x => x.mail).NotEmpty().WithMessage("Mail address is required.").MaximumLength(255).WithMessage("Mail address is too long.").EmailAddress().WithMessage("Mail adress is not in valid format.");
            RuleFor(x => x.username).NotEmpty().WithMessage("Username is required.").MinimumLength(6).WithMessage("Username must be between 6 and 24 characters.").MaximumLength(24).WithMessage("Username must be between 6 and 24 characters.").Matches("^[^@]*$").WithMessage("Username cannot contain letter '@'.");
            RuleFor(x => x.password).NotEmpty().WithMessage("Password is required.").Length(128).WithMessage("Password is not valid.");
            RuleFor(x => x.date_of_birth).Must(UserLegalAgeValidator.IsLegal).WithMessage("User must be at least 16 years old.");
            RuleFor(x => x.phone).MaximumLength(25).WithMessage("Phone number can't be longer than 25 characters.");
            RuleFor(x => x).MustAsync((x, cancellation) => isUserRegistered(x)).WithMessage("User is already registered.");
        }
        private async Task<bool> isUserRegistered(UserCreateDto userCreateDto)
        {
            var task = await _userRepository.IsUserRegistered(userCreateDto);
            return !task;
        }
    }
}
