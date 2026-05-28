using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.WebApi.Filters;

public class CustomExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if(context.Exception is ArgumentException)
        {
            context.Result = new ObjectResult(new { ErrorMessage = context.Exception.Message })
            {
                StatusCode = 400,
            };
            return;
        }

        if(context.Exception is UnauthorizedAccessException)
        {
            context.Result = new ObjectResult(new { ErrorMessage = context.Exception.Message })
            {
                StatusCode = 401,
            };
            return;
        }

        if(context.Exception is KeyNotFoundException)
        {
            context.Result = new ObjectResult(new { ErrorMessage = context.Exception.Message })
            {
                StatusCode = 404,
            };
            return;
        }

        if(context.Exception is InvalidOperationException)
        {
            context.Result = new ObjectResult(new { ErrorMessage = context.Exception.Message })
            {
                StatusCode = 409,
            };
            return;
        }

        if(context.Exception is NotImplementedException)
        {
            context.Result = new ObjectResult(new { ErrorMessage = "Not implemented" })
            {
                StatusCode = 500,
            };
            return;
        }

        context.Result = new ObjectResult(new { ErrorMessage = "Something went wrong" })
        {
            StatusCode = 500,
        };
    }
}
