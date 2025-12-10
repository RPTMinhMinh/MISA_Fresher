using MISA.QLTS.Core.Dtos;
using MISA.QLTS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Mappers
{
    /// <summary>
    /// Lớp mapper chuyển đổi giữa thực thể Department và DTO
    /// </summary>
    public static class DepartmentMapper
    {
        /// <summary>
        /// Chuyển đổi từ thực thể Department sang DepartmentDto
        /// </summary>
        /// <param name="department">Thực thể Department cần chuyển đổi</param>
        /// <returns>Đối tượng DepartmentDto đã được ánh xạ</returns>
        public static DepartmentDto MapToDepartmentDto(Department department)
        {
            return new DepartmentDto
            {
                DepartmentId = department.DepartmentId,
                DepartmentCode = department.DepartmentCode,
                DepartmentName = department.DepartmentName
            };
        }
    }
}
