using MISA.QLTS.Core.Dtos;
using MISA.QLTS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Mappers
{
    public static class AssetTypeMapper
    {
        public static AssetTypeDto MaptoAssetTypeDto (AssetType assetType)
        {
            return new AssetTypeDto
            {
                AssetTypeId = assetType.AssetTypeId,
                AssetTypeCode = assetType.AssetTypeCode,
                AssetTypeName = assetType.AssetTypeName,
                UsefulLife = assetType.UsefulLife,
                RecreciationRate = assetType.RecreciationRate
            };
        }
    }
}
