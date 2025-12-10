using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.Core.Dtos;
using MISA.QLTS.Core.Dtos.Common;
using MISA.QLTS.Core.Interfaces.Services;

namespace MISA.QLTS.Api.Controllers
{
    /// <summary>
    /// Controller quản lý các thao tác liên quan đến bộ phận
    /// </summary>
    public class DepartmentsController : BaseApiController
    {
        private readonly IDepartmentService _departmentService;

        /// <summary>
        /// Khởi tạo controller với service bộ phận
        /// </summary>
        /// <param name="departmentService">Service xử lý nghiệp vụ bộ phận</param>
        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        /// <summary>
        /// Lấy tất cả bộ phận
        /// </summary>
        /// <returns>Danh sách bộ phận hoặc thông báo lỗi</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DepartmentDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var departments = await _departmentService.GetAllAsync();

                if(!departments.Any())
                {
                    return NoContentResult<IEnumerable<DepartmentDto>>("Không có dữ liệu bộ phận");
                }
                return OkResult(departments, "Lấy danh sách bộ phận thành công");
            }
            catch (Exception ex)
            {
                return InternalServerErrorResult<IEnumerable<DepartmentDto>>(
                    "Đã xảy ra lỗi khi xử lý yêu cầu",
                    new List<string> { ex.Message });
            }
        }

        /// <summary>
        /// Lấy thông tin bộ phận theo mã
        /// </summary>
        /// <param name="code">Mã bộ phận</param>
        /// <returns>Thông tin bộ phận hoặc thông báo lỗi</returns>
        [HttpGet("{code}")]
        [ProducesResponseType(typeof(ApiResponse<DepartmentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByCodeAsync(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                    return BadRequestResult<DepartmentDto>("Mã bộ phận không được để trống");

                var department = await _departmentService.GetByCodeAsync(code);

                if (department == null)
                    return NotFoundResult<DepartmentDto>($"Không tìm thấy bộ phận với mã {code}");

                return OkResult(department, "Lấy thông tin bộ phận thành công");
            }
            catch (Exception ex)
            {
                // Log error here
                return InternalServerErrorResult<DepartmentDto>(
                    "Đã xảy ra lỗi khi xử lý yêu cầu",
                    new List<string> { ex.Message });
            }
        }
    }
}
