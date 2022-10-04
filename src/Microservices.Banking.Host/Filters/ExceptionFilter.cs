using System.Diagnostics;
using FluentValidation;
using Microservices.Banking.Core.Api;
using Microservices.Banking.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Microservices.Banking.Host.Filters;

internal sealed class ExceptionFilter : ExceptionFilterAttribute
{
	private readonly ILogger<ExceptionFilter> _logger;
	
	public ExceptionFilter(ILogger<ExceptionFilter> logger)
	{
		_logger = logger;
	}
	
	public override void OnException(ExceptionContext context)
	{
		HandleExceptionAsync(context);
		context.ExceptionHandled = true;
	}

	private void HandleExceptionAsync(ExceptionContext context)
	{
		switch (context.Exception)
		{
			case ItemNotFoundException:
				SetExceptionResult(context, StatusCodes.Status404NotFound);
				break;
			case AppValidationException:
				SetExceptionResult(context, StatusCodes.Status400BadRequest);
				break;
			case ValidationException:
				HandleFluentValidationException(context);
				break;
			default:
				_logger.LogError(context.Exception, "Unexpected error occured during request.");
				SetExceptionResult(context, StatusCodes.Status500InternalServerError);
				break;
		}
	}

	private void SetExceptionResult(ExceptionContext context, int code)
	{
		var exception = context.Exception;
		object responseModel;

		if (string.IsNullOrEmpty(exception.Message))
		{
			context.Result = new StatusCodeResult(code);
			return;
		}

		if (exception is CoreException coreException)
		{
			var propertyErrors = coreException.PropertyErrors
				.Select(node => new ApiResponse.ApiResponseErrorNode(node.Property, node.Errors)).ToArray();

			responseModel = new ApiResponse()
			{
				ErrorMessage = coreException.Message,
				PropertyErrors = propertyErrors,
			};
		}
		else
		{
			responseModel = new TraceableApiResponse()
			{
				ErrorMessage = "Unexpected error occured.",
				PropertyErrors = Array.Empty<ApiResponse.ApiResponseErrorNode>(),
				TraceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier,
			};
		}

		context.Result = new JsonResult(responseModel)
		{
			StatusCode = code
		};
	}

	private static void HandleFluentValidationException(ExceptionContext context)
	{
		var exception = (ValidationException)context.Exception;

		var errorNodes = exception.Errors
			.GroupBy(error => error.PropertyName, error => error.ErrorMessage)
			.Select(group => new ApiResponse.ApiResponseErrorNode(group.Key, group.ToArray()))
			.ToArray();

		var response = new ApiResponse
		{
			ErrorMessage = "Request model validation failed",
			PropertyErrors = errorNodes,
		};
		
		context.Result = new JsonResult(response)
		{
			StatusCode = StatusCodes.Status400BadRequest
		};
	}
}