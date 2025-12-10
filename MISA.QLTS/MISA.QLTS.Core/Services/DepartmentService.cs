using MISA.QLTS.Core.Dtos;
using MISA.QLTS.Core.Entities;
using MISA.QLTS.Core.Interfaces.Repositories;
using MISA.QLTS.Core.Interfaces.Services;
using MISA.QLTS.Core.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Services
{
    /// <summary>
    /// Lớp dịch vụ xử lý nghiệp vụ cho phòng ban
    /// </summary>
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        /// <summary>
        /// Khởi tạo một instance mới của DepartmentService
        /// </summary>
        /// <param name="departmentRepository">Repository cho phòng ban</param>
        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        /// <summary>
        /// Lấy danh sách tất cả phòng ban
        /// </summary>
        /// <returns>Danh sách phòng ban dưới dạng DTO</returns>
        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var department = await _departmentRepository.GetAllAsync();
            return department.Select(DepartmentMapper.MapToDepartmentDto);
        }

        /// <summary>
        /// Tìm phòng ban theo mã phòng ban
        /// </summary>
        /// <param name="code">Mã phòng ban cần tìm</param>
        /// <returns>Thông tin phòng ban dưới dạng DTO hoặc null nếu không tồn tại</returns>
        public async Task<DepartmentDto?> GetByCodeAsync(string code)
        {
            var department = await _departmentRepository.GetByCodeAsync(code);
            if (department == null)
            {
                return null;
            }
            return DepartmentMapper.MapToDepartmentDto(department);
        }
    }
}
