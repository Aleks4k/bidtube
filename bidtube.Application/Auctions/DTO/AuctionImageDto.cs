using bidtube.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Auctions.DTO
{
    public class AuctionImageDto
    {
        public int ID { get; set; }
        public int auction_id { get; set; }
        public string url { get; set; } = string.Empty;
        public string alt_text { get; set; } = string.Empty;
    }
}
