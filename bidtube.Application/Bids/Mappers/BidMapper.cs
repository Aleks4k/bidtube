using bidtube.Application.Bids.DTO;
using bidtube.Domain.Entities;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Bids.Mappers
{
    [Mapper]
    public static partial class BidMapper
    {
        public static partial BidDetailsDto ToDetailsDto(this Bid entity);
        public static partial BidWithUserIDDto ToDetailsWithUserIDDto(this Bid entity);
    }
}
