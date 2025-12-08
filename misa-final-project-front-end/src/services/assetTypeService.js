import { ApiService } from './api'
import { API_ENDPOINTS } from '@/utils/constants'

export class AssetTypeService extends ApiService {
  constructor() {
    super()
    this.endpoint = API_ENDPOINTS.ASSET_TYPES
  }

  // Lấy tất cả loại tài sản
  async getAllAssetTypes() {
    try {
      const response = await this.get(this.endpoint)
      return this.handleResponse(response)
    } catch (error) {
      return this.handleError(error)
    }
  }

  // Lấy loại tài sản theo mã
  async getAssetTypeByCode(code) {
    try {
      const response = await this.get(`${this.endpoint}/${code}`)
      return this.handleResponse(response)
    } catch (error) {
      return this.handleError(error)
    }
  }
}

export const assetTypeService = new AssetTypeService()
