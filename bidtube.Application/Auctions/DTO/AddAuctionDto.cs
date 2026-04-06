using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Auctions.DTO
{
    public class AddAuctionDto
    {
        public required string mail { get; set; }
        public required string title { get; set; }
        public int category_id { get; set; }
        public required string description { get; set; }
        public double starting_price { get; set; }
        public DateTime date_of_exp { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        public List<string> Image_names { get; set; } = new List<string>();
    }
}
