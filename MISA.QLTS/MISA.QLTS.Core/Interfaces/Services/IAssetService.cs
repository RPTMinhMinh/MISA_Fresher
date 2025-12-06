using MISA.QLTS.Core.Dtos;
using MISA.QLTS.Core.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Interfaces.Services
{
    public interface IAssetService
    {
        Task<AssetResponseDto> CreateAssetAsync(CreateAssetDto createAssetDto);
        Task<AssetResponseDto?> GetAssetByCodeAsync(string assetCode);
        Task<PaginatedResult<AssetResponseDto>> GetAssetsPaginatedAsync(PaginationRequest request);
        Task<CloneAssetDto> CloneAssetAsync(string assetCode);
        Task<AssetResponseDto> UpdateAssetAsync(string assetCode, UpdateAssetDto updateAssetDto);
        Task<int> DeleteAssetsAsync(List<string> assetCodes);
        Task<string> GetNextAssetCodeAsync();
    }
}
