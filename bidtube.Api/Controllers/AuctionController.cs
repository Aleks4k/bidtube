using bidtube.Application.Auctions.Commands;
using bidtube.Application.Auctions.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bidtube.Api.Controllers
{
    public class AuctionController : BaseController
    {
        public AuctionController(){}
        [HttpPost]
        [Route("addAuction")]
        public async Task<ActionResult> AddAuction(AuctionCreateCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpPost]
        [Route("getAuctions")]
        public async Task<ActionResult> GetAuctions(GetAuctionsQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
        [HttpPost]
        [Route("getAuctionsWithCategories")]
        public async Task<ActionResult> GetAuctionsWithCategories(GetAuctionsWithCategoriesQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
    }
}
