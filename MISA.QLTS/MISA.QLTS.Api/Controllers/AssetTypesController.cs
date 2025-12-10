using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.Core.Dtos;
using MISA.QLTS.Core.Dtos.Common;
using MISA.QLTS.Core.Interfaces.Services;

namespace MISA.QLTS.Api.Controllers
{
    /// <summary>
    /// Controller quản lý các thao tác liên quan đến loại tài sản
    /// </summary>
    public class AssetTypesController : BaseApiController
    {
        private readonly IAssetTypeService _assetTypeService;

        /// <summary>
        /// Khởi tạo controller với service loại tài sản
        /// </summary>
        /// <param name="assetTypeService">Service xử lý nghiệp vụ loại tài sản</param>
        public AssetTypesController(IAssetTypeService assetTypeService)
        {
            _assetTypeService = assetTypeService;
        }

        /// <summary>
        /// Lấy tất cả loại tài sản
        /// </summary>
        /// <returns>Danh sách loại tài sản hoặc thông báo lỗi</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssetTypeDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var assetTypes = await _assetTypeService.GetAllAsync();

                if (!assetTypes.Any())
                {
                    return NoContentResult<IEnumerable<AssetTypeDto>>("Không có dữ liệu loại tài sản");
                }
                return OkResult(assetTypes, "Lấy danh sách loại tài sản thành công");
            }
            catch (Exception ex)
            {
                return InternalServerErrorResult<IEnumerable<AssetTypeDto>>(
                    "Đã xảy ra lỗi khi xử lý yêu cầu",
                    new List<string> { ex.Message });
            }
        }

        /// <summary>
        /// Lấy thông tin loại tài sản theo mã
        /// </summary>
        /// <param name="code">Mã loại tài sản</param>
        /// <returns>Thông tin loại tài sản hoặc thông báo lỗi</returns>
        [HttpGet("{code}")]
        [ProducesResponseType(typeof(ApiResponse<AssetTypeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByCodeAsync(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                    return BadRequestResult<AssetTypeDto>("Mã loại tài sản cố định không được để trống");

                var department = await _assetTypeService.GetByCodeAsync(code);

                if (department == null)
                    return NotFoundResult<AssetTypeDto>($"Không tìm thấy laại tài sản với mã {code}");

                return OkResult(department, "Lấy thông tin loại tài sản thành công");
            }
            catch (Exception ex)
            {
                // Log error here
                return InternalServerErrorResult<AssetTypeDto>(
                    "Đã xảy ra lỗi khi xử lý yêu cầu",
                    new List<string> { ex.Message });
            }
        }
    }
}
