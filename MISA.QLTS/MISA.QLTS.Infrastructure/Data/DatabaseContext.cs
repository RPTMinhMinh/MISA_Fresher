using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace MISA.QLTS.Infrastructure.Data
{
    /// <summary>
    /// Interface định nghĩa ngữ cảnh cơ sở dữ liệu
    /// </summary>
    public interface IDatabaseContext
    {
        /// <summary>
        /// Tạo kết nối đến cơ sở dữ liệu
        /// </summary>
        /// <returns>Đối tượng kết nối IDbConnection</returns>
        IDbConnection CreateConnection();
    }

    /// <summary>
    /// Lớp triển khai ngữ cảnh cơ sở dữ liệu
    /// </summary>
    public class DatabaseContext : IDatabaseContext
    {
        private readonly string _connectionString;

        /// <summary>
        /// Khởi tạo một instance mới của DatabaseContext
        /// </summary>
        /// <param name="configuration">Cấu hình ứng dụng</param>
        /// <exception cref="ArgumentNullException">Khi không tìm thấy chuỗi kết nối</exception>
        public DatabaseContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("QLTSConnectionString")
                ?? throw new ArgumentNullException("Connection string 'QLTSConnectionString' not found.");
        }

        /// <summary>
        /// Tạo kết nối đến cơ sở dữ liệu MySQL
        /// </summary>
        /// <returns>Đối tượng MySqlConnection</returns>
        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
