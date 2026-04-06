using bidtube.Application.Auctions.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Auctions.DTO
{
    public class GetAuctionsQueryDto
    {
        public int? category_id { get; set; }
        public SortType sort_type { get; set; }
        public int price_filter { get; set; } //SortType.Price
        public int auction_id_filter { get; set; } //SortType.TimePosted i SortType.Price
        public DateTime? date_of_expiration_filter { get; set; } //SortType.AlmostFinished
        public OrderType order_type { get; set; }
        public PaginationDirectionType pagination_direction_type { get; set; }
        public int rows_to_skip { get; set; }
    }
}
