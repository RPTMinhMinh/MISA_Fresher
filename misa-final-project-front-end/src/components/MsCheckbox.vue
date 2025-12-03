<template>
    <div class="ms-checkbox-wrapper">
        <a-checkbox :checked="modelValue" :disabled="disabled" @update:checked="handleUpdateChecked" v-bind="$attrs"
            class="ms-checkbox font-roboto text-[14px]">
            <slot>{{ label }}</slot>
        </a-checkbox>
    </div>
</template>
  
<script setup>
import { computed } from 'vue';

// Định nghĩa Props
const props = defineProps({
    // Giá trị v-model (true/false)
    modelValue: {
        type: Boolean,
        default: false,
    },
    // Label hiển thị bên cạnh checkbox (nếu không dùng slot)
    label: {
        type: String,
        default: '',
    },
    // Trạng thái disable
    disabled: {
        type: Boolean,
        default: false,
    },
});

// Định nghĩa Emits
const emit = defineEmits(['update:modelValue', 'change']);

// Xử lý cập nhật giá trị cho v-model
const handleUpdateChecked = (checked) => {
    emit('update:modelValue', checked);
    emit('change', checked);
};
</script>
  
<style scoped>
/* Màu Cyan/Teal chủ đạo: #20A5C8 */

/* 1. Ghi đè kích thước và viền mặc định của hộp checkbox */
.ms-checkbox-wrapper :deep(.ant-checkbox-inner) {
    width: 16px;
    height: 16px;
    border-radius: 2px !important;
    /* Bo góc nhẹ */
    border-color: #d9d9d9;
    /* Default border color (xám nhạt) */
    background-color: white;
}

/* 2. Trạng thái HOVER (Chưa checked) */
.ms-checkbox-wrapper :deep(.ant-checkbox:hover .ant-checkbox-inner) {
    border-color: #20A5C8 !important;
    /* Viền xanh Cyan/Teal khi Hover */
}

/* 3. Trạng thái ACTIVE (Checked) */
.ms-checkbox-wrapper :deep(.ant-checkbox-checked .ant-checkbox-inner) {
    background-color: #20A5C8 !important;
    /* Nền xanh Cyan/Teal */
    border-color: #20A5C8 !important;
    /* Viền xanh Cyan/Teal */
}

/* 4. Trạng thái HOVER (Đã checked) */
.ms-checkbox-wrapper :deep(.ant-checkbox-checked:hover .ant-checkbox-inner) {
    background-color: #20A5C8 !important;
    /* Nền đậm hơn khi Hover checked */
    border-color: #20A5C8 !important;
}

/* 5. Icon dấu tích (tick/check mark) */
.ms-checkbox-wrapper :deep(.ant-checkbox-checked .ant-checkbox-inner::after) {
    border-color: white;
    /* Dấu tích màu trắng */
}

/* 6. Trạng thái DISABLE (Chưa checked) */
.ms-checkbox-wrapper :deep(.ant-checkbox-disabled .ant-checkbox-inner) {
    background-color: #f5f5f5 !important;
    /* Nền xám nhạt */
    border-color: #d9d9d9 !important;
    /* Viền xám */
}

/* 7. Trạng thái DISABLE (Đã checked) - Icon mờ hơn */
.ms-checkbox-wrapper :deep(.ant-checkbox-disabled.ant-checkbox-checked .ant-checkbox-inner) {
    background-color: #aee3ed !important;
    /* Nền xanh nhạt (giống Disable trong MsButton Sub) */
    border-color: #aee3ed !important;
}

.ms-checkbox-wrapper :deep(.ant-checkbox-disabled.ant-checkbox-checked .ant-checkbox-inner::after) {
    border-color: white;
    /* Vẫn giữ màu trắng nhưng Ant Design tự làm mờ opacity */
}</style>