using MISA.QLTS.Core.Dtos;
using MISA.QLTS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Interfaces.Repositories
{
    /// <summary>
    /// Interface định nghĩa các thao tác truy cập dữ liệu cho phòng ban
    /// </summary>
    public interface IDepartmentRepository
    {
        /// <summary>
        /// Lấy tất cả phòng ban từ cơ sở dữ liệu
        /// </summary>
        /// <returns>Danh sách tất cả phòng ban</returns>
        Task<IEnumerable<Department>> GetAllAsync();

        /// <summary>
        /// Tìm phòng ban theo mã phòng ban
        /// </summary>
        /// <param name="code">Mã phòng ban cần tìm</param>
        /// <returns>Phòng ban tìm thấy hoặc null nếu không tồn tại</returns>
        Task<Department?> GetByCodeAsync(string code);
    }
}
