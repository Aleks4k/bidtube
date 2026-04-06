using bidtube.Application.Bids.Contracts;
using bidtube.Application.Bids.DTO;
using bidtube.Application.Common.Contracts;
using bidtube.Application.Common.DTO;
using bidtube.Application.Users.Contracts;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace bidtube.Application.Bids.Validators
{
    public class AddBidDtoValidatior: AbstractValidator<AddBidDto>
    {
        private readonly IUser _userRepository;
        private readonly IBid _bidRepository;
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string checkBidObject_errorMessage = string.Empty;
        public AddBidDtoValidatior(IUser userRepository, IBid bidRepository, IJwtService jwtService, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _bidRepository = bidRepository;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
            RuleFor(x => x.mail).NotEmpty().WithMessage("Mail address is required.").MaximumLength(255).WithMessage("Mail address is too long.").EmailAddress().WithMessage("Mail adress is not in valid format.").MustAsync((x, cancellation) => checkMail(x)).WithMessage("You are not allowed to make bid.").MustAsync((x, cancellation) => isMailNotRegistered(x)).WithMessage("This mail is not registered.");
            RuleFor(x => x.auction_id).GreaterThanOrEqualTo(1).WithMessage("Invalid auction ID."); //Ostatak proveravamo na validatoru objekta.
            RuleFor(x => x.amount).NotEmpty().WithMessage("Offer amount is required.").GreaterThanOrEqualTo(0.50).WithMessage("Offer amount needs to be at least 0,50€.").LessThanOrEqualTo(1000000000).WithMessage("Offer amount cannot exceed 1,000,000,000€.");
            RuleFor(x => x).MustAsync(async (x, cancellation) =>
            {
                var result = await validateBidObject(x);
                if (!result.IsValid)
                {
                    checkBidObject_errorMessage = result.ErrorMessage;
                }
                return result.IsValid;
            }).WithMessage(x => checkBidObject_errorMessage);
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
        private async Task<ValidationResult> validateBidObject(AddBidDto bid)
        {
            var authorizationHeader = _httpContextAccessor.HttpContext!.Request.Headers["Authorization"].FirstOrDefault();
            var token = authorizationHeader?.Substring("Bearer ".Length).Trim();
            var user_id = await _jwtService.GetUserIdFromAccessToken(token!);
            if(user_id == 0)
            {
                throw new UnauthorizedAccessException("Access token is not valid.");
            }
            return await _bidRepository.ValidateBidObject(bid, user_id);
        }
    }
}
