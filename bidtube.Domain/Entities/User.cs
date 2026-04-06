using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Domain.Entities
{
    public class User
    {
        public int ID { get; set; }
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string mail { get; set; } = string.Empty;
        public DateTime? date_of_birth { get; set; }
        public string? phone { get; set; }
        public bool msg_allowed_from_anyone { get; set; }
        public ICollection<Review> ReviewsGiven { get; set; } = new List<Review>();
        public ICollection<Review> ReviewsReceived { get; set; } = new List<Review>();
        public ICollection<Bid> UserBids { get; set; } = new List<Bid>();
        public ICollection<BidHistory> UserBids_history { get; set; } = new List<BidHistory>();
        public ICollection<Auction> Auctions { get; set; } = new List<Auction>();
        public ICollection<AuctionHistory> AuctionsWon { get; set; } = new List<AuctionHistory>();
        public ICollection<AuctionHistory> Auctions_history { get; set; } = new List<AuctionHistory>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
