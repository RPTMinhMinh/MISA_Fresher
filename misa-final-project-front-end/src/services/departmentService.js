import { ApiService } from './api'
import { API_ENDPOINTS } from '@/utils/constants'

export class DepartmentService extends ApiService {
  constructor() {
    super()
    this.endpoint = API_ENDPOINTS.DEPARTMENTS
  }

  // Lấy tất cả bộ phận
  async getAllDepartments() {
    try {
      const response = await this.get(this.endpoint)
      return this.handleResponse(response)
    } catch (error) {
      return this.handleError(error)
    }
  }

  // Lấy bộ phận theo mã
  async getDepartmentByCode(code) {
    try {
      const response = await this.get(`${this.endpoint}/${code}`)
      return this.handleResponse(response)
    } catch (error) {
      return this.handleError(error)
    }
  }
}

export const departmentService = new DepartmentService()
