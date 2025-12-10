using MISA.QLTS.Core.Dtos;
using MISA.QLTS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Interfaces.Services
{
    /// <summary>
    /// Interface định nghĩa các dịch vụ xử lý nghiệp vụ cho phòng ban
    /// </summary>
    public interface IDepartmentService
    {
        /// <summary>
        /// Lấy danh sách tất cả phòng ban
        /// </summary>
        /// <returns>Danh sách phòng ban dưới dạng DTO</returns>
        Task<IEnumerable<DepartmentDto>> GetAllAsync();

        /// <summary>
        /// Tìm phòng ban theo mã phòng ban
        /// </summary>
        /// <param name="code">Mã phòng ban cần tìm</param>
        /// <returns>Thông tin phòng ban dưới dạng DTO hoặc null nếu không tồn tại</returns>
        Task<DepartmentDto?> GetByCodeAsync (string code);
    }
}
