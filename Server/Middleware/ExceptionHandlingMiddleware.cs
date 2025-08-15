using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using WarehouseManager.Server.Exceptions;
using WarehouseManager.Shared.Api;

namespace WarehouseManager.Server.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Необработанное исключение, обнаруженное в middleware");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            int statusCode = StatusCodes.Status500InternalServerError;
            string message = "Внутренняя ошибка сервера";

            switch (exception)
            {
                case ConflictException conflictEx:
                    statusCode = StatusCodes.Status409Conflict;
                    message = conflictEx.Message;
                    break;

                case NotFoundException notFoundEx:
                    statusCode = StatusCodes.Status404NotFound;
                    message = notFoundEx.Message;
                    break;

                case ValidationException validationEx:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = validationEx.Message;
                    break;

                case BusinessException businessEx:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = businessEx.Message;
                    break;
            }

            context.Response.StatusCode = statusCode;

            var response = new ApiResponse(message, success: false);
            var json = System.Text.Json.JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }

}
