// src/composables/useAsset.js
import { ref } from 'vue'
import { assetService } from '@/services'

export function useAsset() {
  const loading = ref(false)
  const error = ref(null)
  const assets = ref([])
  
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

      // Gọi API
      const response = await assetService.getAssetsPaged(params)
      console.log('API Response:', response) // Log để debug

      // ĐIỀU CHỈNH: Response từ API có cấu trúc { success, message, data, ... }
      // data chứa: { pageNumber, pageSize, totalPages, totalRecords, hasPrevious, hasNext, data: [...] }
      if (response && response.success && response.data) {
        const apiData = response.data
        
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
          depreciation: item.annualDecreciation,
          remainingValue: item.originalPrice - item.annualDecreciation,
          departmentCode: item.departmentCode,
          assetTypeCode: item.assetTypeCode,
          useYear: item.useYear,
          usefulLife: item.usefulLife,
          trackingStartYear: item.trackingStartYear,
          decreciationRate: item.decreciationRate,
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
        console.warn('No data returned from API or API returned error:', response)
      }
    } catch (err) {
      error.value = err.message || err.response?.message || 'Có lỗi xảy ra khi tải dữ liệu'
      console.error('Error fetching assets:', err)
      assets.value = []
    } finally {
      loading.value = false
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
    pagination,
    filters: currentFilters(),

    // Methods
    fetchAssets,
    setFilter,
    resetFilters,
    changePage,
    changePageSize,
  }
}
