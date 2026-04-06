using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Domain.Entities
{
    public class AuctionHistory
    {
        public int ID { get; set; }
        public int auction_id { get; set; }
        public int user_id { get; set; }
        public User? User { get; set; }
        public int? user_bought_id { get; set; }
        public User? Winner { get; set; }
        public int category_id { get; set; }
        public Category? Category { get; set; }
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public double startPrice { get; set; }
        public DateTime date_of_auction { get; set; }
        public DateTime date_of_expiration { get; set; }
        public ICollection<AuctionImageHistory> Images { get; set; } = new List<AuctionImageHistory>();
        public ICollection<BidHistory> Bids { get; set; } = new List<BidHistory>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
