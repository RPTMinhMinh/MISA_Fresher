using MISA.QLTS.Core.Dtos;
using MISA.QLTS.Core.Dtos.Common;
using MISA.QLTS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface định nghĩa các thao tác truy cập dữ liệu cho tài sản
    /// </summary>
    public interface IAssetRepository
    {
        /// <summary>
        /// Tìm tài sản theo mã tài sản
        /// </summary>
        /// <param name="assetCode">Mã tài sản cần tìm</param>
        /// <returns>Tài sản tìm thấy hoặc null nếu không tồn tại</returns>
        Task<Asset?> GetByCodeAsync(string assetCode);

        /// <summary>
        /// Tạo mới tài sản trong cơ sở dữ liệu
        /// </summary>
        /// <param name="asset">Thông tin tài sản cần tạo</param>
        /// <returns>Tài sản đã được tạo</returns>
        Task<Asset> CreateAsset(Asset asset);

        /// <summary>
        /// Lấy mã tài sản tiếp theo từ cơ sở dữ liệu
        /// </summary>
        /// <returns>Mã tài sản tiếp theo</returns>
        Task<string> GetNextAssetCodeAsync();

        /// <summary>
        /// Lấy tổng số lượng tài sản theo điều kiện lọc
        /// </summary>
        /// <param name="searchKeyword">Từ khóa tìm kiếm</param>
        /// <param name="departmentCode">Mã bộ phận</param>
        /// <param name="assetTypeCode">Mã loại tài sản</param>
        /// <returns>Tổng số lượng tài sản thỏa mãn điều kiện</returns>
        Task<int> GetTotalCountAsync(string? searchKeyword = null, string? departmentCode = null, string? assetTypeCode = null);

        /// <summary>
        /// Lấy danh sách tài sản phân trang theo điều kiện
        /// </summary>
        /// <param name="request">Thông tin yêu cầu phân trang và lọc</param>
        /// <returns>Danh sách tài sản của trang hiện tại</returns>
        Task<IEnumerable<Asset>> GetPagedAsync(PaginationRequest request);

        /// <summary>
        /// Cập nhật thông tin tài sản
        /// </summary>
        /// <param name="asset">Thông tin tài sản cần cập nhật</param>
        Task UpdateAsync(Asset asset);

        /// <summary>
        /// Xóa tài sản theo định danh
        /// </summary>
        /// <param name="assetId">Định danh tài sản cần xóa</param>
        Task DeleteAsync(string assetId);

        /// <summary>
        /// Lấy thống kê tài sản theo các tiêu chí
        /// </summary>
        /// <param name="searchKeyword">Từ khóa tìm kiếm</param>
        /// <param name="departmentCode">Mã bộ phận</param>
        /// <param name="assetTypeCode">Mã loại tài sản</param>
        /// <returns>Thông tin thống kê tài sản</returns>
        Task<AssetStatisticsDto> GetAssetStatisticsAsync(
            string? searchKeyword = null,
            string? departmentCode = null,
            string? assetTypeCode = null);
    }
}
