<template>
    <div class="flex flex-col w-full gap-1">
        <label v-if="label" class="text-[13px] font-medium text-gray-700 font-roboto flex items-center gap-1"
            :class="{ 'opacity-50': disabled }">
            {{ label }}
            <span v-if="required" class="text-[4px] text-[#ff4d4f] leading-none mt-[-2px]">*</span>
        </label>

        <a-input ref="inputRef" :value="displayValue" :disabled="disabled" :status="error ? 'error' : ''"
            @input="handleInput" @blur="handleBlur" @focus="handleFocus" @keydown="handleKeyDown"
            class="ms-input h-[40px] rounded-[4px] text-[14px] font-roboto text-right" :class="inputClasses">
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
import { computed, ref, watch } from 'vue';

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
    // Số chữ số thập phân (mặc định 2)
    decimalPlaces: {
        type: Number,
        default: 2,
        validator: (value) => value >= 0 && value <= 10
    },
    // Cho phép số âm
    allowNegative: {
        type: Boolean,
        default: false,
    },
    // Sử dụng dấu chấm phân cách hàng nghìn (mặc định true)
    useThousandSeparator: {
        type: Boolean,
        default: true,
    }
});

// Định nghĩa Emits
const emit = defineEmits(['update:modelValue', 'change']);

// Biến để lưu giá trị hiển thị
const displayValue = ref('');
const isFocused = ref(false);

// Format số sang định dạng Việt Nam (dấu phẩy cho thập phân, dấu chấm cho hàng nghìn)
const formatToVietnameseNumber = (value) => {
    try {
        if (value === '' || value === null || value === undefined) return '';

        let num = parseFloat(value);
        if (isNaN(num)) {
            return '';
        }

        // Format với số chữ số thập phân
        let formatted = props.decimalPlaces > 0 ? num.toFixed(props.decimalPlaces) : Math.round(num).toString();

        // Tách phần nguyên và phần thập phân
        let parts = formatted.split('.');
        let integerPart = parts[0];
        let decimalPart = props.decimalPlaces > 0 ? parts[1] : '';

        // Xử lý số âm
        let isNegative = integerPart.startsWith('-');
        if (isNegative) {
            integerPart = integerPart.substring(1);
        }

        // Thêm dấu chấm phân cách hàng nghìn (nếu được bật)
        if (props.useThousandSeparator) {
            integerPart = integerPart.replace(/\B(?=(\d{3})+(?!\d))/g, '.');
        }

        // Thêm dấu trở lại nếu là số âm
        if (isNegative) {
            integerPart = '-' + integerPart;
        }

        // Ghép lại với dấu phẩy cho phần thập phân
        if (decimalPart) {
            // Loại bỏ số 0 không cần thiết ở cuối
            decimalPart = decimalPart.replace(/0+$/, '');
            if (decimalPart) {
                return `${integerPart},${decimalPart}`;
            }
        }

        return integerPart;
    } catch (error) {
        console.error('Error formatting number:', error);
        return '';
    }
};

// Parse từ định dạng Việt Nam sang số thông thường
const parseFromVietnameseNumber = (value) => {
    if (!value || value.trim() === '') return '';

    try {
        // Loại bỏ khoảng trắng
        let cleanValue = value.trim();

        // Kiểm tra số âm
        let isNegative = false;
        if (props.allowNegative && cleanValue.startsWith('-')) {
            isNegative = true;
            cleanValue = cleanValue.substring(1);
        }

        // Loại bỏ tất cả dấu chấm phân cách hàng nghìn (nếu có)
        let withoutThousandSeparators = cleanValue.replace(/\./g, '');

        // Thay dấu phẩy thập phân bằng dấu chấm
        let withDotDecimal = withoutThousandSeparators.replace(/,/g, '.');

        // Chuyển thành số
        let number = parseFloat(withDotDecimal);

        if (isNaN(number)) return '';

        // Áp dụng dấu âm nếu có
        if (isNegative) {
            number = -Math.abs(number);
        }

        return number;
    } catch (error) {
        console.error('Error parsing number:', error);
        return '';
    }
};

// Cập nhật giá trị hiển thị
const updateDisplayValue = (value) => {
    if (value === '' || value === null || value === undefined) {
        displayValue.value = '';
    } else {
        // Format số theo định dạng Việt Nam
        displayValue.value = formatToVietnameseNumber(value);
    }
};

// Theo dõi modelValue để cập nhật displayValue
watch(() => props.modelValue, (newVal) => {
    if (!isFocused.value) {
        updateDisplayValue(newVal);
    }
}, { immediate: true });

// Xử lý keydown để ngăn chặn hoàn toàn việc nhập chữ
const handleKeyDown = (e) => {
    // Danh sách các phím được phép
    const allowedKeys = [
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        'Backspace', 'Tab', 'ArrowLeft', 'ArrowRight', 'Delete',
        'Home', 'End', 'Enter'
    ];

    // Nếu là phím điều khiển (Ctrl, Cmd, Alt, Shift) thì cho phép
    if (e.ctrlKey || e.metaKey || e.altKey) {
        return;
    }

    // Nếu là phím số hoặc các phím điều hướng thì cho phép
    if (allowedKeys.includes(e.key)) {
        return;
    }

    // Nếu cho phép số âm và nhập dấu trừ ở đầu
    if (props.allowNegative && e.key === '-' &&
        (displayValue.value === '' || displayValue.value.startsWith('-'))) {
        return;
    }

    // Nếu cho phập thập phân và nhập dấu phẩy hoặc dấu chấm
    if (props.decimalPlaces > 0 && (e.key === ',' || e.key === '.') &&
        !displayValue.value.includes(',') && !displayValue.value.includes('.')) {
        return;
    }

    // Ngăn chặn tất cả các phím khác
    e.preventDefault();
};

// Xử lý input
const handleInput = (e) => {
    const value = e.target.value;

    // Kiểm tra nếu giá trị rỗng
    if (value === '') {
        displayValue.value = '';
        emit('update:modelValue', '');
        emit('change', { target: { value: '' } });
        return;
    }

    // Lọc ký tự chỉ giữ lại số, dấu âm (nếu được phép), và dấu thập phân (nếu được phép)
    let filteredValue = '';
    let hasDecimal = false;
    let hasNegative = false;

    for (let char of value) {
        if (props.allowNegative && char === '-' && !hasNegative && filteredValue === '') {
            filteredValue += char;
            hasNegative = true;
        } else if (char >= '0' && char <= '9') {
            filteredValue += char;
        } else if ((char === ',' || char === '.') && props.decimalPlaces > 0 && !hasDecimal) {
            // Chỉ cho phép một dấu thập phân và luôn dùng dấu phẩy
            filteredValue += ',';
            hasDecimal = true;
        }
    }

    // Giới hạn số chữ số thập phân
    if (hasDecimal && props.decimalPlaces > 0) {
        const parts = filteredValue.split(',');
        if (parts.length > 1 && parts[1].length > props.decimalPlaces) {
            filteredValue = parts[0] + ',' + parts[1].substring(0, props.decimalPlaces);
        }
    }

    // Nếu decimalPlaces = 0, loại bỏ dấu thập phân
    if (props.decimalPlaces === 0) {
        filteredValue = filteredValue.replace(/[,.]/g, '');
    }

    displayValue.value = filteredValue;

    // Parse và emit giá trị
    const parsedValue = parseFromVietnameseNumber(filteredValue);
    emit('update:modelValue', parsedValue);
    emit('change', { target: { value: parsedValue } });
};

// Xử lý blur
const handleBlur = () => {
    isFocused.value = false;

    if (displayValue.value) {
        // Parse giá trị hiện tại
        const parsedValue = parseFromVietnameseNumber(displayValue.value);

        if (parsedValue !== '') {
            // Format lại số khi blur
            displayValue.value = formatToVietnameseNumber(parsedValue);
            // Emit giá trị cuối cùng
            emit('update:modelValue', parsedValue);
        } else {
            displayValue.value = '';
            emit('update:modelValue', '');
        }
    }
};

// Xử lý focus
const handleFocus = () => {
    isFocused.value = true;

    if (displayValue.value) {
        // Khi focus, hiển thị số không định dạng để dễ chỉnh sửa
        const parsedValue = parseFromVietnameseNumber(displayValue.value);
        if (parsedValue !== '') {
            // Hiển thị với dấu chấm cho phần thập phân
            let valueStr = parsedValue.toString();
            // Thay dấu phẩy bằng dấu chấm nếu có
            valueStr = valueStr.replace(',', '.');
            displayValue.value = valueStr;
        }
    }
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

/* Đảm bảo căn phải cho input số */
.ms-input.text-right :deep(.ant-input) {
    text-align: right;
}

/* Ẩn mũi tên tăng giảm cho input number trên Chrome, Safari */
:deep(input[type="number"]::-webkit-inner-spin-button),
:deep(input[type="number"]::-webkit-outer-spin-button) {
    -webkit-appearance: none;
    margin: 0;
}

/* Ẩn mũi tên tăng giảm cho input number trên Firefox */
:deep(input[type="number"]) {
    -moz-appearance: textfield;
}
</style>