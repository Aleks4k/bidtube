using bidtube.Application.Auctions.DTO;
using bidtube.Domain.Entities;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Auctions.Mappers
{
    [Mapper]
    public static partial class AuctionMapper
    {
        public static partial AuctionImageDto ToDetailsDto(this AuctionImage entity);
        [MapProperty(nameof(Auction.ID), nameof(AuctionHistory.auction_id))]
        [MapperIgnoreSource(nameof(Auction.Bids))]
        [MapperIgnoreSource(nameof(Auction.Images))]
        [MapperIgnoreTarget(nameof(AuctionHistory.ID))]
        public static partial AuctionHistory ToHistoryEntityHelper(this Auction entity);
        [MapProperty(nameof(AuctionImage.ID), nameof(AuctionImageHistory.auction_image_id))]
        [MapperIgnoreSource(nameof(AuctionImage.Auction))]
        [MapperIgnoreTarget(nameof(AuctionImageHistory.Auction))]
        [MapperIgnoreTarget(nameof(AuctionImageHistory.ID))]
        public static partial AuctionImageHistory ToHistoryEntity(this AuctionImage entity);
        [MapProperty(nameof(Bid.ID), nameof(BidHistory.bid_id))]
        [MapperIgnoreSource(nameof(Bid.Auction))]
        [MapperIgnoreTarget(nameof(BidHistory.Auction))]
        [MapperIgnoreTarget(nameof(BidHistory.ID))]
        public static partial BidHistory ToHistoryEntity(this Bid entity);
        [UserMapping(Default = true)]
        public static AuctionHistory ToHistoryEntity(this Auction entity)
        {
            var response = ToHistoryEntityHelper(entity);
            response.Images = entity.Images.Select(x => x.ToHistoryEntity()).ToList();
            response.Bids = entity.Bids.Select(x => x.ToHistoryEntity()).ToList();
            return response;
        }
    }
}
