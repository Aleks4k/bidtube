using bidtube.Application.Auctions.Contracts;
using bidtube.Application.Bids.Contracts;
using bidtube.Application.Bids.DTO;
using bidtube.Application.Common.Contracts;
using bidtube.Application.Common.DTO;
using bidtube.Application.Users.Contracts;
using bidtube.Domain.Data;
using bidtube.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Infrastructure.Repository
{
    public class BidRepository : IBid
    {
        private readonly AppDbContext _context;
        private readonly IAuction _auctionRepository;
        public BidRepository(AppDbContext context, IAuction auctionRepository)
        {
            _context = context;
            _auctionRepository = auctionRepository;
        }
        public async Task AddBid(Bid bid, CancellationToken cancellationToken)
        {
            await _context.Bids.AddAsync(bid, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task<ValidationResult> ValidateBidObject(AddBidDto bid, int user_id)
        {
            //amount - Provera da li je veće od poslednjeg offera koji je ponuđen za 10 ili da li je isto kao startingPrice.
            var auction = await _auctionRepository.GetAuctionForBidCheckById(bid.auction_id);
            if(auction == null)
            {
                return new ValidationResult {
                    ErrorMessage = "Invalid auction ID.",
                    IsValid = false
                };
            }
            if(auction.user_id == user_id)
            {
                return new ValidationResult
                {
                    ErrorMessage = "You cannot place a bid on your own auction.",
                    IsValid = false
                };
            }
            if(auction.date_of_expiration <= DateTime.UtcNow)
            {
                return new ValidationResult
                {
                    ErrorMessage = "Auction expired.",
                    IsValid = false
                };
            }
            if(auction.top_bid != null)
            {
                if(auction.top_bid.user_id == user_id)
                {
                    return new ValidationResult
                    {
                        ErrorMessage = "You are current leader of this auction.",
                        IsValid = false
                    };
                }
                if(auction.top_bid.amount + 10.0 > bid.amount)
                {
                    return new ValidationResult
                    {
                        ErrorMessage = string.Format("The minimum amount you need to offer to participate is {0:F2} €.", auction.top_bid.amount + 10.0),
                        IsValid = false
                    };
                }
            }
            else
            {
                if(auction.startPrice > bid.amount)
                {
                    return new ValidationResult
                    {
                        ErrorMessage = string.Format("The minimum amount you need to offer to participate is {0:F2} €.", auction.startPrice),
                        IsValid = false
                    };
                }
            }
            return new ValidationResult
            {
                ErrorMessage = string.Empty,
                IsValid = true
            };
        }
    }
}
