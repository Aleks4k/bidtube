using bidtube.Application.Bids.Contracts;
using bidtube.Application.Bids.DTO;
using bidtube.Application.Bids.Validators;
using bidtube.Application.Common.Contracts;
using bidtube.Application.Users.Contracts;
using bidtube.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Bids.Commands
{
    public class BidCreateCommandValidator : AbstractValidator<BidCreateCommand>
    {
        public BidCreateCommandValidator(IUser userRepository, IBid bidRepository, IJwtService _jwtService, IHttpContextAccessor _httpContextAccessor)
        {
            RuleFor(x => x.Bid).SetValidator(new AddBidDtoValidatior(userRepository, bidRepository, _jwtService, _httpContextAccessor));
        }
    }
    public class BidCreateCommand : IRequest<Unit>
    {
        public required AddBidDto Bid { get; set; }
        public BidCreateCommand()
        {}
        public class BidCreateCommandHandler : IRequestHandler<BidCreateCommand, Unit>
        {
            private readonly IBid _bidRepository;
            private readonly IJwtService _jwtService;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IHubSender _hubSender;
            public BidCreateCommandHandler(IBid bidRepository, IJwtService jwtService, IHttpContextAccessor httpContextAccessor, IHubSender hubSender)
            {
                _bidRepository = bidRepository;
                _jwtService = jwtService;
                _httpContextAccessor = httpContextAccessor;
                _hubSender = hubSender;
            }
            public async Task<Unit> Handle(BidCreateCommand request, CancellationToken cancellationToken)
            {
                var bid = new Bid();
                var authorizationHeader = _httpContextAccessor.HttpContext!.Request.Headers["Authorization"].FirstOrDefault();
                var token = authorizationHeader?.Substring("Bearer ".Length).Trim();
                var user_id = await _jwtService.GetUserIdFromAccessToken(token!);
                if (user_id == 0)
                {
                    throw new UnauthorizedAccessException("Access token is not valid.");
                }
                bid.user_id = user_id;
                bid.auction_id = request.Bid.auction_id;
                bid.amount = request.Bid.amount;
                bid.date_of_bid = DateTime.UtcNow;
                await _bidRepository.AddBid(bid, cancellationToken);
                await _hubSender.SendAuctionUpdateToGroup(string.Concat("a_", bid.auction_id), JsonConvert.SerializeObject(new { bid.auction_id, bid.amount }));
                return Unit.Value;
            }
        }
    }
}
