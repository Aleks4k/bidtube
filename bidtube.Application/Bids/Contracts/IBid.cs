using bidtube.Application.Bids.DTO;
using bidtube.Application.Common.DTO;
using bidtube.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Bids.Contracts
{
    public interface IBid
    {
        Task<ValidationResult> ValidateBidObject(AddBidDto bid, int user_id);
        Task AddBid(Bid bid, CancellationToken cancellationToken);
    }
}
