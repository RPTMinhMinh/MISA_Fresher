using MISA.QLTS.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Interfaces.Services
{
    /// <summary>
    /// Interface định nghĩa các dịch vụ xử lý nghiệp vụ cho loại tài sản
    /// </summary>
    public interface IAssetTypeService
    {
        /// <summary>
        /// Lấy danh sách tất cả loại tài sản
        /// </summary>
        /// <returns>Danh sách loại tài sản dưới dạng DTO</returns>
        Task<IEnumerable<AssetTypeDto>> GetAllAsync();

        /// <summary>
        /// Tìm loại tài sản theo mã loại
        /// </summary>
        /// <param name="code">Mã loại tài sản cần tìm</param>
        /// <returns>Thông tin loại tài sản dưới dạng DTO hoặc null nếu không tồn tại</returns>
        Task<AssetTypeDto?> GetByCodeAsync(string code);
    }
}
