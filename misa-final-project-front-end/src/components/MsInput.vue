<template>
    <div class="flex flex-col w-full gap-1">
        <label v-if="label" class="text-[13px] font-medium text-gray-700 font-roboto flex items-center gap-1"
            :class="{ 'opacity-50': disabled }">
            {{ label }}
            <span v-if="required" class="text-[4px] text-[#ff4d4f] leading-none mt-[-2px]">*</span>
        </label>

        <a-input ref="inputRef" :value="modelValue" :disabled="disabled" :status="error ? 'error' : ''" v-bind="$attrs"
            @input="handleInput" class="ms-input h-[40px] rounded-[4px] text-[14px] font-roboto" :class="inputClasses">
            <template v-for="(_, slot) in $slots" v-slot:[slot]="scope">
                <slot :name="slot" v-bind="scope || {}" />
            </template>
        </a-input>

        <span v-if="error && errorMessage" class="text-[12px] text-[#ff4d4f] font-roboto mt-0.5">
            {{ errorMessage }}
        </span>
    </div>
</template>
  
<script setup>
import { computed } from 'vue';

// Định nghĩa Props
const props = defineProps({
    // Giá trị v-model
    modelValue: {
        type: [String, Number],
        default: '',
    },
    // Label hiển thị bên trên
    label: {
        type: String,
        default: '',
    },
    // Trạng thái disable
    disabled: {
        type: Boolean,
        default: false,
    },
    // Trạng thái lỗi (hiện viền đỏ)
    error: {
        type: Boolean,
        default: false,
    },
    // Câu thông báo lỗi bên dưới
    errorMessage: {
        type: String,
        default: '',
    },
    // Trường bắt buộc (hiển thị dấu *)
    required: {
        type: Boolean,
        default: false,
    },
});

// Định nghĩa Emits
const emit = defineEmits(['update:modelValue', 'change']);

// Xử lý cập nhật value cho v-model
const handleInput = (e) => {
    const target = e.target;
    emit('update:modelValue', target.value);
    emit('change', e);
};

// Style classes động bằng Tailwind để override Ant Design
const inputClasses = computed(() => {
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
/* Deep selector để ghi đè style của Ant Design cho khớp design system.
     Màu #0097c2 lấy từ button component trước đó cho đồng bộ.
  */

/* Trạng thái Hover & Focus Text Color (nếu cần) */
:deep(.ant-input) {
    font-family: 'Roboto', sans-serif;
    /* Đảm bảo font đúng yêu cầu */
}

/* Placeholder color styling (Optional) */
:deep(.ant-input::placeholder) {
    color: #bfbfbf;
    font-style: italic;
    /* Hình ảnh design cho thấy placeholder hơi nghiêng */
}

/* Remove default Ant Design hover border color conflict */
:deep(.ant-input:hover) {
    border-color: #0097c2;
}

:deep(.ant-input-status-error:hover) {
    border-color: #ff4d4f !important;
}
</style>