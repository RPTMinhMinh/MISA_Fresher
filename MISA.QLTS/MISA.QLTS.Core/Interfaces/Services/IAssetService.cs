using MISA.QLTS.Core.Dtos;
using MISA.QLTS.Core.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Interfaces.Services
{
    /// <summary>
    /// Interface định nghĩa các dịch vụ xử lý nghiệp vụ cho tài sản
    /// </summary>
    public interface IAssetService
    {
        /// <summary>
        /// Tạo mới một tài sản
        /// </summary>
        /// <param name="createAssetDto">Thông tin tài sản cần tạo</param>
        /// <returns>Thông tin tài sản đã tạo</returns>
        Task<AssetResponseDto> CreateAssetAsync(CreateAssetDto createAssetDto);

        /// <summary>
        /// Lấy thông tin tài sản theo mã tài sản
        /// </summary>
        /// <param name="assetCode">Mã tài sản cần tìm</param>
        /// <returns>Thông tin tài sản hoặc null nếu không tồn tại</returns>
        Task<AssetResponseDto?> GetAssetByCodeAsync(string assetCode);

        /// <summary>
        /// Lấy danh sách tài sản phân trang với các điều kiện lọc
        /// </summary>
        /// <param name="request">Thông tin yêu cầu phân trang và lọc</param>
        /// <returns>Kết quả phân trang danh sách tài sản</returns>
        Task<PaginatedResult<AssetResponseDto>> GetAssetsPaginatedAsync(PaginationRequest request);

        /// <summary>
        /// Sao chép (clone) thông tin từ một tài sản có sẵn
        /// </summary>
        /// <param name="assetCode">Mã tài sản cần sao chép</param>
        /// <returns>Thông tin tài sản được sao chép dưới dạng DTO</returns>
        Task<CloneAssetDto> CloneAssetAsync(string assetCode);

        /// <summary>
        /// Cập nhật thông tin tài sản
        /// </summary>
        /// <param name="assetCode">Mã tài sản cần cập nhật</param>
        /// <param name="updateAssetDto">Thông tin cập nhật</param>
        /// <returns>Thông tin tài sản sau khi cập nhật</returns>
        Task<AssetResponseDto> UpdateAssetAsync(string assetCode, UpdateAssetDto updateAssetDto);

        /// <summary>
        /// Xóa nhiều tài sản theo danh sách mã
        /// </summary>
        /// <param name="assetCodes">Danh sách mã tài sản cần xóa</param>
        /// <returns>Số lượng tài sản đã xóa thành công</returns>
        Task<int> DeleteAssetsAsync(List<string> assetCodes);

        /// <summary>
        /// Lấy mã tài sản tiếp theo theo quy tắc đánh mã
        /// </summary>
        /// <returns>Mã tài sản tiếp theo</returns>
        Task<string> GetNextAssetCodeAsync();

        /// <summary>
        /// Lấy thống kê về tài sản theo các tiêu chí
        /// </summary>
        /// <param name="request">Thông tin yêu cầu thống kê</param>
        /// <returns>Thông tin thống kê tài sản</returns>
        Task<AssetStatisticsDto> GetAssetStatisticsAsync(AssetStatisticsRequest request);
    }
}
