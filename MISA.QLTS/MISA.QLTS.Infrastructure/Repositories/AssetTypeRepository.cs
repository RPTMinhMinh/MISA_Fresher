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
    public class AssetTypeRepository : IAssetTypeRepository
    {
        private readonly IDatabaseContext _dbContext;
        public AssetTypeRepository(IDatabaseContext dbcontect)
        {
            _dbContext = dbcontect;    
        }
        public async Task<IEnumerable<AssetType>> GetAllAsync()
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                select
                    asset_type_id as AssetTypeId,
                    asset_type_code as AssetTypeCode,
                    asset_type_name as AssetTypeName,
                    useful_life as UsefulLife,
                    recreciation_rate as RecreciationRate
                from asset_type
                order by asset_type_code";
            return await connection.QueryAsync<AssetType>(sql);
        }

        public async Task<AssetType?> GetByCodeAsync(string code)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                select 
                    asset_type_id as AssetTypeId,
                    asset_type_code as AssetTypeCode,
                    asset_type_name as AssetTypeName,
                    useful_life as UsefulLife,
                    recreciation_rate as RecreciationRate
                from asset_type
                where asset_type_code = @Code";
            var parameter = new { Code = code };
            return await connection.QueryFirstOrDefaultAsync<AssetType>(sql, parameter);
        }
    }
}
