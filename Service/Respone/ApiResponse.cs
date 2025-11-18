// File: Service/Respone/ApiResponse.cs
using Model.Pagging;
using System.Collections.Generic;

namespace Service.Respone
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public object? Errors { get; set; }
        public PaginationInfo? Pagination { get; set; }

        // 200 / 201
        public static ApiResponse<T> Ok(T data, string message = "Success", int statusCode = 200)
            => new() { Success = true, Message = message, Data = data, StatusCode = statusCode };

        public static ApiResponse<T> Created(T data, string message = "Created successfully")
            => Ok(data, message, 201);

        // 400
        public static ApiResponse<T> BadRequest(string message = "Bad Request", object? errors = null)
            => new() { Success = false, Message = message, Errors = errors, StatusCode = 400 };

        // 401 – JWT hết hạn, sai token, thiếu token
        public static ApiResponse<T> Unauthorized(string message = "Unauthorized")
            => new() { Success = false, Message = message, StatusCode = 401 };

        // 403 – Có token nhưng không có quyền
        public static ApiResponse<T> Forbidden(string message = "Forbidden")
            => new() { Success = false, Message = message, StatusCode = 403 };

        // 404
        public static ApiResponse<T> NotFound(string message = "Not Found")
            => new() { Success = false, Message = message, StatusCode = 404 };

        // 409 Conflict (nếu cần)
        public static ApiResponse<T> Fail(string message = "Fail", object? errors = null)
            => new() { Success = false, Message = message, Errors = errors, StatusCode = 409 };

        // 500
        public static ApiResponse<T> Error(string message = "Internal Server Error", object? exception = null)
            => new() { Success = false, Message = message, Errors = exception, StatusCode = 500 };

        // Phân trang – vẫn giữ nguyên
        public static ApiResponse<IReadOnlyCollection<T>> FromPaginatedList(
            PaginatedList<T> list,
            string message = "Fetched successfully",
            int statusCode = 200)
        {
            return new ApiResponse<IReadOnlyCollection<T>>
            {
                Success = true,
                Message = message,
                Data = list.Items,
                StatusCode = statusCode,
                Pagination = new PaginationInfo
                {
                    PageNumber = list.PageNumber,
                    PageSize = list.PageSize,
                    TotalCount = list.TotalCount,
                    TotalPages = list.TotalPages
                }
            };
        }
    }

    public class PaginationInfo
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}