import { assetService } from './assetService'
import { assetTypeService } from './assetTypeService'
import { departmentService } from './departmentService'

export const useServices = () => {
  return {
    asset: assetService,
    assetType: assetTypeService,
    department: departmentService,
  }
}

// Export riêng lẻ để import dễ dàng
export { assetService, assetTypeService, departmentService }
