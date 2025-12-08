// src/composables/useDropdownData.js
import { ref, onMounted } from 'vue'
import { assetTypeService, departmentService } from '@/services'

export function useDropdownData() {
  // Options cho filter (hiển thị tên)
  const assetTypeOptions = ref([])
  const departmentOptions = ref([])

  // Options cho form (hiển thị mã)
  const assetTypeOptionsForForm = ref([])
  const departmentOptionsForForm = ref([])

  const isLoading = ref(false)
  const error = ref(null)

  const loadData = async () => {
    isLoading.value = true
    error.value = null

    try {
      // Load asset types
      const assetTypes = await assetTypeService.getAllAssetTypes()
      console.log('AssetTypes API Response:', assetTypes)

      if (assetTypes && assetTypes.success && assetTypes.data) {
        // Tạo options cho filter (hiển thị tên) - DÙNG CHO GIAO DIỆN LỌC
        assetTypeOptions.value = assetTypes.data.map((item) => ({
          value: item.assetTypeCode, // Giá trị thực tế là mã
          label: item.assetTypeName, // Hiển thị tên
          data: {
            ...item,
            usefulLife: item.usefulLife,
            depreciationRate: item.recreciationRate,
          },
        }))

        // Tạo options cho form (hiển thị mã) - DÙNG CHO ASSET POPUP
        assetTypeOptionsForForm.value = assetTypes.data.map((item) => ({
          value: item.assetTypeCode,
          label: item.assetTypeCode, // HIỂN THỊ MÃ
          data: {
            ...item,
            usefulLife: item.usefulLife,
            depreciationRate: item.recreciationRate,
            assetTypeName: item.assetTypeName, // Lưu tên để dùng khi cần
          },
        }))
      } else {
        console.warn('AssetTypes API returned error or no data:', assetTypes)
        assetTypeOptions.value = []
        assetTypeOptionsForForm.value = []
      }

      // Load departments
      const departments = await departmentService.getAllDepartments()
      console.log('Departments API Response:', departments)

      if (departments && departments.success && departments.data) {
        // Tạo options cho filter (hiển thị tên) - DÙNG CHO GIAO DIỆN LỌC
        departmentOptions.value = departments.data.map((item) => ({
          value: item.departmentCode, // Giá trị thực tế là mã
          label: item.departmentName, // Hiển thị tên
          data: item,
        }))

        // Tạo options cho form (hiển thị mã) - DÙNG CHO ASSET POPUP
        departmentOptionsForForm.value = departments.data.map((item) => ({
          value: item.departmentCode,
          label: item.departmentCode, // HIỂN THỊ MÃ
          data: {
            ...item,
            departmentName: item.departmentName, // Lưu tên để dùng khi cần
          },
        }))
      } else {
        console.warn('Departments API returned error or no data:', departments)
        departmentOptions.value = []
        departmentOptionsForForm.value = []
      }
    } catch (err) {
      error.value = err.message || 'Có lỗi xảy ra khi tải dữ liệu dropdown'
      console.error('Error loading dropdown data:', err)

      assetTypeOptions.value = []
      departmentOptions.value = []
      assetTypeOptionsForForm.value = []
      departmentOptionsForForm.value = []
    } finally {
      isLoading.value = false
    }
  }

  // Hàm lấy thông tin asset type theo code
  const getAssetTypeByCode = (code) => {
    return assetTypeOptions.value.find((opt) => opt.value === code)?.data
  }

  // Hàm lấy thông tin department theo code
  const getDepartmentByCode = (code) => {
    return departmentOptions.value.find((opt) => opt.value === code)?.data
  }

  onMounted(() => {
    loadData()
  })

  return {
    // Cho filter (giao diện lọc)
    assetTypeOptions,
    departmentOptions,
    // Cho form (AssetPopup)
    assetTypeOptionsForForm,
    departmentOptionsForForm,
    isLoading,
    error,
    loadData,
    reload: loadData,
    getAssetTypeByCode,
    getDepartmentByCode,
  }
}
