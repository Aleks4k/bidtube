using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;
using Microsoft.AspNetCore.Http;
using FluentValidation;
using FluentValidation.Results;

namespace bidtube.Api.Filters
{
    public class ValidationBehaviour : IFluentValidationAutoValidationResultFactory
    {
        public Task<IActionResult?> CreateActionResult(ActionExecutingContext context, ValidationProblemDetails? validationProblemDetails, IDictionary<IValidationContext, ValidationResult> validationResults)
        {
            var result = new BadRequestObjectResult(new { ValidationErrors = validationProblemDetails?.Errors });
            result.StatusCode = 400; //Ovo je 400 svakako ali ne škodi da ga opet postavim.
            return Task.FromResult<IActionResult?>(result);
        }
    }
}
