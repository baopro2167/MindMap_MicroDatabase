using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Respone;

namespace MindMap_MicroProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        protected IActionResult Success<T>(T data, string message = "Success", PaginationInfo? pagination = null)
        {
            var response = ApiResponse<T>.Ok(data, message);
            response.Pagination = pagination;
            return Ok(response); // HTTP 200
        }

        /// <summary>
        /// Trả về response khi tạo mới (HTTP 201).
        /// </summary>
        protected IActionResult Created<T>(T data, string message = "Created successfully")
        {
            return StatusCode(201, ApiResponse<T>.Ok(data, message)); // HTTP 201
        }

        /// <summary>
        /// Trả về response khi request không hợp lệ (HTTP 400).
        /// </summary>
        protected IActionResult BadRequestResponse(string message = "Bad Request", object? errors = null)
        {
            return BadRequest(ApiResponse<object>.Fail(message, errors)); // HTTP 400
        }

        /// <summary>
        /// Trả về response khi không tìm thấy tài nguyên (HTTP 404).
        /// </summary>
        protected IActionResult NotFoundResponse(string message = "Not Found")
        {
            return NotFound(ApiResponse<object>.Fail(message)); // HTTP 404
        }

        /// <summary>
        /// Trả về response khi gặp lỗi hệ thống (HTTP 500).
        /// </summary>
        protected IActionResult ServerErrorResponse(string message = "Internal Server Error", object? exception = null)
        {
            return StatusCode(500, ApiResponse<object>.Error(message, exception)); // HTTP 500
        }
    }
}
