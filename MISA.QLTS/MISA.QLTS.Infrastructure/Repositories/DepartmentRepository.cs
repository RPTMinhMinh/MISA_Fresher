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
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly IDatabaseContext _dbContext;

        public DepartmentRepository(IDatabaseContext context)
        {
            _dbContext = context;
        }
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
