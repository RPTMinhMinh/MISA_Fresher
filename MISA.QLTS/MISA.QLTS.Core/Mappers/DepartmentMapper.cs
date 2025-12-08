using MISA.QLTS.Core.Dtos;
using MISA.QLTS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Mappers
{
    public static class DepartmentMapper
    {
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
