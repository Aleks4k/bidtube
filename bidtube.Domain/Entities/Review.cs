using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Domain.Entities
{
    public class Review
    {
        public int ID { get; set; }
        public int received_id { get; set; }
        public User? User { get; set; }
        public int sent_id { get; set; }
        public User? User_Sent { get; set; }
        public int auction_history_id { get; set; }
        public AuctionHistory? Auction { get; set; }
        public byte rating { get; set; }
        public string? comment { get; set; }
        public DateTime date_of_review { get; set; }
    }
}
