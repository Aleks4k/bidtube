using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Users.DTO
{
    public class UserBasicDataDto
    {
        public double average_rating { get; set; }
        public int total_reviews { get; set; }
        public int posted_auctions { get; set; }
        public int auctions_won { get; set; }
    }
}
