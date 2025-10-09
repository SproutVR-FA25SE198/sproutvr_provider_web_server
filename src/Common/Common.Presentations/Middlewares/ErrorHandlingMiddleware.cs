using Common.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Common.Presentation.Middlewares;
public sealed class ErrorHandlingMiddleware : IMiddleware
{

    private readonly ILogger<ErrorHandlingMiddleware> _logger;


    public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
    {
        _logger = logger;
    }


    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "NotFoundException occurred: {Message}", ex.Message);
            await WriteToResponse(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "UnauthorizedAccessException occurred: {Message}", ex.Message);
            await WriteToResponse(context, StatusCodes.Status401Unauthorized, "Unauthorized access. Please authenticate.");
        }
        catch (FileUploadException ex)
        {
            _logger.LogWarning(ex, "FileUploadException occurred: {Message}", ex.Message);
            await WriteToResponse(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (OperationFailedException ex)
        {
            _logger.LogWarning(ex, "Operation failed: {Message}", ex.Message);
            await WriteToResponse(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await WriteToResponse(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
        }
    }

    /// <summary>
    /// Write essential data to response object
    /// </summary>
    /// <param name="context"></param>
    /// <param name="statusCode"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    private async Task WriteToResponse(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            StatusCode = statusCode,
            Message = message
        };

        string json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}

