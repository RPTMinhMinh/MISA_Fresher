using MISA.QLTS.Core.Dtos;
using MISA.QLTS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Core.Mappers
{
    public static class AssetMapper
    {
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

        public static Asset ToEntity(CreateAssetDto dto, Guid assetId, string assetCode,
         Guid departmentId, Guid assetTypeId, decimal usefulLife, decimal decreciationRate)
        {
            return new Asset
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
                DecreciationRate = decreciationRate
            };
        }

        public static void CalculateAssetFields(Asset asset, AssetType assetType)
        {
            // Trường năm sử dụng = năm của ngày mua
            asset.UseYear = asset.CreatedDate.Year;

            // Trường năm bắt đầu theo dõi = năm của ngày mua
            asset.TrackingStartYear = asset.CreatedDate.Year;

            // Số năm sử dụng lấy từ loại tài sản
            asset.UsefulLife = assetType.UsefulLife;

            // Tỉ lệ hao mòn lấy từ loại tài sản
            asset.DecreciationRate = assetType.RecreciationRate;

            // Giá trị hao mòn năm = Nguyên giá * Tỉ lệ hao mòn / 100
            asset.AnnualDecreciation = Math.Round(
                asset.OriginalPrice * asset.DecreciationRate / 100,
                4,
                MidpointRounding.AwayFromZero);
        }

        public static Asset UpdateEntity(Asset asset, UpdateAssetDto dto,
        Department? department = null, AssetType? assetType = null,
        Department? oldDepartment = null, AssetType? oldAssetType = null)
        {
            if (!string.IsNullOrWhiteSpace(dto.AssetName))
                asset.AssetName = dto.AssetName;

            if (department != null) asset.DepartmentId = department.DepartmentId;

            if (assetType != null) asset.AssetTypeId = assetType.AssetTypeId;

            if (dto.Quantity.HasValue) asset.Quantity = dto.Quantity.Value;

            if (dto.OriginalPrice.HasValue) asset.OriginalPrice = dto.OriginalPrice.Value;

            if (dto.CreatedDate.HasValue) asset.CreatedDate = dto.CreatedDate.Value;

            // Nếu có thay đổi về department, assetType, createdDate, hoặc originalPrice
            // thì cần tính toán lại các trường phụ thuộc
            if (department != null || assetType != null || dto.CreatedDate.HasValue || dto.OriginalPrice.HasValue)
            {
                // Lấy assetType mới nếu có, nếu không dùng assetType cũ
                var currentAssetType = assetType ?? oldAssetType;
                if (currentAssetType != null)
                {
                    CalculateAssetFields(asset, currentAssetType);
                }
            }

            return asset;
        }

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
