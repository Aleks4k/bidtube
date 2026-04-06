using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Domain.Entities
{
    public class Category
    {
        public int ID { get; set; }
        public string name { get; set; } = string.Empty;
        public int? parent_category_id { get; set; }
        public Category? Parent { get; set; }
        public string? icon_name { get; set; }
        public ICollection<Category> Subcategories { get; set; } = new List<Category>();
        public ICollection<Auction> Auctions { get; set; } = new List<Auction>();
        public ICollection<AuctionHistory> Auctions_history { get; set; } = new List<AuctionHistory>();
    }
}
