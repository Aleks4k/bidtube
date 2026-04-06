using bidtube.Application.Auctions.Contracts;
using bidtube.Application.Auctions.DTO;
using bidtube.Application.Auctions.Validators;
using bidtube.Application.Categories.Contracts;
using bidtube.Application.Common.Contracts;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace bidtube.Application.Auctions.Queries
{
    public class GetAuctionsWithCategoriesQueryValidator : AbstractValidator<GetAuctionsWithCategoriesQuery>
    {
        public GetAuctionsWithCategoriesQueryValidator(ICategory categoryRepository)
        {
            RuleFor(x => x.Auction).SetValidator(new GetAuctionsQueryDtoValidator(categoryRepository));
        }
    }
    public class GetAuctionsWithCategoriesQuery : IRequest<AuctionsWithCategoriesResponseDto>
    {
        public required GetAuctionsQueryDto Auction { get; set; }
        public GetAuctionsWithCategoriesQuery() { }
        public class GetAuctionsWithCategoriesQueryHandler : IRequestHandler<GetAuctionsWithCategoriesQuery, AuctionsWithCategoriesResponseDto>
        {
            private readonly IAuction _auctionRepository;
            private readonly ICategory _categoryRepository;
            private readonly IJwtService _jwtService;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IHubSender _hubSender;
            public GetAuctionsWithCategoriesQueryHandler(IAuction auctionRepository, ICategory categoryRepository, IJwtService jwtService, IHttpContextAccessor httpContextAccessor, IHubSender hubSender)
            {
                _auctionRepository = auctionRepository;
                _categoryRepository = categoryRepository;
                _jwtService = jwtService;
                _httpContextAccessor = httpContextAccessor;
                _hubSender = hubSender;
            }
            public async Task<AuctionsWithCategoriesResponseDto> Handle(GetAuctionsWithCategoriesQuery request, CancellationToken cancellationToken)
            {
                var response = new AuctionsWithCategoriesResponseDto();
                response.auctions = await _auctionRepository.GetAuctions(request.Auction);
                response.categories = await _categoryRepository.GetAllCategories();
                var authorizationHeader = _httpContextAccessor.HttpContext!.Request.Headers["Authorization"].FirstOrDefault();
                var token = authorizationHeader?.Substring("Bearer ".Length).Trim();
                var user_id = await _jwtService.GetUserIdFromAccessToken(token!);
                if (user_id != 0)
                {
                    //Grupe.
                    await _hubSender.RemoveUserFromAuctionGroups(user_id);
                    await _hubSender.JoinUserToAuctionGroups(user_id, response.auctions.auctions.Select(x => string.Concat("a_", x.ID)).ToList());
                }
                return response;
            }
        }
    }
}
