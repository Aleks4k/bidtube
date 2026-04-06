using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Domain.Entities
{
    public class BidHistory
    {
        public int ID { get; set; }
        public int bid_id { get; set; }
        public int user_id { get; set; }
        public User? User { get; set; }
        public int auction_id { get; set; }
        public double amount { get; set; }
        public DateTime date_of_bid { get; set; }
        public int auction_history_id { get; set; }
        public AuctionHistory? Auction { get; set; }
    }
}
