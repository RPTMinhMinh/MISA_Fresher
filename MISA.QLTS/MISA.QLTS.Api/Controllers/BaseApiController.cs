using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.Core.Dtos.Common;

namespace MISA.QLTS.Api.Controllers
{
    /// <summary>
    /// Controller cơ sở cung cấp các phương thức hỗ trợ xử lý response API
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class BaseApiController : ControllerBase
    {
        /// <summary>
        /// Tạo kết quả API từ response object
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của response</typeparam>
        /// <param name="response">Đối tượng response</param>
        /// <returns>ActionResult tương ứng với status code</returns>
        protected IActionResult ApiResult<T>(ApiResponse<T> response)
        {
            return response.StatusCode switch
            {
                200 => Ok(response),
                201 => Created("", response),
                204 => NoContent(),
                400 => BadRequest(response),
                401 => Unauthorized(response),
                403 => Forbid(),
                404 => NotFound(response),
                409 => Conflict(response),
                422 => UnprocessableEntity(response),
                500 => StatusCode(500, response),
                503 => StatusCode(503, response),
                _ => StatusCode(response.StatusCode, response)
            };
        }

        /// <summary>
        /// Tạo response thành công (200 OK)
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu</typeparam>
        /// <param name="data">Dữ liệu trả về</param>
        /// <param name="message">Thông báo</param>
        /// <returns>ActionResult với status 200</returns>
        protected IActionResult OkResult<T>(T? data, string message = "Thành công")
        {
            var response = ApiResponse<T>.SuccessResponse(data, message);
            return Ok(response);
        }

        /// <summary>
        /// Tạo response tạo mới thành công (201 Created)
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu</typeparam>
        /// <param name="data">Dữ liệu đã tạo</param>
        /// <param name="message">Thông báo</param>
        /// <returns>ActionResult với status 201</returns>
        protected IActionResult CreatedResult<T>(T? data, string message = "Tạo mới thành công")
        {
            var response = ApiResponse<T>.CreatedResponse(data, message);
            return Created("", response);
        }

        /// <summary>
        /// Tạo response không có nội dung (204 No Content)
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu</typeparam>
        /// <param name="message">Thông báo</param>
        /// <returns>ActionResult với status 204</returns>
        protected IActionResult NoContentResult<T>(string message = "Không có dữ liệu")
        {
            var response = ApiResponse<T>.NoContentResponse(message);
            return NoContent();
        }

        /// <summary>
        /// Tạo response yêu cầu không hợp lệ (400 Bad Request)
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu</typeparam>
        /// <param name="message">Thông báo lỗi</param>
        /// <param name="errors">Danh sách chi tiết lỗi</param>
        /// <returns>ActionResult với status 400</returns>
        protected IActionResult BadRequestResult<T>(string message = "Yêu cầu không hợp lệ", List<string>? errors = null)
        {
            var response = ApiResponse<T>.BadRequestResponse(message, errors);
            return BadRequest(response);
        }

        /// <summary>
        /// Tạo response không có quyền truy cập (401 Unauthorized)
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu</typeparam>
        /// <param name="message">Thông báo</param>
        /// <returns>ActionResult với status 401</returns>
        protected IActionResult UnauthorizedResult<T>(string message = "Không có quyền truy cập")
        {
            var response = ApiResponse<T>.UnauthorizedResponse(message);
            return Unauthorized(response);
        }

        /// <summary>
        /// Tạo response không tìm thấy (404 Not Found)
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu</typeparam>
        /// <param name="message">Thông báo</param>
        /// <returns>ActionResult với status 404</returns>
        protected IActionResult NotFoundResult<T>(string message = "Không tìm thấy dữ liệu")
        {
            var response = ApiResponse<T>.NotFoundResponse(message);
            return NotFound(response);
        }

        /// <summary>
        /// Tạo response xung đột dữ liệu (409 Conflict)
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu</typeparam>
        /// <param name="message">Thông báo</param>
        /// <returns>ActionResult với status 409</returns>
        protected IActionResult ConflictResult<T>(string message = "Xung đột dữ liệu")
        {
            var response = ApiResponse<T>.ConflictResponse(message);
            return Conflict(response);
        }

        /// <summary>
        /// Tạo response lỗi xác thực (422 Unprocessable Entity)
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu</typeparam>
        /// <param name="message">Thông báo lỗi</param>
        /// <param name="errors">Danh sách chi tiết lỗi</param>
        /// <returns>ActionResult với status 422</returns>
        protected IActionResult ValidationErrorResult<T>(string message = "Lỗi xác thực", List<string>? errors = null)
        {
            var response = ApiResponse<T>.ValidationErrorResponse(message, errors);
            return UnprocessableEntity(response);
        }

        /// <summary>
        /// Tạo response lỗi máy chủ (500 Internal Server Error)
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu</typeparam>
        /// <param name="message">Thông báo lỗi</param>
        /// <param name="errors">Danh sách chi tiết lỗi</param>
        /// <returns>ActionResult với status 500</returns>
        protected IActionResult InternalServerErrorResult<T>(string message = "Lỗi máy chủ", List<string>? errors = null)
        {
            var response = ApiResponse<T>.InternalServerErrorResponse(message, errors);
            return StatusCode(500, response);
        }
    }
}
