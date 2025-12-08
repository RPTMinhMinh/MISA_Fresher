import useApi from '@/composables/useApi';

export class ApiService {
    constructor() {
        const { 
            loading, 
            error, 
            data, 
            get, 
            post, 
            put, 
            delete: del,
            getWithPagination,
            reset 
        } = useApi();
        
        this.loading = loading;
        this.error = error;
        this.data = data;
        this.get = get;
        this.post = post;
        this.put = put;
        this.delete = del;
        this.getWithPagination = getWithPagination;
        this.reset = reset;
    }

    // Phương thức helper để xử lý response
    handleResponse(response) {
        // QUAN TRỌNG: Trả về toàn bộ response từ API
        // Không tự động trích data vì mỗi API có cấu trúc khác nhau
        return response;
    }

    // Phương thức xử lý lỗi chung
    handleError(error) {
        console.error('API Service Error:', {
            message: error.message,
            response: error.response,
            stack: error.stack
        });
        
        throw error;
    }
}