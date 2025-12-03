<template>
    <div class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4" @click.self="closePopup">
        <div class="bg-white rounded-lg shadow-xl w-full max-w-[900px]">
            <!-- Header - Hiển thị tiêu đề theo mode -->
            <div class="px-6 py-4 border-b border-gray-200">
                <h2 class="text-lg font-semibold text-gray-800">{{ mode === 'add' ? 'Thêm tài sản' : 'Sửa tài sản' }}</h2>
            </div>

            <!-- Body -->
            <div class="p-6">
                <div class="grid grid-cols-2 gap-x-5 gap-y-4">
                    <!-- Cột trái -->
                    <div class="space-y-4">
                        <MsInput v-model="form.assetCode" label="Mã tài sản" placeholder="TS00001" />
                        <MsDropdown 
                            v-model="form.departmentCode" 
                            :options="departmentOptions" 
                            label="Mã bộ phận sử dụng *" 
                            placeholder="Chọn mã bộ phận sử dụng"
                        />
                        <MsDropdown 
                            v-model="form.assetTypeCode" 
                            :options="assetTypeOptions" 
                            label="Mã loại tài sản *" 
                            placeholder="Chọn mã loại tài sản"
                        />
                        <MsInput v-model="form.quantity" label="Số lượng *" placeholder="01" type="number" />
                        <MsInput v-model="form.purchaseDate" label="Ngày mua *" placeholder="25/08/2021" />
                        <MsInput v-model="form.usefulLife" label="Số năm sử dụng *" placeholder="10" type="number" />
                    </div>

                    <!-- Cột phải -->
                    <div class="space-y-4">
                        <MsInput v-model="form.assetName" label="Tên tài sản *" placeholder="Nhập tên tài sản" />
                        <MsInput 
                            v-model="form.departmentName" 
                            label="Tên bộ phận sử dụng" 
                            placeholder="Phòng Hành chính Tổng hợp" 
                            :disabled="true" 
                        />
                        <MsInput 
                            v-model="form.assetTypeName" 
                            label="Tên loại tài sản" 
                            placeholder="Máy tính xách tay" 
                            :disabled="true" 
                        />
                        <MsInput v-model="form.originalPrice" label="Nguyên giá *" placeholder="10.000.000" />
                        <MsInput v-model="form.startDate" label="Ngày bắt đầu sử dụng *" placeholder="25/08/2021" />
                        <MsInput v-model="form.annualDepreciation" label="Giá trị hao mòn năm *" placeholder="1.000.000" />
                        <MsInput v-model="form.depreciationRate" label="Tỷ lệ hao mòn (%) *" placeholder="6,67" />
                        <MsInput v-model="form.trackingYear" label="Năm theo dõi" placeholder="2021" />
                    </div>
                </div>
            </div>

            <!-- Footer -->
            <div class="px-6 py-4 border-t border-gray-200 flex justify-end gap-3">
                <MsButton variant="outline" @click="closePopup" class="px-6">Hủy</MsButton>
                <MsButton variant="main" @click="save" class="px-6">Lưu</MsButton>
            </div>
        </div>
    </div>
</template>

<script setup>
import { reactive, watch, onMounted } from 'vue';
import MsInput from '@/components/MsInput.vue';
import MsButton from '@/components/MsButton.vue';
import MsDropdown from '@/components/MsDropdown.vue';

const props = defineProps({
    mode: {
        type: String,
        default: 'add'
    },
    assetData: {
        type: Object,
        default: null
    }
});

const emit = defineEmits(['close', 'save']);

// Options cho dropdown (mẫu)
const departmentOptions = [
    { value: 'HCTH', label: 'HCTH', name: 'Phòng Hành chính Tổng hợp' },
    { value: 'IT', label: 'IT', name: 'Phòng Công nghệ thông tin' },
    { value: 'HR', label: 'HR', name: 'Phòng Nhân sự' }
];

const assetTypeOptions = [
    { value: 'MTXT', label: 'MTXT', name: 'Máy tính xách tay' },
    { value: 'PC', label: 'PC', name: 'Máy tính để bàn' },
    { value: 'PR', label: 'PR', name: 'Máy in' }
];

// Helper function để format currency
const formatCurrency = (value) => {
    if (!value && value !== 0) return '0';
    return new Intl.NumberFormat('vi-VN').format(value);
};

// Reset form về giá trị mặc định
const resetForm = () => {
    Object.assign(form, {
        assetCode: '',
        assetName: '',
        departmentCode: '',
        departmentName: 'Phòng Hành chính Tổng hợp',
        assetTypeCode: '',
        assetTypeName: 'Máy tính xách tay',
        quantity: '',
        originalPrice: '',
        purchaseDate: '25/08/2021',
        startDate: '25/08/2021',
        usefulLife: '10',
        annualDepreciation: '',
        depreciationRate: '6,67',
        trackingYear: '2021',
    });
};

// Khởi tạo form với giá trị mặc định
const form = reactive({
    assetCode: '',
    assetName: '',
    departmentCode: '',
    departmentName: 'Phòng Hành chính Tổng hợp',
    assetTypeCode: '',
    assetTypeName: 'Máy tính xách tay',
    quantity: '',
    originalPrice: '',
    purchaseDate: '25/08/2021',
    startDate: '25/08/2021',
    usefulLife: '10',
    annualDepreciation: '',
    depreciationRate: '6,67',
    trackingYear: '2021',
});

// Watch để cập nhật tên khi chọn mã
watch(() => form.departmentCode, (newVal) => {
    const selected = departmentOptions.find(opt => opt.value === newVal);
    if (selected) {
        form.departmentName = selected.name;
    } else {
        form.departmentName = 'Phòng Hành chính Tổng hợp';
    }
});

watch(() => form.assetTypeCode, (newVal) => {
    const selected = assetTypeOptions.find(opt => opt.value === newVal);
    if (selected) {
        form.assetTypeName = selected.name;
    } else {
        form.assetTypeName = 'Máy tính xách tay';
    }
});

const closePopup = () => {
    emit('close');
};

const save = () => {
    // Validate các trường bắt buộc
    const requiredFields = [
        'assetCode', 'assetName', 'departmentCode', 'assetTypeCode',
        'quantity', 'originalPrice', 'purchaseDate', 'startDate',
        'usefulLife', 'annualDepreciation', 'depreciationRate'
    ];

    const missingFields = requiredFields.filter(field => !form[field]);

    if (missingFields.length > 0) {
        alert('Vui lòng điền đầy đủ các trường bắt buộc');
        return;
    }

    emit('save', { ...form });
    closePopup();
};

// Khi component được mount
onMounted(() => {
    console.log('AssetPopup mounted with mode:', props.mode);
    console.log('AssetPopup mounted with assetData:', props.assetData);
});

// Watch để cập nhật form khi assetData thay đổi
watch(() => props.assetData, (newData) => {
    console.log('AssetPopup received new assetData:', newData);
    
    if (newData) {
        // Điền dữ liệu từ record vào form (chế độ edit)
        form.assetCode = newData.assetCode || '';
        form.assetName = newData.assetName || '';
        form.departmentCode = newData.department || '';
        form.departmentName = newData.department || 'Phòng Hành chính Tổng hợp';
        form.assetTypeCode = newData.assetType || '';
        form.assetTypeName = newData.assetType || 'Máy tính xách tay';
        form.quantity = newData.quantity?.toString() || '';
        form.originalPrice = formatCurrency(newData.originalPrice) || '';
        form.annualDepreciation = formatCurrency(newData.depreciation) || '';
    } else {
        // Reset form về giá trị mặc định (chế độ add)
        resetForm();
    }
}, { immediate: true });

// Watch để theo dõi mode thay đổi
watch(() => props.mode, (newMode) => {
    console.log('AssetPopup mode changed to:', newMode);
    if (newMode === 'add') {
        resetForm();
    }
});
</script>

<style scoped>
/* Custom spacing theo đúng ảnh - 16px giữa các trường */
.space-y-4>*+* {
    margin-top: 16px;
}

/* Đảm bảo chiều cao input nhất quán 36px */
:deep(.ms-input .ant-input),
:deep(.ms-dropdown .ant-select-selector) {
    height: 36px !important;
    display: flex;
    align-items: center;
}

/* Custom cho các trường disabled */
:deep(.ant-input-disabled) {
    background-color: #f5f5f5 !important;
    color: #000000 !important;
    border-color: #d9d9d9 !important;
}

/* Đảm bảo dropdown cũng có chiều cao 36px */
:deep(.ms-dropdown .ant-select-selection-item),
:deep(.ms-dropdown .ant-select-selection-placeholder) {
    line-height: 34px !important;
}

/* Đảm bảo khoảng cách giữa hai cột là 20px */
.gap-x-5 {
    gap: 20px;
}

/* Đảm bảo khoảng cách giữa các hàng là 16px */
.gap-y-4 {
    gap: 16px;
}
</style>