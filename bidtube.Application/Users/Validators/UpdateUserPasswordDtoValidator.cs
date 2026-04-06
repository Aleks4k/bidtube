using bidtube.Application.Common.Contracts;
using bidtube.Application.Users.Contracts;
using bidtube.Application.Users.DTO;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Users.Validators
{
    public class UpdateUserPasswordDtoValidator : AbstractValidator<UpdateUserPasswordDto>
    {
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUser _userRepository;
        public UpdateUserPasswordDtoValidator(IJwtService jwtService, IHttpContextAccessor httpContextAccessor, IUser userRepository)
        {
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            RuleFor(x => x.mail).NotEmpty().WithMessage("Mail address is required.").MaximumLength(255).WithMessage("Mail address is too long.").EmailAddress().WithMessage("Mail adress is not in valid format.").MustAsync((x, cancellation) => checkMail(x)).WithMessage("You are not allowed to update this user data.").MustAsync((x, cancellation) => isMailNotRegistered(x)).WithMessage("This mail is not registered.");
            RuleFor(x => x.current_password).NotEmpty().WithMessage("Password is required.").Length(128).WithMessage("Password is not valid.");
            RuleFor(x => x.new_password).NotEmpty().WithMessage("Password is required.").Length(128).WithMessage("Password is not valid.");
            RuleFor(x => x).Must(CheckPasswords)
                .WithMessage("New password cannot be same as current one.")
                .MustAsync((x, cancellation) => isPasswordCorrect(x))
                .WithMessage("The provided current password does not match the actual password.");
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
        private bool CheckPasswords(UpdateUserPasswordDto user)
        {
            if (user.new_password.Equals(user.current_password))
            {
                return false;
            }
            return true;
        }
        private async Task<bool> isPasswordCorrect(UpdateUserPasswordDto user)
        {
            var password = await this._userRepository.GetUserPasswordByEmail(user.mail);
            if (password.Equals(user.current_password))
            {
                return true;
            }
            return false;
        }
    }
}
