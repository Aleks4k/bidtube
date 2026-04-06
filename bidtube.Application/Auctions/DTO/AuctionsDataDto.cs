using bidtube.Application.Bids.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Auctions.DTO
{
    public class AuctionsDataDto
    {
        public int ID { get; set; }
        public string username { get; set; } = string.Empty;
        public double average_rating { get; set; }
        public int total_reviews { get; set; }
        public int category_id { get; set; }
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public double startPrice { get; set; }
        public DateTime date_of_auction { get; set; }
        public DateTime date_of_expiration { get; set; }
        public List<AuctionImageDto> images { get; set;} = new List<AuctionImageDto>();
        public BidDetailsDto? top_bid { get; set; } //null ako ga nema.
    }
}
