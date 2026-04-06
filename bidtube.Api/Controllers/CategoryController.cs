using bidtube.Application.Categories.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bidtube.Api.Controllers
{
    public class CategoryController : BaseController
    {
        public CategoryController() { }
        [HttpGet]
        [Route("getCategories")]
        public async Task<ActionResult> GetCategories()
        {
            return Ok(await Mediator.Send(new GetAllCategoriesQuery()));
        }
    }
}
