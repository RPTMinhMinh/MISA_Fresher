using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Dtos.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        public DateTime Timestamp { get; set; }
        public int StatusCode { get; set; }

        public ApiResponse()
        {
            Timestamp = DateTime.UtcNow;
        }

        public ApiResponse(bool success, string message, int statusCode, T? data = default, List<string>? errors = null)
        {
            Success = success;
            Message = message;
            StatusCode = statusCode;
            Data = data;
            Errors = errors;
            Timestamp = DateTime.UtcNow;
        }

        public static ApiResponse<T> SuccessResponse(T? data, string message = "Thành công")
        {
            return new ApiResponse<T>(true, message, 200, data);
        }

        public static ApiResponse<T> CreatedResponse(T? data, string message = "Tạo mới thành công")
        {
            return new ApiResponse<T>(true, message, 201, data);
        }

        public static ApiResponse<T> NoContentResponse(string message = "Không có dữ liệu")
        {
            return new ApiResponse<T>(true, message, 204);
        }

        public static ApiResponse<T> BadRequestResponse(string message = "Yêu cầu không hợp lệ", List<string>? errors = null)
        {
            return new ApiResponse<T>(false, message, 400, default, errors);
        }

        public static ApiResponse<T> UnauthorizedResponse(string message = "Không có quyền truy cập")
        {
            return new ApiResponse<T>(false, message, 401);
        }

        public static ApiResponse<T> ForbiddenResponse(string message = "Truy cập bị từ chối")
        {
            return new ApiResponse<T>(false, message, 403);
        }

        public static ApiResponse<T> NotFoundResponse(string message = "Không tìm thấy dữ liệu")
        {
            return new ApiResponse<T>(false, message, 404);
        }

        public static ApiResponse<T> ConflictResponse(string message = "Xung đột dữ liệu")
        {
            return new ApiResponse<T>(false, message, 409);
        }

        public static ApiResponse<T> ValidationErrorResponse(string message = "Lỗi xác thực", List<string>? errors = null)
        {
            return new ApiResponse<T>(false, message, 422, default, errors);
        }

        public static ApiResponse<T> InternalServerErrorResponse(string message = "Lỗi máy chủ", List<string>? errors = null)
        {
            return new ApiResponse<T>(false, message, 500, default, errors);
        }

        public static ApiResponse<T> ServiceUnavailableResponse(string message = "Dịch vụ tạm thời không khả dụng")
        {
            return new ApiResponse<T>(false, message, 503);
        }
    }
}
