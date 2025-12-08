<template>
    <div class="flex flex-col w-full gap-1">
        <label v-if="label" class="text-[13px] font-medium text-gray-700 font-roboto flex items-center gap-1"
            :class="{ 'opacity-50': disabled }">
            {{ label }}
            <span v-if="required" class="text-[4px] text-[#ff4d4f] leading-none mt-[-2px]">*</span>
        </label>

        <a-date-picker ref="datePickerRef" :value="modelValue" :disabled="disabled" :status="error ? 'error' : ''"
            v-bind="$attrs" @change="handleChange" class="ms-date-picker w-full" :class="datePickerClasses" :format="format"
            :value-format="valueFormat">
        </a-date-picker>

        <span v-if="error && errorMessage" class="text-[12px] text-[#ff4d4f] font-roboto mt-0.5">
            {{ errorMessage }}
        </span>
    </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue';
import { DatePicker } from 'ant-design-vue';
import dayjs from 'dayjs';

const ADatePicker = DatePicker;

// Định nghĩa Props
const props = defineProps({
    modelValue: {
        type: [Object, String, Number, Date],
        default: null,
    },
    label: {
        type: String,
        default: '',
    },
    disabled: {
        type: Boolean,
        default: false,
    },
    error: {
        type: Boolean,
        default: false,
    },
    errorMessage: {
        type: String,
        default: '',
    },
    required: {
        type: Boolean,
        default: false,
    },
    placeholder: {
        type: String,
        default: '',
    },
    format: {
        type: String,
        default: 'DD/MM/YYYY'
    },
    valueFormat: {
        type: String,
        default: 'DD/MM/YYYY' // THÊM: định dạng giá trị
    }
});

// Định nghĩa Emits
const emit = defineEmits(['update:modelValue', 'change']);

// Xử lý thay đổi giá trị
const handleChange = (date, dateString) => {
    emit('update:modelValue', date);
    emit('change', date, dateString);
};

// Style classes động bằng Tailwind để override Ant Design
const datePickerClasses = computed(() => {
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
/* Deep selector để ghi đè style của Ant Design */

:deep(.ant-picker) {
    height: 40px !important;
    border-radius: 4px !important;
    font-family: 'Roboto', sans-serif !important;
    width: 100% !important;
}

:deep(.ant-picker:hover) {
    border-color: #0097c2 !important;
}

:deep(.ant-picker-focused) {
    border-color: #0097c2 !important;
    box-shadow: 0 0 0 2px rgba(0, 151, 194, 0.2) !important;
}

:deep(.ant-picker-input > input::placeholder) {
    color: #bfbfbf !important;
    font-style: italic !important;
}

:deep(.ant-picker-status-error) {
    border-color: #ff4d4f !important;
}

:deep(.ant-picker-status-error:hover) {
    border-color: #ff4d4f !important;
}

:deep(.ant-picker-status-error:focus) {
    border-color: #ff4d4f !important;
    box-shadow: 0 0 0 2px rgba(255, 77, 79, 0.2) !important;
}

:deep(.ant-picker-disabled) {
    background-color: #f5f5f5 !important;
    color: #000000 !important;
    border-color: #d9d9d9 !important;
}
</style>