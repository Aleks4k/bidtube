using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Domain.Entities
{
    public class Auction
    {
        public int ID { get; set; }
        public int user_id { get; set; }
        public User? User { get; set; }
        public int category_id { get; set; }
        public Category? Category { get; set; }
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public double startPrice { get; set; }
        public DateTime date_of_auction { get; set; }
        public DateTime date_of_expiration { get; set; }
        public ICollection<AuctionImage> Images { get; set; } = new List<AuctionImage>();
        public ICollection<Bid> Bids { get; set; } = new List<Bid>();
    }
}
