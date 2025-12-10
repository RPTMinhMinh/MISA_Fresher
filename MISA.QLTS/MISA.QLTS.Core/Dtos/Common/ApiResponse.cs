using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Dtos.Common
{
    /// <summary>
    /// Lớp đại diện cho phản hồi API chuẩn hóa với kiểu dữ liệu tổng quát
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu của Data</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Trạng thái thành công hay thất bại của yêu cầu
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Thông điệp mô tả kết quả
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Dữ liệu trả về (có thể null)
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Danh sách các lỗi chi tiết (nếu có)
        /// </summary>
        public List<string>? Errors { get; set; }

        /// <summary>
        /// Thời điểm tạo phản hồi (UTC)
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Mã trạng thái HTTP
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Khởi tạo một instance mới của ApiResponse với thời điểm hiện tại
        /// </summary>
        public ApiResponse()
        {
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Khởi tạo một instance của ApiResponse với các tham số cụ thể
        /// </summary>
        /// <param name="success">Trạng thái thành công</param>
        /// <param name="message">Thông điệp mô tả</param>
        /// <param name="statusCode">Mã trạng thái HTTP</param>
        /// <param name="data">Dữ liệu trả về</param>
        /// <param name="errors">Danh sách lỗi</param>
        public ApiResponse(bool success, string message, int statusCode, T? data = default, List<string>? errors = null)
        {
            Success = success;
            Message = message;
            StatusCode = statusCode;
            Data = data;
            Errors = errors;
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Tạo phản hồi thành công với mã trạng thái 200
        /// </summary>
        /// <param name="data">Dữ liệu trả về</param>
        /// <param name="message">Thông điệp (mặc định: "Thành công")</param>
        /// <returns>ApiResponse với trạng thái thành công</returns>
        public static ApiResponse<T> SuccessResponse(T? data, string message = "Thành công")
        {
            return new ApiResponse<T>(true, message, 200, data);
        }

        /// <summary>
        /// Tạo phản hồi thành công khi tạo mới với mã trạng thái 201
        /// </summary>
        /// <param name="data">Dữ liệu đã tạo</param>
        /// <param name="message">Thông điệp (mặc định: "Tạo mới thành công")</param>
        /// <returns>ApiResponse với trạng thái Created</returns>
        public static ApiResponse<T> CreatedResponse(T? data, string message = "Tạo mới thành công")
        {
            return new ApiResponse<T>(true, message, 201, data);
        }

        /// <summary>
        /// Tạo phản hồi thành công khi không có nội dung với mã trạng thái 204
        /// </summary>
        /// <param name="message">Thông điệp (mặc định: "Không có dữ liệu")</param>
        /// <returns>ApiResponse với trạng thái No Content</returns>
        public static ApiResponse<T> NoContentResponse(string message = "Không có dữ liệu")
        {
            return new ApiResponse<T>(true, message, 204);
        }

        /// <summary>
        /// Tạo phản hồi lỗi yêu cầu không hợp lệ với mã trạng thái 400
        /// </summary>
        /// <param name="message">Thông điệp (mặc định: "Yêu cầu không hợp lệ")</param>
        /// <param name="errors">Danh sách lỗi chi tiết</param>
        /// <returns>ApiResponse với trạng thái Bad Request</returns>
        public static ApiResponse<T> BadRequestResponse(string message = "Yêu cầu không hợp lệ", List<string>? errors = null)
        {
            return new ApiResponse<T>(false, message, 400, default, errors);
        }

        /// <summary>
        /// Tạo phản hồi lỗi không có quyền truy cập với mã trạng thái 401
        /// </summary>
        /// <param name="message">Thông điệp (mặc định: "Không có quyền truy cập")</param>
        /// <returns>ApiResponse với trạng thái Unauthorized</returns>
        public static ApiResponse<T> UnauthorizedResponse(string message = "Không có quyền truy cập")
        {
            return new ApiResponse<T>(false, message, 401);
        }

        /// <summary>
        /// Tạo phản hồi lỗi truy cập bị từ chối với mã trạng thái 403
        /// </summary>
        /// <param name="message">Thông điệp (mặc định: "Truy cập bị từ chối")</param>
        /// <returns>ApiResponse với trạng thái Forbidden</returns>
        public static ApiResponse<T> ForbiddenResponse(string message = "Truy cập bị từ chối")
        {
            return new ApiResponse<T>(false, message, 403);
        }

        /// <summary>
        /// Tạo phản hồi lỗi không tìm thấy dữ liệu với mã trạng thái 404
        /// </summary>
        /// <param name="message">Thông điệp (mặc định: "Không tìm thấy dữ liệu")</param>
        /// <returns>ApiResponse với trạng thái Not Found</returns>
        public static ApiResponse<T> NotFoundResponse(string message = "Không tìm thấy dữ liệu")
        {
            return new ApiResponse<T>(false, message, 404);
        }

        /// <summary>
        /// Tạo phản hồi lỗi xung đột dữ liệu với mã trạng thái 409
        /// </summary>
        /// <param name="message">Thông điệp (mặc định: "Xung đột dữ liệu")</param>
        /// <returns>ApiResponse với trạng thái Conflict</returns>
        public static ApiResponse<T> ConflictResponse(string message = "Xung đột dữ liệu")
        {
            return new ApiResponse<T>(false, message, 409);
        }

        /// <summary>
        /// Tạo phản hồi lỗi xác thực với mã trạng thái 422
        /// </summary>
        /// <param name="message">Thông điệp (mặc định: "Lỗi xác thực")</param>
        /// <param name="errors">Danh sách lỗi xác thực chi tiết</param>
        /// <returns>ApiResponse với trạng thái Unprocessable Entity</returns>
        public static ApiResponse<T> ValidationErrorResponse(string message = "Lỗi xác thực", List<string>? errors = null)
        {
            return new ApiResponse<T>(false, message, 422, default, errors);
        }

        /// <summary>
        /// Tạo phản hồi lỗi máy chủ với mã trạng thái 500
        /// </summary>
        /// <param name="message">Thông điệp (mặc định: "Lỗi máy chủ")</param>
        /// <param name="errors">Danh sách lỗi chi tiết</param>
        /// <returns>ApiResponse với trạng thái Internal Server Error</returns>
        public static ApiResponse<T> InternalServerErrorResponse(string message = "Lỗi máy chủ", List<string>? errors = null)
        {
            return new ApiResponse<T>(false, message, 500, default, errors);
        }

        /// <summary>
        /// Tạo phản hồi lỗi dịch vụ không khả dụng với mã trạng thái 503
        /// </summary>
        /// <param name="message">Thông điệp (mặc định: "Dịch vụ tạm thời không khả dụng")</param>
        /// <returns>ApiResponse với trạng thái Service Unavailable</returns>
        public static ApiResponse<T> ServiceUnavailableResponse(string message = "Dịch vụ tạm thời không khả dụng")
        {
            return new ApiResponse<T>(false, message, 503);
        }
    }
}
