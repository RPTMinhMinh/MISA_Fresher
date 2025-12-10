using MISA.QLTS.Core.Dtos;
using MISA.QLTS.Core.Dtos.Common;
using MISA.QLTS.Core.Entities;
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
    public class AssetService : IAssetService
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IAssetTypeRepository _assetTypeRepository;
        public AssetService(IAssetRepository assetRepository, IDepartmentRepository departmentRepository, IAssetTypeRepository assetTypeRepository)
        {
            _assetRepository = assetRepository;
            _departmentRepository = departmentRepository;
            _assetTypeRepository = assetTypeRepository;
        }
        public async Task<AssetResponseDto> CreateAssetAsync(CreateAssetDto createAssetDto)
        {
            var department = await _departmentRepository.GetByCodeAsync(createAssetDto.DepartmentCode);
            if (department == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy bộ phận với mã: {createAssetDto.DepartmentCode}");
            }

            var assetType = await _assetTypeRepository.GetByCodeAsync(createAssetDto.AssetTypeCode);
            if (assetType == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy loại tài sản với mã: {createAssetDto.AssetTypeCode}");
            }

            var nextAssetCode = await _assetRepository.GetNextAssetCodeAsync();

            var asset = AssetMapper.ToEntity(
                createAssetDto,
                Guid.NewGuid(),
                nextAssetCode,
                department.DepartmentId,
                assetType.AssetTypeId,
                assetType.UsefulLife,
                assetType.RecreciationRate
            );

            AssetMapper.CalculateAssetFields(asset, assetType);

            var createdAsset = await _assetRepository.CreateAsset(asset);

            return AssetMapper.ToAssetResponseDto(createdAsset, department, assetType);
        }

        public async Task<AssetResponseDto?> GetAssetByCodeAsync(string assetCode)
        {
            var asset = await _assetRepository.GetByCodeAsync(assetCode);
            if (asset == null) return null;

            var department = await _departmentRepository.GetByCodeAsync(
            (await _departmentRepository.GetAllAsync())
            .FirstOrDefault(d => d.DepartmentId == asset.DepartmentId)?.DepartmentCode ?? string.Empty);

            var assetType = await _assetTypeRepository.GetByCodeAsync(
                (await _assetTypeRepository.GetAllAsync())
                .FirstOrDefault(at => at.AssetTypeId == asset.AssetTypeId)?.AssetTypeCode ?? string.Empty);

            if (department == null || assetType == null) return null;
            return AssetMapper.ToAssetResponseDto(asset, department, assetType);
        }

        public async Task<PaginatedResult<AssetResponseDto>> GetAssetsPaginatedAsync(PaginationRequest request)
        {
            // Validate department code nếu có
            if (!string.IsNullOrWhiteSpace(request.DepartmentCode))
            {
                var department = await _departmentRepository.GetByCodeAsync(request.DepartmentCode);
                if (department == null)
                {
                    return new PaginatedResult<AssetResponseDto>(
                        new List<AssetResponseDto>(),
                        request.PageNumber,
                        request.PageSize,
                        0
                    );
                }
            }

            // Validate asset type code nếu có
            if (!string.IsNullOrWhiteSpace(request.AssetTypeCode))
            {
                var assetType = await _assetTypeRepository.GetByCodeAsync(request.AssetTypeCode);
                if (assetType == null)
                {
                    return new PaginatedResult<AssetResponseDto>(
                        new List<AssetResponseDto>(),
                        request.PageNumber,
                        request.PageSize,
                        0
                    );
                }
            }

            // Lấy dữ liệu phân trang từ repository
            var assets = await _assetRepository.GetPagedAsync(request);
            var totalRecords = await _assetRepository.GetTotalCountAsync(
                request.SearchKeyword,
                request.DepartmentCode,
                request.AssetTypeCode);

            // Lấy tất cả department và asset type để map
            var departments = (await _departmentRepository.GetAllAsync()).ToDictionary(d => d.DepartmentId);
            var assetTypes = (await _assetTypeRepository.GetAllAsync()).ToDictionary(at => at.AssetTypeId);

            var assetDtos = new List<AssetResponseDto>();

            foreach (var asset in assets)
            {
                if (departments.TryGetValue(asset.DepartmentId, out var department) &&
                    assetTypes.TryGetValue(asset.AssetTypeId, out var assetType))
                {
                    assetDtos.Add(AssetMapper.ToAssetResponseDto(asset, department, assetType));
                }
            }

            return new PaginatedResult<AssetResponseDto>(
                assetDtos,
                request.PageNumber,
                request.PageSize,
                totalRecords
            );
        }

        public async Task<CloneAssetDto> CloneAssetAsync(string assetCode)
        {
            // 1. Lấy thông tin tài sản gốc
            var originalAsset = await _assetRepository.GetByCodeAsync(assetCode);
            if (originalAsset == null)
                throw new KeyNotFoundException($"Không tìm thấy tài sản với mã: {assetCode}");

            // 2. Lấy thông tin department và asset type của tài sản gốc
            var department = await _departmentRepository.GetByCodeAsync(
                (await _departmentRepository.GetAllAsync())
                .FirstOrDefault(d => d.DepartmentId == originalAsset.DepartmentId)?.DepartmentCode ?? string.Empty);

            var assetType = await _assetTypeRepository.GetByCodeAsync(
                (await _assetTypeRepository.GetAllAsync())
                .FirstOrDefault(at => at.AssetTypeId == originalAsset.AssetTypeId)?.AssetTypeCode ?? string.Empty);

            if (department == null || assetType == null)
                throw new InvalidOperationException("Không thể lấy thông tin bộ phận hoặc loại tài sản");

            // 3. Tạo mã tài sản mới
            var newAssetCode = await GetNextAssetCodeAsync();

            // 4. Clone tài sản (chỉ trong memory, không lưu database)
            var clonedAsset = AssetMapper.CloneAsset(originalAsset, newAssetCode);

            // 5. Trả về DTO
            return AssetMapper.ToCloneAssetDto(clonedAsset, assetCode, department, assetType);
        }

        public async Task<AssetResponseDto> UpdateAssetAsync(string assetCode, UpdateAssetDto updateAssetDto)
        {
            // 1. Lấy thông tin tài sản hiện tại
            var existingAsset = await _assetRepository.GetByCodeAsync(assetCode);
            if (existingAsset == null)
                throw new KeyNotFoundException($"Không tìm thấy tài sản với mã: {assetCode}");

            // 2. Lấy thông tin department và asset type cũ
            var oldDepartment = await _departmentRepository.GetByCodeAsync(
                (await _departmentRepository.GetAllAsync())
                .FirstOrDefault(d => d.DepartmentId == existingAsset.DepartmentId)?.DepartmentCode ?? string.Empty);

            var oldAssetType = await _assetTypeRepository.GetByCodeAsync(
                (await _assetTypeRepository.GetAllAsync())
                .FirstOrDefault(at => at.AssetTypeId == existingAsset.AssetTypeId)?.AssetTypeCode ?? string.Empty);

            if (oldDepartment == null || oldAssetType == null)
                throw new InvalidOperationException("Không thể lấy thông tin bộ phận hoặc loại tài sản");

            // 3. Validate và lấy thông tin department mới nếu có
            Department? newDepartment = null;
            if (!string.IsNullOrWhiteSpace(updateAssetDto.DepartmentCode))
            {
                newDepartment = await _departmentRepository.GetByCodeAsync(updateAssetDto.DepartmentCode);
                if (newDepartment == null)
                    throw new KeyNotFoundException($"Không tìm thấy bộ phận với mã: {updateAssetDto.DepartmentCode}");
            }

            // 4. Validate và lấy thông tin asset type mới nếu có
            AssetType? newAssetType = null;
            if (!string.IsNullOrWhiteSpace(updateAssetDto.AssetTypeCode))
            {
                newAssetType = await _assetTypeRepository.GetByCodeAsync(updateAssetDto.AssetTypeCode);
                if (newAssetType == null)
                    throw new KeyNotFoundException($"Không tìm thấy loại tài sản với mã: {updateAssetDto.AssetTypeCode}");
            }

            // 5. Cập nhật entity
            var updatedAsset = AssetMapper.UpdateEntity(
                existingAsset,
                updateAssetDto,
                newDepartment,
                newAssetType,
                oldDepartment,
                oldAssetType
            );

            // 6. Lưu vào database
            await _assetRepository.UpdateAsync(updatedAsset);

            // 7. Lấy thông tin department và asset type cuối cùng để trả về
            var finalDepartment = newDepartment ?? oldDepartment;
            var finalAssetType = newAssetType ?? oldAssetType;

            return AssetMapper.ToAssetResponseDto(updatedAsset, finalDepartment, finalAssetType);
        }

        public async Task<int> DeleteAssetsAsync(List<string> assetCodes)
        {
            if (assetCodes == null || !assetCodes.Any())
                throw new ArgumentException("Danh sách mã tài sản không được để trống");

            var deletedCount = 0;

            foreach (var assetCode in assetCodes.Distinct())
            {
                var asset = await _assetRepository.GetByCodeAsync(assetCode);
                if (asset != null)
                {
                    await _assetRepository.DeleteAsync(asset.AssetCode);
                    deletedCount++;
                }
            }

            return deletedCount;
        }

        public async Task<string> GetNextAssetCodeAsync()
        {
            return await _assetRepository.GetNextAssetCodeAsync();
        }

        public async Task<AssetStatisticsDto> GetAssetStatisticsAsync(AssetStatisticsRequest request)
        {
            // Validate department code nếu có
            if (!string.IsNullOrWhiteSpace(request.DepartmentCode))
            {
                var department = await _departmentRepository.GetByCodeAsync(request.DepartmentCode);
                if (department == null)
                {
                    // Nếu department không tồn tại, trả về thống kê 0
                    return new AssetStatisticsDto();
                }
            }

            // Validate asset type code nếu có
            if (!string.IsNullOrWhiteSpace(request.AssetTypeCode))
            {
                var assetType = await _assetTypeRepository.GetByCodeAsync(request.AssetTypeCode);
                if (assetType == null)
                {
                    // Nếu asset type không tồn tại, trả về thống kê 0
                    return new AssetStatisticsDto();
                }
            }

            // Lấy thống kê từ repository
            var statistics = await _assetRepository.GetAssetStatisticsAsync(
                request.SearchKeyword,
                request.DepartmentCode,
                request.AssetTypeCode);

            return statistics;
        }
    }
}
