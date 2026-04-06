using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Auctions.DTO
{
    public class AuctionsReturnDto
    {
        public List<AuctionsDataDto> auctions { get; set; } = new List<AuctionsDataDto>();
        public int total_rows { get; set; }
    }
}
