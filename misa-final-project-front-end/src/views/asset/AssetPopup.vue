<template>
    <div class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4" @click.self="closePopup">
        <div class="bg-white rounded-lg shadow-xl w-full max-w-[1000px] max-h-[94vh] overflow-hidden flex flex-col">
            <!-- Header - Hiển thị tiêu đề theo mode -->
            <div class="px-6 mt-4 flex-shrink-0">
                <h2 class="text-md font-semibold text-gray-800">{{ mode === 'add' ? 'Thêm tài sản' : 'Sửa tài sản' }}</h2>
            </div>

            <div class="p-6 overflow-y-auto flex-grow">
                <div class="grid grid-cols-3 gap-x-4 gap-y-3">
                    <div class="col-span-1">
                        <MsInput v-model="form.assetCode" label="Mã tài sản" placeholder="TS00001" required/>
                    </div>

                    <div class="col-span-2">
                        <MsInput v-model="form.assetName" label="Tên tài sản" required placeholder="Nhập tên tài sản" />
                    </div>

                    <div class="col-span-1">
                        <MsDropdown v-model="form.departmentCode" :options="departmentOptions" label="Mã bộ phận sử dụng" required
                            placeholder="Chọn mã bộ phận sử dụng" />
                    </div>

                    <div class="col-span-2">
                        <MsInput v-model="form.departmentName" label="Tên bộ phận sử dụng"
                            placeholder="Phòng Hành chính Tổng hợp" :disabled="true" />
                    </div>

                    <div class="col-span-1">
                        <MsDropdown v-model="form.assetTypeCode" :options="assetTypeOptions" label="Mã loại tài sản" required=""
                            placeholder="Chọn mã loại tài sản" />
                    </div>

                    <div class="col-span-2">
                        <MsInput v-model="form.assetTypeName" label="Tên loại tài sản" placeholder="Máy tính xách tay"
                            :disabled="true" />
                    </div>

                    <div class="col-span-1">
                        <MsInputNumber v-model="form.quantity" label="Số lượng" required placeholder="01" :decimal-places="0" />
                    </div>

                    <div class="col-span-1">
                        <MsInputNumber v-model="form.originalPrice" label="Nguyên giá" required :decimal-places="0" />
                    </div>

                    <div class="col-span-1">
                        <MsInputNumber v-model="form.depreciationRate" type="number"  label="Tỷ lệ hao mòn (%)" required :decimal-places="2" />
                    </div>

                    <div class="col-span-1">
                        <MsInput v-model="form.purchaseDate" label="Ngày mua" required placeholder="25/08/2021" />
                    </div>

                    <div class="col-span-1">
                        <MsInput v-model="form.startDate" label="Ngày bắt đầu sử dụng" required placeholder="25/08/2021" />
                    </div>

                    <div class="col-span-1">
                        <MsInputNumber v-model="form.trackingYear" label="Năm theo dõi" placeholder="2021" :use-thousand-separator="false"/>
                    </div>

                    <div class="col-span-1">
                        <MsInputNumber v-model="form.usefulLife" label="Số năm sử dụng"  required placeholder="10" :decimal-places="0" />
                    </div>

                    <div class="col-span-1">
                        <MsInputNumber v-model="form.annualDepreciation" :decimal-places="0" label="Giá trị hao mòn năm" required placeholder="1.000.000" />
                    </div>

                    <div class="col-span-1"></div>
                </div>
            </div>

            <div class="px-6 py-4 border-t border-gray-200 flex justify-end gap-3 flex-shrink-0">
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
import MsInputNumber from '@/components/MsInputNumber.vue'

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
        depreciationRate: '',
        trackingYear: '2021',
    });
};

// Khởi tạo form với giá trị mặc định
const form = reactive({
    assetCode: '',
    assetName: '',
    departmentCode: '',
    departmentName: '',
    assetTypeCode: '',
    assetTypeName: '',
    quantity: '',
    originalPrice: '',
    purchaseDate: '25/08/2021',
    startDate: '25/08/2021',
    usefulLife: '10',
    annualDepreciation: '',
    depreciationRate: '',
    trackingYear: '2021',
});

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

onMounted(() => {
    console.log('AssetPopup mounted with mode:', props.mode);
    console.log('AssetPopup mounted with assetData:', props.assetData);
});

watch(() => props.assetData, (newData) => {
    console.log('AssetPopup received new assetData:', newData);

    if (newData) {
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
        resetForm();
    }
}, { immediate: true });

watch(() => props.mode, (newMode) => {
    console.log('AssetPopup mode changed to:', newMode);
    if (newMode === 'add') {
        resetForm();
    }
});
</script>

<style scoped>
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

:deep(.ms-dropdown .ant-select-selection-item),
:deep(.ms-dropdown .ant-select-selection-placeholder) {
    line-height: 34px !important;
}
.gap-x-4 {
    gap: 16px;
}

.gap-y-3 {
    gap: 12px;
}

.col-span-1,
.col-span-2 {
    display: flex;
    flex-direction: column;
    min-height: 80px;
    /* Đảm bảo có đủ không gian cho label và input */
}

/* Responsive cho màn hình nhỏ */
@media (max-width: 768px) {
    .grid-cols-3 {
        grid-template-columns: 1fr;
    }

    .col-span-1,
    .col-span-2 {
        grid-column: span 1 !important;
    }
}
</style>