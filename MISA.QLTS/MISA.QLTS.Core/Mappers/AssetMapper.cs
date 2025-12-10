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
    /// Lớp mapper chuyển đổi giữa thực thể Asset, DTO và thực hiện các tính toán nghiệp vụ liên quan đến tài sản
    /// </summary>
    public static class AssetMapper
    {
        /// <summary>
        /// Chuyển đổi từ thực thể Asset sang AssetResponseDto với thông tin đầy đủ từ Department và AssetType
        /// </summary>
        /// <param name="asset">Thực thể Asset cần chuyển đổi</param>
        /// <param name="department">Thông tin phòng ban liên quan</param>
        /// <param name="assetType">Thông tin loại tài sản liên quan</param>
        /// <returns>Đối tượng AssetResponseDto đã được ánh xạ</returns>
        public static AssetResponseDto ToAssetResponseDto(Asset asset, Department department, AssetType assetType)
        {
            return new AssetResponseDto
            {
                AssetId = asset.AssetId,
                AssetCode = asset.AssetCode,
                AssetName = asset.AssetName,
                DepartmentCode = department.DepartmentCode,
                DepartmentName = department.DepartmentName,
                AssetTypeCode = assetType.AssetTypeCode,
                AssetTypeName = assetType.AssetTypeName,
                CreatedDate = asset.CreatedDate,
                UseYear = asset.UseYear,
                UsefulLife = asset.UsefulLife,
                TrackingStartYear = asset.TrackingStartYear,
                DecreciationRate = asset.DecreciationRate,
                Quantity = asset.Quantity,
                OriginalPrice = asset.OriginalPrice,
                AnnualDecreciation = asset.AnnualDecreciation
            };
        }

        /// <summary>
        /// Chuyển đổi từ CreateAssetDto sang thực thể Asset
        /// </summary>
        /// <param name="dto">Đối tượng DTO chứa thông tin tạo tài sản</param>
        /// <param name="assetId">Định danh tài sản mới</param>
        /// <param name="assetCode">Mã tài sản mới</param>
        /// <param name="departmentId">Định danh phòng ban</param>
        /// <param name="assetTypeId">Định danh loại tài sản</param>
        /// <param name="usefulLife">Thời gian sử dụng hữu ích</param>
        /// <returns>Thực thể Asset đã được tạo</returns>
        public static Asset ToEntity(CreateAssetDto dto, Guid assetId, string assetCode,
            Guid departmentId, Guid assetTypeId, decimal usefulLife)
        {
            var asset = new Asset
            {
                AssetId = assetId,
                AssetCode = assetCode,
                AssetName = dto.AssetName,
                DepartmentId = departmentId,
                AssetTypeId = assetTypeId,
                CreatedDate = dto.CreatedDate ?? DateTime.Now,
                Quantity = dto.Quantity,
                OriginalPrice = dto.OriginalPrice,
                UsefulLife = usefulLife,
                DecreciationRate = dto.DecreciationRate
            };

            CalculateAssetFields(asset);

            return asset;
        }

        /// <summary>
        /// Tính toán các trường nghiệp vụ cho tài sản dựa trên thông tin loại tài sản
        /// </summary>
        /// <param name="asset">Thực thể Asset cần tính toán</param>
        /// <param name="assetType">Thông tin loại tài sản để lấy dữ liệu tham chiếu</param>
        private static void CalculateAssetFields(Asset asset)
        {
            // 1. Các trường phụ thuộc thời gian
            asset.UseYear = asset.CreatedDate.Year;
            asset.TrackingStartYear = asset.CreatedDate.Year;

            // 2. CÔNG THỨC CỐ ĐỊNH: Tính AnnualDecreciation
            asset.AnnualDecreciation = Math.Round(
                asset.OriginalPrice * asset.DecreciationRate / 100,
                4,
                MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Cập nhật thực thể Asset từ UpdateAssetDto
        /// </summary>
        /// <param name="asset">Thực thể Asset cần cập nhật</param>
        /// <param name="dto">Đối tượng DTO chứa thông tin cập nhật</param>
        /// <param name="department">Thông tin phòng ban mới (nếu có thay đổi)</param>
        /// <param name="assetType">Thông tin loại tài sản mới (nếu có thay đổi)</param>
        /// <param name="oldDepartment">Thông tin phòng ban cũ</param>
        /// <param name="oldAssetType">Thông tin loại tài sản cũ</param>
        /// <returns>Thực thể Asset đã được cập nhật</returns>
        public static Asset UpdateEntity(Asset asset, UpdateAssetDto dto,
        Department? department = null, AssetType? assetType = null)
        {
            if (!string.IsNullOrWhiteSpace(dto.AssetName))
                asset.AssetName = dto.AssetName;

            if (department != null) asset.DepartmentId = department.DepartmentId;

            if (assetType != null) asset.AssetTypeId = assetType.AssetTypeId;

            if (dto.Quantity.HasValue) asset.Quantity = dto.Quantity.Value;

            bool needRecalculate = false;

            if (dto.OriginalPrice.HasValue)
            {
                asset.OriginalPrice = dto.OriginalPrice.Value;
                needRecalculate = true;
            }

            if (dto.DecreciationRate.HasValue)
            {
                asset.DecreciationRate = dto.DecreciationRate.Value;
                needRecalculate = true;
            }

            if (dto.CreatedDate.HasValue)
            {
                asset.CreatedDate = dto.CreatedDate.Value;
                needRecalculate = true;
            }

            if (assetType != null)
            {
                asset.AssetTypeId = assetType.AssetTypeId;
                asset.UsefulLife = assetType.UsefulLife;
            }

            // Nếu có thay đổi các trường ảnh hưởng đến tính toán
            if (needRecalculate)
            {
                CalculateAssetFields(asset);
            }

            return asset;
        }

        /// <summary>
        /// Tạo bản sao (clone) của một tài sản với mã tài sản mới
        /// </summary>
        /// <param name="asset">Thực thể Asset gốc cần sao chép</param>
        /// <param name="newAssetCode">Mã tài sản mới cho bản sao</param>
        /// <returns>Thực thể Asset đã được sao chép</returns>
        public static Asset CloneAsset(Asset asset, string newAssetCode)
        {
            return new Asset
            {
                AssetId = Guid.NewGuid(),
                AssetCode = newAssetCode,
                AssetName = asset.AssetName,
                DepartmentId = asset.DepartmentId,
                AssetTypeId = asset.AssetTypeId,
                CreatedDate = asset.CreatedDate,
                UseYear = asset.UseYear,
                UsefulLife = asset.UsefulLife,
                TrackingStartYear = asset.TrackingStartYear,
                DecreciationRate = asset.DecreciationRate,
                Quantity = asset.Quantity,
                OriginalPrice = asset.OriginalPrice,
                AnnualDecreciation = asset.AnnualDecreciation
            };
        }

        /// <summary>
        /// Chuyển đổi thông tin tài sản sao chép sang CloneAssetDto
        /// </summary>
        /// <param name="clonedAsset">Thực thể Asset đã được sao chép</param>
        /// <param name="originalAssetCode">Mã tài sản gốc</param>
        /// <param name="department">Thông tin phòng ban</param>
        /// <param name="assetType">Thông tin loại tài sản</param>
        /// <returns>Đối tượng CloneAssetDto đã được ánh xạ</returns>
        public static CloneAssetDto ToCloneAssetDto(Asset clonedAsset, string originalAssetCode,
        Department department, AssetType assetType)
        {
            return new CloneAssetDto
            {
                AssetCode = originalAssetCode,
                NewAssetCode = clonedAsset.AssetCode,
                ClonedAsset = ToAssetResponseDto(clonedAsset, department, assetType)
            };
        }
    }
}
