using bidtube.Application.Bids.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Auctions.DTO
{
    public class AuctionsBidCheckDataDto
    {
        public int ID { get; set; }
        public int user_id { get; set; }
        public double startPrice { get; set; }
        public DateTime date_of_expiration { get; set; }
        public BidWithUserIDDto? top_bid { get; set; } //null ako ga nema.
    }
}
