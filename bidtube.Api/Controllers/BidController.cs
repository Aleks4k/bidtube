using bidtube.Application.Bids.Commands;
using Microsoft.AspNetCore.Mvc;

namespace bidtube.Api.Controllers
{
    public class BidController: BaseController
    {
        public BidController()
        {}
        [HttpPost]
        [Route("putOffer")]
        public async Task<ActionResult> PutOffer(BidCreateCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
