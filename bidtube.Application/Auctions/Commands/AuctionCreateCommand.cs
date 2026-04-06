using bidtube.Application.Auctions.Contracts;
using bidtube.Application.Auctions.DTO;
using bidtube.Application.Auctions.Validators;
using bidtube.Application.Categories.Contracts;
using bidtube.Application.Common.Contracts;
using bidtube.Application.Users.Contracts;
using bidtube.Domain.Entities;
using FluentValidation;
using Ganss.Xss;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace bidtube.Application.Auctions.Commands
{
    public class AuctionCreateCommandValidator : AbstractValidator<AuctionCreateCommand>
    {
        public AuctionCreateCommandValidator(IUser userRepository, ICategory categoryRepository, IAuction auctionRepository, IJwtService _jwtService, IHttpContextAccessor _httpContextAccessor)
        {
            RuleFor(x => x.Auction).SetValidator(new AddAuctionDtoValidatior(userRepository, categoryRepository, auctionRepository, _jwtService, _httpContextAccessor));
        }
    }
    public class AuctionCreateCommand : IRequest<Unit>
    {
        public required AddAuctionDto Auction { get;  set; }
        public AuctionCreateCommand(){}
        public class AuctionCreateCommandHandler : IRequestHandler<AuctionCreateCommand, Unit>
        {
            private readonly IAuction _auctionRepository;
            private readonly ICloudinaryService _cloudinaryService;
            private readonly IJwtService _jwtService;
            private readonly IHttpContextAccessor _httpContextAccessor;
            public AuctionCreateCommandHandler(IAuction auctionRepository, ICloudinaryService cloudinaryService, IJwtService jwtService, IHttpContextAccessor httpContextAccessor)
            {
                _auctionRepository = auctionRepository;
                _cloudinaryService = cloudinaryService;
                _jwtService = jwtService;
                _httpContextAccessor = httpContextAccessor;
            }
            public async Task<Unit> Handle(AuctionCreateCommand request, CancellationToken cancellationToken)
            {
                var auction = new Auction();
                var authorizationHeader = _httpContextAccessor.HttpContext!.Request.Headers["Authorization"].FirstOrDefault();
                var token = authorizationHeader?.Substring("Bearer ".Length).Trim();
                var user_id = await _jwtService.GetUserIdFromAccessToken(token!);
                if (user_id == 0)
                {
                    throw new UnauthorizedAccessException("Access token is not valid.");
                }
                auction.user_id = user_id;
                auction.category_id = request.Auction.category_id;
                auction.title = request.Auction.title;
                var sanitizer = new HtmlSanitizer();
                sanitizer.AllowedTags.Clear();
                sanitizer.AllowedTags.Add("p");
                sanitizer.AllowedTags.Add("strong");
                sanitizer.AllowedTags.Add("em");
                sanitizer.AllowedTags.Add("u");
                sanitizer.AllowedTags.Add("span");
                sanitizer.AllowedTags.Add("ul");
                sanitizer.AllowedTags.Add("li");
                sanitizer.AllowedTags.Add("ol");
                sanitizer.AllowedAttributes.Clear();
                sanitizer.AllowedAttributes.Add("data-list");
                sanitizer.AllowedAttributes.Add("class");
                sanitizer.AllowedClasses.Clear();
                sanitizer.AllowedClasses.Add("ql-ui");
                auction.description = sanitizer.Sanitize(request.Auction.description);
                auction.startPrice = request.Auction.starting_price;
                auction.date_of_auction = DateTime.UtcNow;
                auction.date_of_expiration = request.Auction.date_of_exp;
                for(int i = 0; i < request.Auction.Images.Count; i++)
                {
                    var image = request.Auction.Images[i];
                    var image_name = request.Auction.Image_names[i];
                    var response = await _cloudinaryService.UploadImage(image, image_name, cancellationToken);
                    var auction_image = new AuctionImage();
                    auction_image.alt_text = image_name;
                    auction_image.url = response;
                    auction.Images.Add(auction_image);
                }
                await _auctionRepository.AddAuction(auction, cancellationToken);
                return Unit.Value;
            }
        }
    }
}
