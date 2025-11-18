using Model.Pagging;
using System;
using System.Collections.Generic;

namespace Service.Respone
{
    public class ApiResponse<T>
    {
        /// <summary>
        /// Mã trạng thái HTTP (200, 400, 500…)
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Trạng thái xử lý: true = thành công, false = lỗi logic hoặc exception.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mô tả ngắn gọn kết quả xử lý.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Dữ liệu trả về (model, list, hoặc null).
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Chi tiết lỗi (nếu có).
        /// </summary>
        public object? Errors { get; set; }

        /// <summary>
        /// Thông tin phân trang (nếu có).
        /// </summary>
        public PaginationInfo? Pagination { get; set; }

        // ✅ 200 / 201 - Thành công
        public static ApiResponse<T> Ok(T data, string message = "Success", int statusCode = 200)
            => new() { Success = true, Message = message, Data = data, StatusCode = statusCode };

        // ✅ 400 / 404 - Lỗi logic hoặc request sai
        public static ApiResponse<T> Fail(string message, object? errors = null, int statusCode = 400)
            => new() { Success = false, Message = message, Errors = errors, StatusCode = statusCode };

        // ✅ 500 - Lỗi hệ thống
        public static ApiResponse<T> Error(string message = "Internal Server Error", object? exception = null, int statusCode = 500)
            => new() { Success = false, Message = message, Errors = exception, StatusCode = statusCode };

        // ✅ Tạo phản hồi có phân trang từ PaginatedList<T>
        public static ApiResponse<IReadOnlyCollection<T>> FromPaginatedList(PaginatedList<T> list, string message = "Fetched successfully", int statusCode = 200)
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
