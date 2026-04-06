using bidtube.Application.Common.Contracts;
using bidtube.Application.Users.Contracts;
using bidtube.Application.Users.DTO;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace bidtube.Application.Users.Validators
{
    public class UserGetBasicEditDataMailValidator : AbstractValidator<UserGetBasicEditDataDto>
    {
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUser _userRepository;
        public UserGetBasicEditDataMailValidator(IJwtService jwtService, IHttpContextAccessor httpContextAccessor, IUser userRepository)
        {
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            RuleFor(x => x.mail).NotEmpty().WithMessage("Mail address is required.").MaximumLength(255).WithMessage("Mail address is too long.").EmailAddress().WithMessage("Mail adress is not in valid format.").MustAsync((x, cancellation) => checkMail(x)).WithMessage("You are not allowed to get this user data.").MustAsync((x, cancellation) => isMailNotRegistered(x)).WithMessage("This mail is not registered.");
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
