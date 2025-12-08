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
    public interface IDatabaseContext
    {
        IDbConnection CreateConnection();
    }
    public class DatabaseContext : IDatabaseContext
    {
        private readonly string _connectionString;
        public DatabaseContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("QLTSConnectionString")
                ?? throw new ArgumentNullException("Connection string 'QLTSConnectionString' not found.");
        }

        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
