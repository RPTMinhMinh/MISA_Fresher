using MISA.QLTS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface định nghĩa các thao tác truy cập dữ liệu cho loại tài sản
    /// </summary>
    public interface IAssetTypeRepository
    {
        /// <summary>
        /// Lấy tất cả loại tài sản từ cơ sở dữ liệu
        /// </summary>
        /// <returns>Danh sách tất cả loại tài sản</returns>
        Task<IEnumerable<AssetType>> GetAllAsync();

        /// <summary>
        /// Tìm loại tài sản theo mã loại
        /// </summary>
        /// <param name="code">Mã loại tài sản cần tìm</param>
        /// <returns>Loại tài sản tìm thấy hoặc null nếu không tồn tại</returns>
        Task<AssetType?> GetByCodeAsync(string code);
    }
}
