using MISA.QLTS.Core.Dtos.Common;
using MISA.QLTS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Interfaces.Repositories
{
    public interface IAssetRepository
    {
        Task<Asset?> GetByCodeAsync(string assetCode);
        Task<Asset> CreateAsset(Asset asset);
        Task<string> GetNextAssetCodeAsync();
        Task<int> GetTotalCountAsync(string? searchKeyword = null, string? departmentCode = null, string? assetTypeCode = null);
        Task<IEnumerable<Asset>> GetPagedAsync(PaginationRequest request);
        Task UpdateAsync(Asset asset);
        Task DeleteAsync(string assetId);
    }
}
