using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.Core.Dtos.Common;

namespace MISA.QLTS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BaseApiController : ControllerBase
    {
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

        protected IActionResult OkResult<T>(T? data, string message = "Thành công")
        {
            var response = ApiResponse<T>.SuccessResponse(data, message);
            return Ok(response);
        }

        protected IActionResult CreatedResult<T>(T? data, string message = "Tạo mới thành công")
        {
            var response = ApiResponse<T>.CreatedResponse(data, message);
            return Created("", response);
        }

        protected IActionResult NoContentResult<T>(string message = "Không có dữ liệu")
        {
            var response = ApiResponse<T>.NoContentResponse(message);
            return NoContent();
        }

        protected IActionResult BadRequestResult<T>(string message = "Yêu cầu không hợp lệ", List<string>? errors = null)
        {
            var response = ApiResponse<T>.BadRequestResponse(message, errors);
            return BadRequest(response);
        }

        protected IActionResult UnauthorizedResult<T>(string message = "Không có quyền truy cập")
        {
            var response = ApiResponse<T>.UnauthorizedResponse(message);
            return Unauthorized(response);
        }

        protected IActionResult NotFoundResult<T>(string message = "Không tìm thấy dữ liệu")
        {
            var response = ApiResponse<T>.NotFoundResponse(message);
            return NotFound(response);
        }

        protected IActionResult ConflictResult<T>(string message = "Xung đột dữ liệu")
        {
            var response = ApiResponse<T>.ConflictResponse(message);
            return Conflict(response);
        }

        protected IActionResult ValidationErrorResult<T>(string message = "Lỗi xác thực", List<string>? errors = null)
        {
            var response = ApiResponse<T>.ValidationErrorResponse(message, errors);
            return UnprocessableEntity(response);
        }

        protected IActionResult InternalServerErrorResult<T>(string message = "Lỗi máy chủ", List<string>? errors = null)
        {
            var response = ApiResponse<T>.InternalServerErrorResponse(message, errors);
            return StatusCode(500, response);
        }
    }
}
