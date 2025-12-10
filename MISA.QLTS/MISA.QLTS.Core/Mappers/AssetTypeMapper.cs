using MISA.QLTS.Core.Dtos;
using MISA.QLTS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Mappers
{
    /// <summary>
    /// Lớp mapper chuyển đổi giữa thực thể AssetType và DTO
    /// </summary>
    public static class AssetTypeMapper
    {
        /// <summary>
        /// Chuyển đổi từ thực thể AssetType sang AssetTypeDto
        /// </summary>
        /// <param name="assetType">Thực thể AssetType cần chuyển đổi</param>
        /// <returns>Đối tượng AssetTypeDto đã được ánh xạ</returns>
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
