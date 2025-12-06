using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.Core.Dtos;
using MISA.QLTS.Core.Dtos.Common;
using MISA.QLTS.Core.Interfaces.Services;

namespace MISA.QLTS.Api.Controllers
{
    public class AssetsController : BaseApiController
    {
        private readonly IAssetService _assetService;

        public AssetsController(IAssetService assetService)
        {
            _assetService = assetService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AssetResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAsset([FromBody] CreateAssetDto createAssetDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                    return BadRequestResult<AssetResponseDto>("Dữ liệu không hợp lệ", errors);
                }

                var asset = await _assetService.CreateAssetAsync(createAssetDto);
                return CreatedResult(asset, "Tạo tài sản thành công");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFoundResult<AssetResponseDto>(ex.Message);
            }
            catch (Exception ex)
            {
                // Log error here
                return InternalServerErrorResult<AssetResponseDto>(
                    "Đã xảy ra lỗi khi tạo tài sản",
                    new List<string> { ex.Message });
            }
        }

        [HttpGet("{assetCode}")]
        [ProducesResponseType(typeof(ApiResponse<AssetResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAssetByCode (string assetCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(assetCode))
                    return BadRequestResult<AssetResponseDto>("Mã tài sản không được để trống");

                var asset = await _assetService.GetAssetByCodeAsync(assetCode);

                if (asset == null)
                    return NotFoundResult<AssetResponseDto>($"Không tìm thấy tài sản với mã {assetCode}");

                return OkResult(asset, "Lấy thông tin tài sản thành công");
            }
            catch (Exception ex)
            {
                // Log error here
                return InternalServerErrorResult<AssetResponseDto>(
                    "Đã xảy ra lỗi khi lấy thông tin tài sản",
                    new List<string> { ex.Message });
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<AssetResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAssetsPaged([FromQuery] PaginationRequest request)
        {
            try
            {
                // Validate request
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequestResult<PaginatedResult<AssetResponseDto>>("Dữ liệu phân trang không hợp lệ", errors);
                }

                var pagedAssets = await _assetService.GetAssetsPaginatedAsync(request);

                if (!pagedAssets.Data.Any())
                    return NoContentResult<PaginatedResult<AssetResponseDto>>("Không tìm thấy tài sản phù hợp");

                return OkResult(pagedAssets, "Lấy danh sách tài sản phân trang thành công");
            }
            catch (Exception ex)
            {
                // Log error here
                return InternalServerErrorResult<PaginatedResult<AssetResponseDto>>(
                    "Đã xảy ra lỗi khi lấy danh sách tài sản",
                    new List<string> { ex.Message });
            }
        }

        [HttpGet("{assetCode}/clone")]
        [ProducesResponseType(typeof(ApiResponse<CloneAssetDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CloneAsset(string assetCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(assetCode))
                    return BadRequestResult<CloneAssetDto>("Mã tài sản không được để trống");

                var clonedAsset = await _assetService.CloneAssetAsync(assetCode);
                return OkResult(clonedAsset, "Nhân bản tài sản thành công");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFoundResult<CloneAssetDto>(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequestResult<CloneAssetDto>(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerErrorResult<CloneAssetDto>(
                    "Đã xảy ra lỗi khi nhân bản tài sản",
                    new List<string> { ex.Message });
            }
        }

        [HttpPut("{assetCode}")]
        [ProducesResponseType(typeof(ApiResponse<AssetResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsset(string assetCode, [FromBody] UpdateAssetDto updateAssetDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(assetCode))
                    return BadRequestResult<AssetResponseDto>("Mã tài sản không được để trống");

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequestResult<AssetResponseDto>("Dữ liệu cập nhật không hợp lệ", errors);
                }

                var updatedAsset = await _assetService.UpdateAssetAsync(assetCode, updateAssetDto);
                return OkResult(updatedAsset, "Cập nhật tài sản thành công");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFoundResult<AssetResponseDto>(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequestResult<AssetResponseDto>(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerErrorResult<AssetResponseDto>(
                    "Đã xảy ra lỗi khi cập nhật tài sản",
                    new List<string> { ex.Message });
            }
        }

        [HttpDelete]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAssets([FromBody] DeleteAssetDto deleteDto)
        {
            try
            {
                if (deleteDto == null || !deleteDto.AssetCodes.Any())
                    return BadRequestResult<int>("Danh sách mã tài sản không được để trống");

                var deletedCount = await _assetService.DeleteAssetsAsync(deleteDto.AssetCodes);
                return OkResult(deletedCount, $"Đã xóa {deletedCount} tài sản thành công");
            }
            catch (ArgumentException ex)
            {
                return BadRequestResult<int>(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerErrorResult<int>(
                    "Đã xảy ra lỗi khi xóa tài sản",
                    new List<string> { ex.Message });
            }
        }

        [HttpGet("next-code")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNextAssetCode()
        {
            try
            {
                var nextCode = await _assetService.GetNextAssetCodeAsync();
                return OkResult(nextCode, "Lấy mã tài sản tiếp theo thành công");
            }
            catch (Exception ex)
            {
                return InternalServerErrorResult<string>(
                    "Đã xảy ra lỗi khi lấy mã tài sản tiếp theo",
                    new List<string> { ex.Message });
            }
        }
    }
}
