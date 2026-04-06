using bidtube.Application.Categories.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Auctions.DTO
{
    public class AuctionsWithCategoriesResponseDto
    {
        public AuctionsReturnDto auctions { get; set; } = new AuctionsReturnDto();  
        public AllCategoriesDto categories { get; set; } = new AllCategoriesDto();
    }
}
