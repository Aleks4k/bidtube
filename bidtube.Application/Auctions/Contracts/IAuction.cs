using bidtube.Application.Auctions.DTO;
using bidtube.Application.Bids.DTO;
using bidtube.Application.Common.DTO;
using bidtube.Application.Notifications.DTO;
using bidtube.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Auctions.Contracts
{
    public interface IAuction
    {
        Task<bool> ValidateImages(List<string> images);
        Task AddAuction(Auction auction, CancellationToken cancellationToken);
        Task<AuctionsReturnDto> GetAuctions(GetAuctionsQueryDto auction);
        Task<AuctionsBidCheckDataDto?> GetAuctionForBidCheckById(int auction_id);
        Task<List<AddNotificationDto>> EndAuctions();
    }
}
