import { ref } from 'vue'
import { assetService } from '@/services'

export function useAsset() {
  const loading = ref(false)
  const error = ref(null)
  const assets = ref([])
  const statistics = ref({
    totalQuantity: 0,
    totalOriginalPrice: 0,
    totalAnnualDecreciation: 0,
    totalRemainingValue: 0
  })
  
  const filters = ref({
    searchKeyword: '',
    departmentCode: '',
    assetTypeCode: ''
  })
  
  const pagination = ref({
    pageNumber: 1,
    pageSize: 10,
    totalPages: 0,
    totalRecords: 0,
    hasPrevious: false,
    hasNext: false,
  })

  // Tải dữ liệu tài sản với các tham số
  const fetchAssets = async (customParams = {}) => {
    loading.value = true
    error.value = null

    try {
      const params = {
        PageNumber: pagination.value.pageNumber,
        PageSize: pagination.value.pageSize,
        SearchKeyword: filters.value.searchKeyword,
        DepartmentCode: filters.value.departmentCode,
        AssetTypeCode: filters.value.assetTypeCode,
        ...customParams
      }

      // Gọi API song song: lấy danh sách và thống kê
      const [assetsResponse, statsResponse] = await Promise.all([
        assetService.getAssetsPaged(params),
        assetService.getAssetStatistics({
          SearchKeyword: filters.value.searchKeyword,
          DepartmentCode: filters.value.departmentCode,
          AssetTypeCode: filters.value.assetTypeCode,
        })
      ]);

      // Xử lý dữ liệu tài sản
      if (assetsResponse && assetsResponse.success && assetsResponse.data) {
        const apiData = assetsResponse.data
        
        // Map dữ liệu từ API sang format cho table
        assets.value = apiData.data.map((item) => ({
          key: item.assetId,
          assetId: item.assetId,
          assetCode: item.assetCode,
          assetName: item.assetName,
          assetType: item.assetTypeName,
          department: item.departmentName,
          quantity: item.quantity,
          originalPrice: item.originalPrice,
          decreciation: item.annualDecreciation,
          remainingValue: item.originalPrice - item.annualDecreciation,
          departmentCode: item.departmentCode,
          assetTypeCode: item.assetTypeCode,
          useYear: item.useYear,
          usefulLife: item.usefulLife,
          trackingStartYear: item.trackingStartYear,
          decreciationRate: item.decreciationRate,
          purchaseDate: item.purchaseDate,
          startDate: item.startDate,
          createdDate: item.createdDate
        }))

        // Cập nhật pagination info
        pagination.value = {
          pageNumber: apiData.pageNumber,
          pageSize: apiData.pageSize,
          totalPages: apiData.totalPages,
          totalRecords: apiData.totalRecords,
          hasPrevious: apiData.hasPrevious,
          hasNext: apiData.hasNext,
        }
      } else {
        assets.value = []
        console.warn('No data returned from API or API returned error:', assetsResponse)
      }

      // Xử lý thống kê
      if (statsResponse && statsResponse.success && statsResponse.data) {
        statistics.value = statsResponse.data
      } else {
        // Reset thống kê nếu có lỗi
        statistics.value = {
          totalQuantity: 0,
          totalOriginalPrice: 0,
          totalAnnualDecreciation: 0,
          totalRemainingValue: 0
        }
      }
    } catch (err) {
      error.value = err.message || err.response?.message || 'Có lỗi xảy ra khi tải dữ liệu'
      console.error('Error fetching assets:', err)
      assets.value = []
      statistics.value = {
        totalQuantity: 0,
        totalOriginalPrice: 0,
        totalAnnualDecreciation: 0,
        totalRemainingValue: 0
      }
    } finally {
      loading.value = false
    }
  }

  // Hàm lấy thống kê riêng
  const fetchStatistics = async (customParams = {}) => {
    try {
      const params = {
        SearchKeyword: filters.value.searchKeyword,
        DepartmentCode: filters.value.departmentCode,
        AssetTypeCode: filters.value.assetTypeCode,
        ...customParams
      }

      const response = await assetService.getAssetStatistics(params)
      if (response && response.success && response.data) {
        statistics.value = response.data
      }
    } catch (err) {
      console.error('Error fetching statistics:', err)
    }
  }

  // Set filter values
  const setFilter = (key, value) => {
    filters.value[key] = value
  }

  // Reset tất cả filters
  const resetFilters = () => {
    filters.value = {
      searchKeyword: '',
      departmentCode: '',
      assetTypeCode: ''
    }
    pagination.value.pageNumber = 1
  }

  // Thay đổi trang
  const changePage = (pageNumber) => {
    if (pageNumber >= 1) {
      pagination.value.pageNumber = pageNumber
      return true
    }
    return false
  }

  // Thay đổi kích thước trang
  const changePageSize = (pageSize) => {
    if (pageSize > 0) {
      pagination.value.pageSize = pageSize
      pagination.value.pageNumber = 1 // Reset về trang đầu
      return true
    }
    return false
  }

  // Computed property để lấy filter values
  const currentFilters = () => filters.value

  return {
    // State
    loading,
    error,
    assets,
    statistics,
    pagination,
    filters: currentFilters(),

    // Methods
    fetchAssets,
    fetchStatistics,
    setFilter,
    resetFilters,
    changePage,
    changePageSize,
  }
}