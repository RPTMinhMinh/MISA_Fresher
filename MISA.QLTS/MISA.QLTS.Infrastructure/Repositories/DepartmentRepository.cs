using Dapper;
using MISA.QLTS.Core.Entities;
using MISA.QLTS.Core.Interfaces.Repositories;
using MISA.QLTS.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Infrastructure.Repositories
{
    /// <summary>
    /// Lớp repository triển khai các thao tác truy cập dữ liệu cho phòng ban
    /// </summary>
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly IDatabaseContext _dbContext;

        /// <summary>
        /// Khởi tạo một instance mới của DepartmentRepository
        /// </summary>
        /// <param name="context">Ngữ cảnh cơ sở dữ liệu</param>
        public DepartmentRepository(IDatabaseContext context)
        {
            _dbContext = context;
        }

        /// <summary>
        /// Lấy tất cả phòng ban từ cơ sở dữ liệu
        /// </summary>
        /// <returns>Danh sách tất cả phòng ban</returns>
        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                select 
                    department_id as DepartmentId,
                    department_code as DepartmentCode,
                    department_name as DepartmentName
                from department
                order by department_code";
            return await connection.QueryAsync<Department>(sql);
        }

        /// <summary>
        /// Tìm phòng ban theo mã phòng ban
        /// </summary>
        /// <param name="code">Mã phòng ban cần tìm</param>
        /// <returns>Phòng ban tìm thấy hoặc null nếu không tồn tại</returns>
        public async Task<Department?> GetByCodeAsync(string code)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                select 
                    department_id as DepartmentId,
                    department_code as DepartmentCode,
                    department_name as DepartmentName
                from department
                where department_code = @Code";
            var parameters = new { Code = code };
            return await connection.QueryFirstOrDefaultAsync<Department>(sql, parameters);
        }
    }
}
