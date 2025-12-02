<template>
    <a-button :class="buttonClasses" :disabled="disabled" v-bind="$attrs"
        class="ms-btn transition-all duration-300 flex items-center justify-center font-medium shadow-none h-[40px] px-6 rounded-[4px]">
        <slot>Button</slot>
    </a-button>
</template>
  
<script setup>
import { computed } from 'vue';

// Định nghĩa Props
const props = defineProps({
    // Loại button: 'main', 'sub', 'outline'
    variant: {
        type: String,
        default: 'main',
        validator: (value) => ['main', 'sub', 'outline'].includes(value),
    },
    // Trạng thái disable
    disabled: {
        type: Boolean,
        default: false,
    },
});

// Xử lý logic style bằng Tailwind dựa trên props 'variant'
const buttonClasses = computed(() => {
    const baseClasses = '';

    // 1. MAIN BUTTON (Nền xanh, chữ trắng)
    if (props.variant === 'main') {
        return `
        bg-[#0097c2] text-white border-transparent
        hover:!bg-[#007b9e] hover:!text-white hover:border-transparent
        active:!bg-[#006582] active:!text-white
        focus:ring-2 focus:ring-[#0097c2]/50
        disabled:!bg-[#8fd5e2] disabled:!text-white disabled:!border-transparent disabled:opacity-80
      `;
    }

    // 2. SUB BUTTON (Nền trắng, viền xanh, chữ xanh)
    if (props.variant === 'sub') {
        return `
        bg-white border border-[#0097c2] text-[#0097c2]
        hover:!bg-[#e6f7fa] hover:!text-[#007b9e] hover:!border-[#007b9e]
        active:!bg-[#cceef5] active:!text-[#006582]
        focus:ring-2 focus:ring-[#0097c2]/30
        disabled:!bg-white disabled:!text-[#aee3ed] disabled:!border-[#aee3ed]
      `;
    }

    // 3. OUTLINE BUTTON (Nền trắng, viền xám, chữ xám - Hover thành xanh)
    if (props.variant === 'outline') {
        return `
        bg-white border border-gray-300 text-gray-600
        hover:!bg-[#0097c2] hover:!text-white hover:!border-[#0097c2]
        active:!bg-[#007b9e] active:!text-white active:!border-[#007b9e]
        focus:ring-2 focus:ring-gray-300/50
        disabled:!bg-[#f5f5f5] disabled:!text-[#bfbfbf] disabled:!border-[#d9d9d9]
      `;
    }

    return baseClasses;
});
</script>
  
<style scoped>
/* Override một số style mặc định cứng đầu của Ant Design nếu cần */
.ms-btn {
    /* Đảm bảo chiều cao và căn chỉnh text */
    display: inline-flex;
    align-items: center;
    justify-content: center;
}

/* Ant Design thường có style mặc định cho hover khá mạnh, 
     ta dùng !important trong class Tailwind (dấu chấm than) để ghi đè,
     nhưng nếu vẫn bị conflict màu shadow, có thể reset ở đây */
:deep(.ant-btn-default:hover) {
    /* Reset màu mặc định của ant design */
    border-color: currentColor;
}
</style>