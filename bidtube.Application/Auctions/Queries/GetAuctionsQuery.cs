using bidtube.Application.Auctions.Contracts;
using bidtube.Application.Auctions.DTO;
using bidtube.Application.Auctions.Validators;
using bidtube.Application.Categories.Contracts;
using bidtube.Application.Common.Contracts;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Auctions.Queries
{
    public class GetAuctionsQueryValidator : AbstractValidator<GetAuctionsQuery>
    {
        public GetAuctionsQueryValidator(ICategory categoryRepository)
        {
            RuleFor(x => x.Auction).SetValidator(new GetAuctionsQueryDtoValidator(categoryRepository));
        }
    }
    public class GetAuctionsQuery: IRequest<AuctionsReturnDto>
    {
        public required GetAuctionsQueryDto Auction { get; set; }
        public GetAuctionsQuery() { }
        public class GetAuctionsQueryHandler : IRequestHandler<GetAuctionsQuery, AuctionsReturnDto>
        {
            private readonly IAuction _auctionRepository;
            private readonly IJwtService _jwtService;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IHubSender _hubSender;
            public GetAuctionsQueryHandler(IAuction auctionRepository, IJwtService jwtService, IHttpContextAccessor httpContextAccessor, IHubSender hubSender)
            {
                _auctionRepository = auctionRepository;
                _jwtService = jwtService;
                _httpContextAccessor = httpContextAccessor;
                _hubSender = hubSender;
            }
            public async Task<AuctionsReturnDto> Handle(GetAuctionsQuery request, CancellationToken cancellationToken)
            {
                var response = await _auctionRepository.GetAuctions(request.Auction);
                var authorizationHeader = _httpContextAccessor.HttpContext!.Request.Headers["Authorization"].FirstOrDefault();
                var token = authorizationHeader?.Substring("Bearer ".Length).Trim();
                var user_id = await _jwtService.GetUserIdFromAccessToken(token!);
                if(user_id != 0)
                {
                    //Grupe.
                    await _hubSender.RemoveUserFromAuctionGroups(user_id);
                    await _hubSender.JoinUserToAuctionGroups(user_id, response.auctions.Select(x => string.Concat("a_", x.ID)).ToList());
                }
                return response;
            }
        }
    }
}
