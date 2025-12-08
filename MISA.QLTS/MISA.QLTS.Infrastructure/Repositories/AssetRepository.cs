using Dapper;
using MISA.QLTS.Core.Dtos.Common;
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
    public class AssetRepository : IAssetRepository
    {
        private readonly IDatabaseContext _connectionString;

        public AssetRepository(IDatabaseContext databaseContext)
        {
            _connectionString = databaseContext;
        }
        public async Task<Asset> CreateAsset(Asset asset)
        {
            using var connection = _connectionString.CreateConnection();
            var sql = @"
                insert into asset (
                    asset_id,
                    asset_code,
                    asset_name,
                    department_id,
                    asset_type_id,
                    created_date,
                    use_year,
                    useful_life,
                    tracking_start_year,
                    decreciation_rate,
                    quantity,
                    original_price,
                    annual_decreciation
                ) values (
                    @AssetId,
                    @AssetCode,
                    @AssetName,
                    @DepartmentId,
                    @AssetTypeId,
                    @CreatedDate,
                    @UseYear,
                    @UsefulLife,
                    @TrackingStartYear,
                    @DecreciationRate,
                    @Quantity,
                    @OriginalPrice,
                    @AnnualDecreciation
                )";
            await connection.ExecuteAsync(sql, asset);
            return asset;
        }

        public async Task<Asset?> GetByCodeAsync(string assetCode)
        {
            using var connection = _connectionString.CreateConnection();
            var sql = @"
                select
                    asset_id AS AssetId,
                    asset_code AS AssetCode,
                    asset_name AS AssetName,
                    department_id AS DepartmentId,
                    asset_type_id AS AssetTypeId,
                    created_date AS CreatedDate,
                    use_year AS UseYear,
                    useful_life AS UsefulLife,
                    tracking_start_year AS TrackingStartYear,
                    decreciation_rate AS DecreciationRate,
                    quantity AS Quantity,
                    original_price AS OriginalPrice,
                    annual_decreciation AS AnnualDecreciation
                from asset
                where asset_code = @AssetCode";
            var parameters = new { AssetCode = assetCode };
            return await connection.QueryFirstOrDefaultAsync<Asset>(sql, parameters);
        }


        public async Task<string> GetNextAssetCodeAsync()
        {
            using var connection = _connectionString.CreateConnection();
            var sql = @"
                select 
                    asset_code
                from asset
                where asset_code like 'TS%'
                order by cast(substring(asset_code, 3) as unsigned) desc
                limit 1";
            var maxCode = await connection.QueryFirstOrDefaultAsync<string>(sql);
            if (string.IsNullOrEmpty(maxCode))
            {
                return "TS00001";
            }

            if (maxCode.Length >= 5 && maxCode.StartsWith("TS"))
            {
                if (int.TryParse(maxCode.Substring(2), out int currentNumber))
                {
                    int nextNumber = currentNumber + 1;
                    return $"TS{nextNumber:D5}";
                }
            }

            return "TS00001";
        }

        public async Task<IEnumerable<Asset>> GetPagedAsync(PaginationRequest request)
        {
            using var connection = _connectionString.CreateConnection();

            var baseSql = @"
            SELECT 
                a.asset_id AS AssetId,
                a.asset_code AS AssetCode,
                a.asset_name AS AssetName,
                a.department_id AS DepartmentId,
                a.asset_type_id AS AssetTypeId,
                a.created_date AS CreatedDate,
                a.use_year AS UseYear,
                a.useful_life AS UsefulLife,
                a.tracking_start_year AS TrackingStartYear,
                a.decreciation_rate AS DecreciationRate,
                a.quantity AS Quantity,
                a.original_price AS OriginalPrice,
                a.annual_decreciation AS AnnualDecreciation
            FROM asset a
            LEFT JOIN department d ON a.department_id = d.department_id
            LEFT JOIN asset_type at ON a.asset_type_id = at.asset_type_id
            WHERE 1=1";

            var parameters = new DynamicParameters();

            // Thêm điều kiện search
            if (!string.IsNullOrWhiteSpace(request.SearchKeyword))
            {
                baseSql += " AND (a.asset_code LIKE @Keyword OR a.asset_name LIKE @Keyword)";
                parameters.Add("Keyword", $"%{request.SearchKeyword}%");
            }

            // Thêm điều kiện filter theo department
            if (!string.IsNullOrWhiteSpace(request.DepartmentCode))
            {
                baseSql += " AND d.department_code = @DepartmentCode";
                parameters.Add("DepartmentCode", request.DepartmentCode);
            }

            // Thêm điều kiện filter theo asset type
            if (!string.IsNullOrWhiteSpace(request.AssetTypeCode))
            {
                baseSql += " AND at.asset_type_code = @AssetTypeCode";
                parameters.Add("AssetTypeCode", request.AssetTypeCode);
            }

            // Sắp xếp mặc định theo asset_code tăng dần
            baseSql += " ORDER BY a.asset_code ASC";

            // Thêm phân trang
            baseSql += " LIMIT @PageSize OFFSET @Offset";

            var offset = (request.PageNumber - 1) * request.PageSize;
            parameters.Add("PageSize", request.PageSize);
            parameters.Add("Offset", offset);

            return await connection.QueryAsync<Asset>(baseSql, parameters);
        }

        public async Task<int> GetTotalCountAsync(string? searchKeyword = null, string? departmentCode = null, string? assetTypeCode = null)
        {
            using var connection = _connectionString.CreateConnection();
            var sql = @"
            SELECT COUNT(*) 
            FROM asset a
            LEFT JOIN department d ON a.department_id = d.department_id
            LEFT JOIN asset_type at ON a.asset_type_id = at.asset_type_id
            WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                sql += " AND (a.asset_code LIKE @Keyword OR a.asset_name LIKE @Keyword)";
                parameters.Add("Keyword", $"%{searchKeyword}%");
            }

            if (!string.IsNullOrWhiteSpace(departmentCode))
            {
                sql += " AND d.department_code = @DepartmentCode";
                parameters.Add("DepartmentCode", departmentCode);
            }

            if (!string.IsNullOrWhiteSpace(assetTypeCode))
            {
                sql += " AND at.asset_type_code = @AssetTypeCode";
                parameters.Add("AssetTypeCode", assetTypeCode);
            }

            return await connection.ExecuteScalarAsync<int>(sql, parameters);
        }

        public async Task UpdateAsync(Asset asset)
        {
            using var connection = _connectionString.CreateConnection();

            var sql = @"
            UPDATE asset 
            SET 
                asset_name = @AssetName,
                department_id = @DepartmentId,
                asset_type_id = @AssetTypeId,
                created_date = @CreatedDate,
                use_year = @UseYear,
                useful_life = @UsefulLife,
                tracking_start_year = @TrackingStartYear,
                decreciation_rate = @DecreciationRate,
                quantity = @Quantity,
                original_price = @OriginalPrice,
                annual_decreciation = @AnnualDecreciation
            WHERE asset_id = @AssetId";

            await connection.ExecuteAsync(sql, asset);
        }

        public async Task DeleteAsync(string assetCode)
        {
            using var connection = _connectionString.CreateConnection();

            var sql = "DELETE FROM asset WHERE asset_code = @AssetCode";

            await connection.ExecuteAsync(sql, new { AssetCode = assetCode });
        }


    }
}
