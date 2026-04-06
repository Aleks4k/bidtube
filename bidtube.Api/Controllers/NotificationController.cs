using bidtube.Application.Notifications.Commands;
using bidtube.Application.Notifications.Queries;
using Microsoft.AspNetCore.Mvc;

namespace bidtube.Api.Controllers
{
    public class NotificationController : BaseController
    {
        public NotificationController() { }
        [HttpPost]
        [Route("getNotifications")]
        public async Task<ActionResult> GetNotifications(GetNotificationsQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
        [HttpGet]
        [Route("unread/count")]
        public async Task<ActionResult> GetUnreadCount()
        {
            return Ok(await Mediator.Send(new GetUnreadCountQuery()));
        }
        [HttpPost]
        [Route("markAsRead")]
        public async Task<ActionResult> MarkAsRead(ReadNotificationCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpPost]
        [Route("markAllAsRead")]
        public async Task<ActionResult> MarkAllAsRead(ReadAllNotificationsCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
