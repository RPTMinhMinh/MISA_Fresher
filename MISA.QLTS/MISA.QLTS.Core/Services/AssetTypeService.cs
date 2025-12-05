using MISA.QLTS.Core.Dtos;
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
    public class AssetTypeService : IAssetTypeService
    {
        private readonly IAssetTypeRepository _assetTypeRepository;
        public AssetTypeService(IAssetTypeRepository assetTypeRepository)
        {
            _assetTypeRepository = assetTypeRepository;
        }
        public async Task<IEnumerable<AssetTypeDto>> GetAllAsync()
        {
            var assetTypes = await _assetTypeRepository.GetAllAsync();
            return assetTypes.Select(AssetTypeMapper.MaptoAssetTypeDto);
        }

        public async Task<AssetTypeDto?> GetByCodeAsync(string code)
        {
            var assetType = await _assetTypeRepository.GetByCodeAsync(code);
            if (assetType == null) return null;
            return AssetTypeMapper.MaptoAssetTypeDto(assetType);
        }
    }
}
