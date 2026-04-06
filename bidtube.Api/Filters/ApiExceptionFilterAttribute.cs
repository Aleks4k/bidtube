using bidtube.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace bidtube.Api.Filters
{
    public class ApiExceptionFilterAttribute: ExceptionFilterAttribute
    {
        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;
        public ApiExceptionFilterAttribute() {
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                { typeof(CloudinaryException), HandleCloudinaryException },
                { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException }
            };
        }
        public override void OnException(ExceptionContext context)
        {
            HandleException(context);
        }
        private void HandleException(ExceptionContext context)
        {
            var type = context.Exception.GetType();
            if (_exceptionHandlers.TryGetValue(type, out var handler))
            {
                handler.Invoke(context);
                return;
            }
            HandleUnknownException(context);
        }
        private void HandleUnknownException(ExceptionContext context)
        {
            var details = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An error occurred while processing your request.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };
            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            context.ExceptionHandled = true;
        }
        private void HandleCloudinaryException(ExceptionContext context)
        {
            var exception = (CloudinaryException)context.Exception;
            var details = new ProblemDetails
            {
                Status = StatusCodes.Status503ServiceUnavailable,
                Title = exception.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            };
            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status503ServiceUnavailable
            };
            context.ExceptionHandled = true;
        }
        private void HandleUnauthorizedAccessException(ExceptionContext context)
        {
            var exception = (UnauthorizedAccessException)context.Exception;
            var details = new ProblemDetails
            {
                Status = StatusCodes.Status405MethodNotAllowed,
                Title = exception.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            };
            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status405MethodNotAllowed
            };
            context.ExceptionHandled = true;
        }
    }
}