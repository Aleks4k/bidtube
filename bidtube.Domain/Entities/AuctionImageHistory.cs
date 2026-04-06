using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Domain.Entities
{
    public class AuctionImageHistory
    {
        public int ID { get; set; }
        public int auction_image_id { get; set; }
        public int auction_id { get; set; }
        public string url { get; set; } = string.Empty;
        public string alt_text { get; set; } = string.Empty;
        public int auction_history_id { get; set; }
        public AuctionHistory? Auction { get; set; }
    }
}
