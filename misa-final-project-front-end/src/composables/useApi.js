import { ref } from 'vue'
import axios from 'axios'
import { API_CONFIG, HTTP_STATUS } from '@/utils/constants'

export function useApi() {
  const loading = ref(false)
  const error = ref(null)
  const data = ref(null)

  // Instance axios với config mặc định
  const apiClient = axios.create({
    baseURL: `${API_CONFIG.BASE_URL}/api/${API_CONFIG.VERSION}`,
    timeout: API_CONFIG.TIMEOUT,
    headers: {
      'Content-Type': 'application/json',
      Accept: 'application/json',
    },
  })

  // Interceptor cho request
  apiClient.interceptors.request.use(
    (config) => {
      loading.value = true
      error.value = null
      return config
    },
    (error) => {
      loading.value = false
      return Promise.reject(error)
    },
  )

  // Interceptor cho response - KHÔNG thay đổi cấu trúc response
  apiClient.interceptors.response.use(
    (response) => {
      loading.value = false
      
      // QUAN TRỌNG: Trả về toàn bộ response.data, không chỉ trích data
      // API của bạn trả về: { success, message, data, errors, timestamp, statusCode }
      return response.data
    },
    (error) => {
      loading.value = false

      if (error.response) {
        const status = error.response.status
        const serverError = error.response.data

        // Xử lý lỗi theo format API của bạn
        if (serverError && typeof serverError === 'object') {
          error.value = serverError.message || serverError.errors?.[0] || `Lỗi ${status}`
        } else {
          switch (status) {
            case HTTP_STATUS.BAD_REQUEST:
              error.value = 'Yêu cầu không hợp lệ'
              break
            case HTTP_STATUS.UNAUTHORIZED:
              error.value = 'Không có quyền truy cập'
              break
            case HTTP_STATUS.FORBIDDEN:
              error.value = 'Truy cập bị từ chối'
              break
            case HTTP_STATUS.NOT_FOUND:
              error.value = 'Không tìm thấy tài nguyên'
              break
            case HTTP_STATUS.INTERNAL_SERVER_ERROR:
              error.value = 'Lỗi máy chủ nội bộ'
              break
            default:
              error.value = `Lỗi ${status}: Có lỗi xảy ra`
          }
        }
      } else if (error.request) {
        error.value = 'Không thể kết nối đến server'
      } else {
        error.value = error.message || 'Lỗi khi thiết lập request'
      }

      return Promise.reject(error)
    },
  )

  // HTTP GET
  const get = async (url, config = {}) => {
    try {
      return await apiClient.get(url, config)
    } catch (err) {
      throw err
    }
  }

  // HTTP POST
  const post = async (url, data, config = {}) => {
    try {
      return await apiClient.post(url, data, config)
    } catch (err) {
      throw err
    }
  }

  // HTTP PUT
  const put = async (url, data, config = {}) => {
    try {
      return await apiClient.put(url, data, config)
    } catch (err) {
      throw err
    }
  }

  // HTTP DELETE
  const del = async (url, data, config = {}) => {
    try {
      return await apiClient.delete(url, { ...config, data })
    } catch (err) {
      throw err
    }
  }

  // GET với phân trang và filter
  const getWithPagination = async (url, params = {}) => {
    try {
      const cleanParams = {}
      Object.keys(params).forEach(key => {
        if (params[key] !== undefined && params[key] !== null && params[key] !== '') {
          cleanParams[key] = params[key]
        }
      })

      const queryString = new URLSearchParams(cleanParams).toString()
      const fullUrl = queryString ? `${url}?${queryString}` : url
      return await get(fullUrl)
    } catch (err) {
      throw err
    }
  }

  // Reset state
  const reset = () => {
    loading.value = false
    error.value = null
    data.value = null
  }

  return {
    loading,
    error,
    data,
    get,
    post,
    put,
    delete: del,
    getWithPagination,
    reset,
    isLoading: loading,
    hasError: error,
    responseData: data,
  }
}

export default useApi