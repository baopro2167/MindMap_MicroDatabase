using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Respone;
using Model.Pagging;

namespace MindMap_MicroProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        // Thành công + có thể kèm phân trang + custom status code
        protected IActionResult Success<T>(
            T data,
            string message = "Success",
            PaginationInfo? pagination = null,
            int statusCode = StatusCodes.Status200OK)
        {
            var response = new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                StatusCode = statusCode,
                Pagination = pagination
            };

            return StatusCode(statusCode, response);
        }

        // Dành riêng cho Created (201)
        protected IActionResult Created<T>(T data, string message = "Created successfully")
        {
            return Success(data, message, statusCode: StatusCodes.Status201Created);
        }

        // BadRequest (400)
        protected IActionResult BadRequestResponse(string message = "Bad Request", object? errors = null)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = message,
                Errors = errors,
                StatusCode = StatusCodes.Status400BadRequest
            });
        }

        // NotFound (404)
        protected IActionResult NotFoundResponse(string message = "Not Found")
        {
            return StatusCode(StatusCodes.Status404NotFound, new ApiResponse<object>
            {
                Success = false,
                Message = message,
                StatusCode = StatusCodes.Status404NotFound
            });
        }

        // Server Error (500)
        protected IActionResult ServerErrorResponse(string message = "Internal Server Error", object? exception = null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                Success = false,
                Message = message,
                Errors = exception,
                StatusCode = StatusCodes.Status500InternalServerError
            });
        }
    }
}