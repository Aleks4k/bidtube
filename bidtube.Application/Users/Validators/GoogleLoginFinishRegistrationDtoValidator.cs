using bidtube.Application.Common.Contracts;
using bidtube.Application.Users.Contracts;
using bidtube.Application.Users.DTO;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Users.Validators
{
    public class GoogleLoginFinishRegistrationDtoValidator : AbstractValidator<GoogleLoginFinishRegistrationDto>
    {
        private readonly IUser _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GoogleLoginFinishRegistrationDtoValidator(IUser userRepository, IJwtService jwtService, IHttpContextAccessor httpContextAccessor)
        {
            _jwtService = jwtService;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            RuleFor(x => x.mail).NotEmpty().WithMessage("Mail address is required.").MaximumLength(255).WithMessage("Mail address is too long.").EmailAddress().WithMessage("Mail adress is not in valid format.").MustAsync((x, cancellation) => checkMail(x)).WithMessage("You are not allowed to update this user data.").MustAsync((x, cancellation) => isMailNotRegistered(x)).WithMessage("This mail is not registered.");
            RuleFor(x => x.username).NotEmpty().WithMessage("Username is required.").MinimumLength(6).WithMessage("Username must be between 6 and 24 characters.").MaximumLength(24).WithMessage("Username must be between 6 and 24 characters.").Matches("^[^@]*$").WithMessage("Username cannot contain letter '@'.").MustAsync((x, cancellation) => isUsernameRegistered(x)).WithMessage("This username is already registered.");
            RuleFor(x => x.password).NotEmpty().WithMessage("Password is required.").Length(128).WithMessage("Password is not valid.");
            RuleFor(x => x.date_of_birth).Must(UserLegalAgeValidator.IsLegal).WithMessage("User must be at least 16 years old.");
            RuleFor(x => x.phone).MaximumLength(25).WithMessage("Phone number can't be longer than 25 characters.").MustAsync((x, cancellation) => isPhoneRegistered(x)).WithMessage("This phone is already registered.");
            RuleFor(x => x).MustAsync((x, cancellation) => checkUserPassword(x)).WithMessage("You can't change password for this account.");
        }
        private async Task<bool> isPhoneRegistered(string? phone)
        {
            if (String.IsNullOrEmpty(phone)) //Vraćamo true jer user nije dužan da za vreme registracije šalje podatak o rođendanu.
                return true;
            var task = await _userRepository.IsPhoneRegistered(phone);
            return !task;
        }
        private async Task<bool> isUsernameRegistered(string username)
        {
            var task = await _userRepository.IsUsernameRegistered(username);
            return !task;
        }
        private async Task<bool> checkUserPassword(GoogleLoginFinishRegistrationDto user)
        {
            var password = await this._userRepository.GetUserPasswordByEmail(user.mail);
            if (!String.IsNullOrEmpty(password) && password.Contains("temppasswordnottrue"))
            {
                return true;
            } 
            else
            {
                return false;
            }
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
