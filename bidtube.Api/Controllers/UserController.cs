using bidtube.Api.Filters;
using bidtube.Application.Users.Commands;
using bidtube.Application.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace bidtube.Api.Controllers
{
    public class UserController : BaseController
    {
        public UserController(){}
        [HttpPost]
        [AllowAnonymous]
        [Route("login-google")]
        public async Task<ActionResult> LogInWithGoogle(GoogleLoginRequestCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<ActionResult> LogIn(UserLoginCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<ActionResult> RegisterUser(UserCreateCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("refresh")]
        public async Task<ActionResult> RefreshAccessToken(UserRefreshTokenQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
        //Ova metoda nije anonimna jer smo na frontu setovali već access token.
        [HttpPost]
        [Route("change-password-google")]
        public async Task<ActionResult> FinishRegistrationGoogle(GoogleLoginFinishRegistrationCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpPost]
        [Route("getUserData")]
        public async Task<ActionResult> GetUserData(UserGetBasicDataQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
        [HttpPost]
        [Route("getUserEditData")]
        public async Task<ActionResult> GetUserEditData(UserGetBasicEditDataQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
        [HttpPost]
        [Route("updateUserData")]
        public async Task<ActionResult> UpdateUserData(UpdateUserDataCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpPost]
        [Route("updateUserPassword")]
        public async Task<ActionResult> UpdateUserPassword(UpdateUserPasswordCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
