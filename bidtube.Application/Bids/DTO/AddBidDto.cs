using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Bids.DTO
{
    public class AddBidDto
    {
        public required string mail { get; set; }
        public int auction_id { get; set; }
        public double amount { get; set; }
    }
}
