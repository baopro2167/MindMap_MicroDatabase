// File: Middlewares/ExceptionMiddleware.cs
using Microsoft.AspNetCore.Http;
using Model.Exceptions;
using Service.Respone;
using System.Net;
using System.Text.Json;

namespace MindMap_MicroProject.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            catch (AppException appEx)               // L?i do mình ném (có status code)
            {
                _logger.LogWarning(appEx, "AppException: {Message}", appEx.Message);
                await HandleAppExceptionAsync(context, appEx);
            }
            catch (UnauthorizedAccessException)       // .NET t? ném khi [Authorize] fail
            {
                _logger.LogWarning("Unauthorized access attempted");
                await HandleUnauthorizedAsync(context);
            }
            catch (Exception ex)                      // L?i không mong mu?n
            {
                _logger.LogError(ex, "Unhandled exception: {ExceptionMessage}", ex.Message);
                await HandleServerErrorAsync(context, ex);
            }
        }

        // 1. X? lý AppException (do mình throw có status code + details)
        private static async Task HandleAppExceptionAsync(HttpContext context, AppException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex.StatusCode;

            ApiResponse<object> response = ex.StatusCode switch
            {
                400 => ApiResponse<object>.BadRequest(ex.Message, ex.Details),
                401 => ApiResponse<object>.Unauthorized(ex.Message),
                403 => ApiResponse<object>.Forbidden(ex.Message),
                404 => ApiResponse<object>.NotFound(ex.Message),
                409 => ApiResponse<object>.Fail(ex.Message,  409), // Conflict
                _ => ApiResponse<object>.Error("L?i không xác ??nh", ex)
            };

            await context.Response.WriteAsJsonAsync(response, JsonOptions);
        }

        // 2. Khi [Authorize] fail ho?c throw UnauthorizedAccessException
        private static async Task HandleUnauthorizedAsync(HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var response = ApiResponse<object>.Unauthorized("Vui lòng ??ng nh?p l?i");
            await context.Response.WriteAsJsonAsync(response, JsonOptions);
        }

        // 3. L?i server th?t s? (500)
        private static async Task HandleServerErrorAsync(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = ApiResponse<object>.Error("?ã x?y ra l?i h? th?ng, vui lòng th? l?i sau", ex);
            await context.Response.WriteAsJsonAsync(response, JsonOptions);
        }

        // Tùy ch?n: camelCase cho frontend (React, Vue, mobile…)
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            // WriteIndented = true // ch? ?? debug, production thì ?? false
        };
    }

    // Extension ?? ??ng ký nhanh
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseAppExceptionHandler(this IApplicationBuilder builder)
            => builder.UseMiddleware<ExceptionMiddleware>();
    }
}