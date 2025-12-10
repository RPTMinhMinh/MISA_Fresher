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
    /// <summary>
    /// Lớp dịch vụ xử lý nghiệp vụ cho loại tài sản
    /// </summary>
    public class AssetTypeService : IAssetTypeService
    {
        private readonly IAssetTypeRepository _assetTypeRepository;

        /// <summary>
        /// Khởi tạo một instance mới của AssetTypeService
        /// </summary>
        /// <param name="assetTypeRepository">Repository cho loại tài sản</param>
        public AssetTypeService(IAssetTypeRepository assetTypeRepository)
        {
            _assetTypeRepository = assetTypeRepository;
        }

        /// <summary>
        /// Lấy danh sách tất cả loại tài sản
        /// </summary>
        /// <returns>Danh sách loại tài sản dưới dạng DTO</returns>
        public async Task<IEnumerable<AssetTypeDto>> GetAllAsync()
        {
            var assetTypes = await _assetTypeRepository.GetAllAsync();
            return assetTypes.Select(AssetTypeMapper.MaptoAssetTypeDto);
        }

        /// <summary>
        /// Tìm loại tài sản theo mã loại
        /// </summary>
        /// <param name="code">Mã loại tài sản cần tìm</param>
        /// <returns>Thông tin loại tài sản dưới dạng DTO hoặc null nếu không tồn tại</returns>
        public async Task<AssetTypeDto?> GetByCodeAsync(string code)
        {
            var assetType = await _assetTypeRepository.GetByCodeAsync(code);
            if (assetType == null) return null;
            return AssetTypeMapper.MaptoAssetTypeDto(assetType);
        }
    }
}
