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
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var department = await _departmentRepository.GetAllAsync();
            return department.Select(DepartmentMapper.MapToDepartmentDto);
        }

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
