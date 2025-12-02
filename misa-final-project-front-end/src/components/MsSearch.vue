<template>
    <div class="ms-search-wrapper w-full flex items-center">
        <a-input-search :value="modelValue" :disabled="disabled" :placeholder="placeholder" :loading="loading"
            :status="error ? 'error' : ''" @update:value="handleUpdateValue" @search="handleSearch" v-bind="$attrs"
            class="ms-search h-[40px] text-[14px] font-roboto rounded-[4px]" :class="searchClasses">
            <template #prefix>
                <span :class="iconSearchClass"></span>
            </template>
        </a-input-search>
    </div>
</template>
 
<script setup>
// Giữ nguyên phần Script
import { computed } from 'vue';

// Định nghĩa Props
const props = defineProps({
    // Giá trị v-model
    modelValue: {
        type: String,
        default: '',
    },
    // Placeholder mặc định
    placeholder: {
        type: String,
        default: 'Tìm kiếm tài sản',
    },
    // Trạng thái disable
    disabled: {
        type: Boolean,
        default: false,
    },
    // Trạng thái loading (từ Antd)
    loading: {
        type: Boolean,
        default: false,
    },
    // Trạng thái lỗi (viền đỏ)
    error: {
        type: Boolean,
        default: false,
    },
    // Tên class icon search do người dùng khai báo trong CSS
    iconSearchClass: {
        type: String,
        default: 'icon-search',
    }
});

// Định nghĩa Emits
const emit = defineEmits(['update:modelValue', 'search']);

// Xử lý cập nhật value cho v-model
const handleUpdateValue = (value) => {
    emit('update:modelValue', value);
};

// Xử lý sự kiện tìm kiếm (Enter hoặc click icon nếu có)
const handleSearch = (value) => {
    emit('search', value);
};

// Logic classes để ghi đè style Ant Design, đồng bộ với MSInput
const searchClasses = computed(() => {
    if (props.error) {
        return 'hover:!border-[#ff4d4f] focus:!border-[#ff4d4f] focus:!shadow-none';
    }

    return `
      hover:!border-[#0097c2] 
      focus:!border-[#0097c2] 
      focus:!shadow-[0_0_0_2px_rgba(0,151,194,0.2)]
    `;
});
</script>
 
<style scoped>
/* Deep Selectors để ghi đè style của Ant Design cho khớp design system. */

/* Áp dụng style cho toàn bộ input field (bao gồm cả selector) */
.ms-search-wrapper :deep(.ant-input-affix-wrapper) {
    height: 40px !important;
    border-radius: 4px !important;
    font-family: 'Roboto', sans-serif;
    font-size: 14px;
    padding: 0 11px !important;
}

/* Ghi đè style của Input Search */
.ms-search-wrapper :deep(.ant-input-search .ant-input) {
    font-family: 'Roboto', sans-serif;
    font-size: 14px;
}

/* 🛑 THAY ĐỔI QUAN TRỌNG: Ẩn nút tìm kiếm bên phải */
.ms-search-wrapper :deep(.ant-input-search-button) {
    display: none !important;
}

/* 🛑 THAY ĐỔI QUAN TRỌNG: Điều chỉnh lại border và padding của wrapper sau khi ẩn nút */
.ms-search-wrapper :deep(.ant-input-search) {
    /* Quan trọng: Đảm bảo Input Search wrapper không có margin/padding thừa do nút search đã bị ẩn */
    padding-inline-end: 0 !important;
    /* Loại bỏ padding mặc định nếu có */
}

/* Các state còn lại giữ nguyên */
.ms-search-wrapper :deep(.ant-input-search:not(.ant-input-search-customize):not(.ant-input-search-rtl) .ant-input-affix-wrapper) {
    border-color: #d9d9d9;
    box-shadow: none !important;
}

.ms-search-wrapper :deep(.ant-input-search:not(.ant-input-search-customize):not(.ant-input-search-rtl) .ant-input-affix-wrapper:hover) {
    border-color: #0097c2 !important;
}

.ms-search-wrapper :deep(.ant-input-search.ant-input-search-focus .ant-input-affix-wrapper) {
    border-color: #0097c2 !important;
    box-shadow: 0 0 0 2px rgba(0, 151, 194, 0.2) !important;
}

.ms-search-wrapper :deep(.ant-input-search.ant-input-search-disabled .ant-input-affix-wrapper) {
    background-color: #f5f5f5 !important;
    border-color: #d9d9d9 !important;
}

.ms-search-wrapper :deep(.ant-input-affix-wrapper-disabled .ant-input-prefix) {
    color: #bfbfbf;
}
</style>