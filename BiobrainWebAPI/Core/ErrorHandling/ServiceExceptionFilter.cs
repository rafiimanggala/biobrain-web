using System.Collections.Generic;
using System.Linq;
using Biobrain.Application.Common.Exceptions;
using Biobrain.Infrastructure.Payments.ErrorHandling;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace BiobrainWebAPI.Core.ErrorHandling
{
    public class ServiceExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ServiceExceptionFilter> _logger;
        public ServiceExceptionFilter(ILogger<ServiceExceptionFilter> logger) => _logger = logger;

        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            switch (exception)
            {
                case ValidationException validationException:
                    context.Result = CreateValidationExceptionResult(validationException);
                    context.ExceptionHandled = true;
                    break;

                case PermissionDeniedException:
                    context.Result = new ForbidResult();
                    context.ExceptionHandled = true;
                    break;

                case ObjectWasNotFoundException objectWasNotFoundException:
                    context.Result = CreateObjectWasNotFoundExceptionResult(objectWasNotFoundException);
                    context.ExceptionHandled = true;
                    break;

                case BusinessLogicException businessLogicException:
                    context.Result = CreateBusinessLogicExceptionResult(businessLogicException);
                    context.ExceptionHandled = true;
                    break;

                //case PaymentException paymentException:
	               // context.Result = CreatePaymentExceptionResult(paymentException);
	               // context.ExceptionHandled = true;
	               // break;

                default:
                    _logger.LogError(exception, exception.Message);
                    context.Result = new BadRequestObjectResult(exception.Message);
                    context.ExceptionHandled = true;
                    break;
            }
        }

        private static IActionResult CreateBusinessLogicExceptionResult(BusinessLogicException businessLogicException) => new UnprocessableEntityObjectResult(new 
                {
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Title = businessLogicException.GetType().Name,
                    Code = businessLogicException.ErrorCode,
                    businessLogicException.Errors
                });

        private static IActionResult CreateValidationExceptionResult(ValidationException validationException)
        {
            var errors = validationException.Errors?
                                            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                                            .ToDictionary(_ => _.Key, _ => _.ToArray());
            if (errors == null || errors.Count < 1)
	            errors = new Dictionary<string, string[]> {{ "Error", new []{validationException.Message} }};

            return new BadRequestObjectResult(new ValidationProblemDetails(errors));
        }

        private static IActionResult CreateObjectWasNotFoundExceptionResult(ObjectWasNotFoundException objectWasNotFoundException) => new NotFoundObjectResult(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Object was not found.",
                    Detail = $"There is no '{objectWasNotFoundException.ObjectName}'" +
                             (objectWasNotFoundException.Id == null ? "." : $" with id '{objectWasNotFoundException.Id}'.")
                });

        private static IActionResult CreatePaymentExceptionResult(PaymentException paymentException) => new BadRequestObjectResult(paymentException.Message);
    }
}
