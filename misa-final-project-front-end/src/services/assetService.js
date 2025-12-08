import { ApiService } from './api'
import { API_ENDPOINTS } from '@/utils/constants'

export class AssetService extends ApiService {
  constructor() {
    super()
    this.endpoint = API_ENDPOINTS.ASSETS
  }

  // Lấy tài sản phân trang với filter
  async getAssetsPaged(params = {}) {
    try {
      // Tạo query params đúng format API yêu cầu
      const queryParams = {
        PageNumber: params.pageNumber || params.PageNumber || 1,
        PageSize: params.pageSize || params.PageSize || 10,
        SearchKeyword: params.searchKeyword || params.SearchKeyword || '',
        DepartmentCode: params.departmentCode || params.DepartmentCode || '',
        AssetTypeCode: params.assetTypeCode || params.AssetTypeCode || '',
      }

      // Xóa các param rỗng để không gửi lên server
      Object.keys(queryParams).forEach((key) => {
        if (queryParams[key] === '' || queryParams[key] === undefined) {
          delete queryParams[key]
        }
      })

      const response = await this.getWithPagination(`${this.endpoint}`, queryParams)
      return this.handleResponse(response)
    } catch (error) {
      return this.handleError(error)
    }
  }

  // Lấy tài sản theo mã
  async getAssetByCode(assetCode) {
    try {
      const response = await this.get(`${this.endpoint}/code/${assetCode}`)
      return this.handleResponse(response)
    } catch (error) {
      return this.handleError(error)
    }
  }

  // Tạo tài sản mới
  async createAsset(assetData) {
    try {
      const response = await this.post(this.endpoint, assetData)
      return this.handleResponse(response)
    } catch (error) {
      return this.handleError(error)
    }
  }

  // Cập nhật tài sản
  async updateAsset(assetCode, updateData) {
    try {
      // THÊM: Validate assetCode
      if (!assetCode) {
        throw new Error('Asset code is required for update')
      }

      const response = await this.put(`${this.endpoint}/${assetCode}`, updateData)
      return this.handleResponse(response)
    } catch (error) {
      return this.handleError(error)
    }
  }

  // Xóa nhiều tài sản
  async deleteAssets(assetCodes) {
    try {
      // Gửi request DELETE với body chứa danh sách mã tài sản
      const response = await this.delete(this.endpoint, { assetCodes })
      return this.handleResponse(response)
    } catch (error) {
      return this.handleError(error)
    }
  }

  // Nhân bản tài sản
  async cloneAsset(assetCode) {
    try {
      const response = await this.get(`${this.endpoint}/${assetCode}/clone`)
      return this.handleResponse(response)
    } catch (error) {
      return this.handleError(error)
    }
  }

  // Lấy mã tài sản tiếp theo
  async getNextAssetCode() {
    try {
      const response = await this.get(`${this.endpoint}/next-code`)
      return this.handleResponse(response)
    } catch (error) {
      return this.handleError(error)
    }
  }

  // Phương thức helper tạo pagination params
  createPaginationParams(page = 1, pageSize = 10, filters = {}) {
    return {
      pageNumber: page,
      pageSize: pageSize,
      ...filters,
    }
  }
}

export const assetService = new AssetService()
