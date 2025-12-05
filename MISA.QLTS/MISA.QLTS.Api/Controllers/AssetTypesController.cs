using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.Core.Dtos;
using MISA.QLTS.Core.Dtos.Common;
using MISA.QLTS.Core.Interfaces.Services;

namespace MISA.QLTS.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AssetTypesController : BaseApiController
    {
        private readonly IAssetTypeService _assetTypeService;
        public AssetTypesController(IAssetTypeService assetTypeService)
        {
            _assetTypeService = assetTypeService;
        }

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
