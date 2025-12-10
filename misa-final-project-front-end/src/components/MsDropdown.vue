<template>
    <div class="flex flex-col w-full gap-1 ms-dropdown-wrapper">
        <label v-if="label" class="text-[13px] font-medium text-gray-700 font-roboto flex items-center gap-1" :class="{ 'opacity-50': disabled }">
            {{ label }}
            <span v-if="required" class="text-[6px] text-[#ff4d4f] leading-none mt-[-2px]">*</span>
        </label>

        <a-select :value="modelValue" :mode="mode" :options="standardOptions" :disabled="disabled"
            :placeholder="placeholder" :show-search="true" :filter-option="filterOption" :status="error ? 'error' : ''"
            @update:value="handleUpdateValue" v-bind="$attrs" class="ms-dropdown h-[40px] text-[14px] font-roboto">

            <template #prefix v-if="iconPrefixClass">
                <span :class="iconPrefixClass"
                    class="ms-dropdown-prefix-icon text-gray-500 text-sm mr-2 w-4 h-4 inline-block"></span>
            </template>
            <template #option="{ value: optionValue, label: optionLabel, code }">
                <div v-if="displayMode === 'table'" class="flex justify-between w-full font-roboto text-sm">
                    <span class="text-gray-500 w-1/4">{{ code }}</span>
                    <span class="w-3/4">{{ optionLabel }}</span>
                </div>

                <div v-else-if="displayMode === 'checkbox'" class="flex items-center gap-2">
                    <input type="checkbox" :checked="isOptionSelected(optionValue)"
                        class="w-4 h-4 text-cyan-600 bg-gray-100 border-gray-300 rounded" />
                    <span>{{ optionLabel }}</span>
                </div>

                <span v-else>{{ optionLabel }}</span>
            </template>
        </a-select>

        <span v-if="error && errorMessage" class="text-[12px] text-[#ff4d4f] font-roboto mt-0.5">
            {{ errorMessage }}
        </span>
    </div>
</template>
 
<script setup>
import { computed } from 'vue';

// Định nghĩa Props
const props = defineProps({
    // Giá trị v-model (có thể là string/number cho single, hoặc Array cho multiple)
    modelValue: {
        type: [String, Number, Array],
        default: undefined,
    },
    // Label hiển thị bên trên
    label: {
        type: String,
        default: '',
    },
    // Danh sách options. Dùng 'Array' thay vì ép kiểu TS
    options: {
        type: Array,
        required: true,
    },
    // Loại lựa chọn: 'default' | 'multiple'. Dùng custom validator thay vì ép kiểu TS
    mode: {
        type: String,
        default: 'default',
        validator: (value) => ['default', 'multiple'].includes(value)
    },
    // Chế độ hiển thị option: 'default' | 'table' | 'checkbox'
    displayMode: {
        type: String,
        default: 'default',
        validator: (value) => ['default', 'table', 'checkbox'].includes(value)
    },
    // Trạng thái disable
    disabled: {
        type: Boolean,
        default: false,
    },
    // Trạng thái lỗi (viền đỏ)
    error: {
        type: Boolean,
        default: false,
    },
    // Thông báo lỗi
    errorMessage: {
        type: String,
        default: '',
    },
    placeholder: {
        type: String,
        default: 'Chọn giá trị',
    },
    // 🛑 PHẦN SỬA ĐỔI: Thêm prop cho class icon prefix
    iconPrefixClass: {
        type: String,
        default: '' // Mặc định không có icon
    },
    // Thêm prop required (bắt buộc)
    required: {
        type: Boolean,
        default: false,
    }
});

// Định nghĩa Emits
const emit = defineEmits(['update:modelValue', 'change']);

// Chuyển đổi options sang định dạng Ant Design tiêu chuẩn nếu cần (Logic giữ nguyên)
const standardOptions = computed(() => {
    // Đảm bảo props.options tồn tại trước khi map
    return props.options?.map(option => ({
        ...option,
        // Dùng toán tử || để đảm bảo có value và label
        value: option.value || option.code,
        label: option.label || option.name,
    })) || [];
});

// Xử lý cập nhật value cho v-model
const handleUpdateValue = (value, option) => { // Loại bỏ type annotations
    emit('update:modelValue', value);
    emit('change', value, option);
};

// Logic tìm kiếm/lọc
const filterOption = (input, option) => { // Loại bỏ type annotations
    // Tìm kiếm trên cả value và label (hoặc name/code nếu là table mode)
    const inputValue = input.toLowerCase();
    const label = option.label?.toLowerCase() || '';
    const code = option.code?.toLowerCase() || '';

    return label.includes(inputValue) || code.includes(inputValue);
};

// Kiểm tra option đã được chọn chưa (chủ yếu cho displayMode='checkbox')
const isOptionSelected = (optionValue) => { // Loại bỏ type annotations
    if (Array.isArray(props.modelValue)) {
        return props.modelValue.includes(optionValue);
    }
    return false;
}

</script>
  
<style scoped>
/* Giữ nguyên Style đã override Ant Design */

/* Đảm bảo chiều cao, font, và bo góc cho Select */
.ms-dropdown-wrapper :deep(.ant-select-selector) {
    height: 40px !important;
    border-radius: 4px !important;
    /* 🛑 Cần điều chỉnh padding-left nếu có prefix */
    padding: 0 11px !important;
    font-family: 'Roboto', sans-serif;
    font-size: 14px;
    align-items: center;
}

/* 🛑 PHẦN SỬA ĐỔI: Chỉnh lại padding khi có prefix */
.ms-dropdown-wrapper :deep(.ant-select-selection-overflow) {
    /* Đảm bảo nội dung text không bị đè lên icon prefix */
    padding-left: 0 !important;
}

/* Ghi đè padding của placeholder và item khi có prefix để chúng hiển thị ngay sau icon */
.ms-dropdown-wrapper :deep(.ant-select-selection-placeholder),
.ms-dropdown-wrapper :deep(.ant-select-selection-item) {
    padding-left: 0 !important;
}

/* Ghi đè vị trí của prefix để nó luôn nằm ở đầu selector */
.ms-dropdown-wrapper :deep(.ant-select-prefix) {
    /* Đảm bảo icon prefix nằm ngoài padding-left mặc định */
    margin-right: 8px;
    /* Có thể dùng margin để tạo khoảng cách giữa icon và text */
}

/* 🛑 SỬA ĐỔI 2: Ghi đè mũi tên trỏ xuống bằng icon-small-arrow-down */
.ms-dropdown-wrapper :deep(.ant-select-arrow) {
    /* Ẩn icon mặc định (svg) */
    color: transparent !important;
    background: none !important;
    display: flex;
    align-items: center;
    justify-content: center;
    transition: transform 0.3s;
    /* Giữ transition cho hiệu ứng xoay */
}

/* Thêm element giả (::after) để chứa icon sprite */
.ms-dropdown-wrapper :deep(.ant-select-arrow::after) {
    content: "";
    /* Sử dụng background sprite cho icon-small-arrow-down */
    background: url('../assets/images/qlts-icon.png') no-repeat -72px -338px;
    width: 8px;
    height: 5px;
    display: inline-block;
}

/* Ghi đè icon khi dropdown mở */
.ms-dropdown-wrapper :deep(.ant-select-open .ant-select-arrow) {
    transform: none !important;
    /* Loại bỏ xoay mặc định của Ant Design */
}

.ms-dropdown-wrapper :deep(.ant-select-open .ant-select-arrow::after) {
    /* Xoay icon để tạo hiệu ứng mũi tên lên */
    transform: rotate(180deg);
}


/* Override viền Hover và Focus */
.ms-dropdown-wrapper :deep(.ant-select:not(.ant-select-disabled):not(.ant-select-customize-input) .ant-select-selector) {
    border-color: #d9d9d9;
    box-shadow: none !important;
}

.ms-dropdown-wrapper :deep(.ant-select:not(.ant-select-disabled):not(.ant-select-customize-input):hover .ant-select-selector) {
    border-color: #0097c2 !important;
}

.ms-dropdown-wrapper :deep(.ant-select-focused:not(.ant-select-disabled):not(.ant-select-customize-input) .ant-select-selector) {
    border-color: #0097c2 !important;
    box-shadow: 0 0 0 2px rgba(0, 151, 194, 0.2) !important;
}

/* Override style cho Option List/Dropdown Menu */
.ms-dropdown-wrapper :deep(.ant-select-dropdown) {
    font-family: 'Roboto', sans-serif;
    font-size: 14px;
    border-radius: 4px;
    box-shadow: 0 1px 6px rgba(0, 0, 0, 0.1);
}

/* Style cho option khi hover/selected trong list */
.ms-dropdown-wrapper :deep(.ant-select-item-option-selected:not(.ant-select-item-option-disabled)) {
    background-color: #e6f7fa !important;
    font-weight: 500;
}

.ms-dropdown-wrapper :deep(.ant-select-item-option-active:not(.ant-select-item-option-disabled)) {
    background-color: #f0f7f9 !important;
}</style>