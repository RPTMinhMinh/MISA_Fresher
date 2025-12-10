<template>
    <div class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4"
        @click.self="handleCancelClick">
        <div class="bg-white rounded-lg shadow-xl w-full max-w-[1000px] max-h-[94vh] overflow-hidden flex flex-col">
            <!-- Header - Hiển thị tiêu đề theo mode -->
            <div class="px-6 mt-4 flex-shrink-0">
                <h2 class="text-md font-semibold text-gray-800">{{ mode === 'add' ? 'Thêm tài sản' : 'Sửa tài sản' }}</h2>
            </div>

            <div class="p-6 overflow-y-auto flex-grow">
                <div class="grid grid-cols-3 gap-x-4 gap-y-3">
                    <!-- Mã tài sản - Cho phép nhập khi thêm mới -->
                    <div class="col-span-1">
                        <MsInput v-model="form.assetCode" label="Mã tài sản"
                            :placeholder="loadingNextCode ? 'Đang tải mã...' : 'TS00001'" 
                            :disabled="mode === 'edit'" 
                            :required="true"
                            :error="fieldErrors.assetCode" 
                            :error-message="fieldErrors.assetCode" 
                            @blur="validateAssetCode"/>
                    </div>

                    <div class="col-span-2">
                        <MsInput v-model="form.assetName" label="Tên tài sản" required placeholder="Nhập tên tài sản"
                            :error="fieldErrors.assetName" :error-message="fieldErrors.assetName" />
                    </div>

                    <!-- Sử dụng departmentOptionsForForm (hiển thị mã) -->
                    <div class="col-span-1">
                        <MsDropdown v-model="form.departmentCode" :options="departmentOptionsForForm"
                            label="Mã bộ phận sử dụng" required placeholder="Chọn mã bộ phận"
                            :error="fieldErrors.departmentCode" :error-message="fieldErrors.departmentCode"
                            @change="handleDepartmentChange" display-mode="table" />
                    </div>

                    <div class="col-span-2">
                        <MsInput v-model="form.departmentName" label="Tên bộ phận sử dụng"
                            placeholder="" :disabled="true" />
                    </div>

                    <!-- Sử dụng assetTypeOptionsForForm (hiển thị mã) -->
                    <div class="col-span-1">
                        <MsDropdown v-model="form.assetTypeCode" :options="assetTypeOptionsForForm" label="Mã loại tài sản"
                            required placeholder="Chọn mã loại tài sản" :error="fieldErrors.assetTypeCode"
                            :error-message="fieldErrors.assetTypeCode" @change="handleAssetTypeChange"
                            display-mode="table" />
                    </div>

                    <div class="col-span-2">
                        <MsInput v-model="form.assetTypeName" label="Tên loại tài sản" placeholder=""
                            :disabled="true" />
                    </div>

                    <div class="col-span-1">
                        <MsInputNumber v-model="form.quantity" label="Số lượng" required placeholder="01"
                            :decimal-places="0" :error="fieldErrors.quantity" :error-message="fieldErrors.quantity" />
                    </div>

                    <div class="col-span-1">
                        <MsInputNumber v-model="form.originalPrice" label="Nguyên giá" required :decimal-places="0"
                            :error="fieldErrors.originalPrice" :error-message="fieldErrors.originalPrice"
                            @change="calculateAnnualDecreciation" />
                    </div>

                    <div class="col-span-1">
                        <MsInputNumber v-model="form.decreciationRate" label="Tỷ lệ hao mòn (%)" required
                            :decimal-places="2" @change="handleDecreciationRateChange" 
                            :error="fieldErrors.decreciationRate" :error-message="fieldErrors.decreciationRate" />
                    </div>

                    <div class="col-span-1">
                        <MsDatePicker v-model="purchaseDateValue" :label="'Ngày mua'" :required="true" format="DD/MM/YYYY"
                            value-format="DD/MM/YYYY" placeholder="Chọn ngày mua" class="w-full"
                            :disabled-date="disabledDate" @change="handlePurchaseDateChange"
                            :error="fieldErrors.purchaseDate" :error-message="fieldErrors.purchaseDate" />
                    </div>

                    <div class="col-span-1">
                        <MsDatePicker v-model="startDateValue" :label="'Ngày bắt đầu sử dụng'" :required="true"
                            format="DD/MM/YYYY" value-format="DD/MM/YYYY" placeholder="Chọn ngày bắt đầu sử dụng"
                            class="w-full" :disabled-date="disabledDate" @change="handleStartDateChange"
                            :error="fieldErrors.startDate" :error-message="fieldErrors.startDate" />
                    </div>

                    <div class="col-span-1">
                        <MsInputNumber v-model="form.trackingYear" label="Năm theo dõi" placeholder="2021"
                            :use-thousand-separator="false" :disabled="true" />
                    </div>

                    <div class="col-span-1">
                        <MsInputNumber v-model="form.usefulLife" label="Số năm sử dụng" required placeholder="10"
                            :decimal-places="0" @change="handleUsefulLifeChange"
                            :error="fieldErrors.usefulLife" :error-message="fieldErrors.usefulLife" />
                    </div>

                    <div class="col-span-1">
                        <MsInputNumber v-model="form.annualDecreciation" :decimal-places="0" label="Giá trị hao mòn năm"
                            required placeholder="1.000.000" :disabled="true" />
                    </div>

                    <div class="col-span-1"></div>
                </div>
            </div>

            <div class="px-6 py-4 border-t border-gray-200 flex justify-end gap-3 flex-shrink-0">
                <MsButton variant="outline" @click="handleCancelClick" class="px-6">Hủy</MsButton>
                <MsButton variant="main" @click="save" class="px-6" :loading="saving">Lưu</MsButton>
            </div>
        </div>
    </div>

    <!-- Popup xác nhận hủy form (giống Asset.vue) -->
</template>

<script setup>
import { ref, reactive, watch, onMounted, nextTick } from 'vue';
import dayjs from 'dayjs';
import MsInput from '@/components/MsInput.vue';
import MsButton from '@/components/MsButton.vue';
import MsDropdown from '@/components/MsDropdown.vue';
import MsInputNumber from '@/components/MsInputNumber.vue';
import MsDatePicker from '@/components/MsDatePicker.vue';
import MsConfirmDialog from '@/components/MsConfirmDialog.vue';
import { useDropdownData } from '@/composables/useDropdownData';
import { assetService } from '@/services';
import { useToast } from '@/composables/useToast';

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

const { showSuccess, showError } = useToast();
const {
    assetTypeOptionsForForm,
    departmentOptionsForForm,
    isLoading: dropdownLoading,
    loadData
} = useDropdownData();

// State
const loadingNextCode = ref(false);
const saving = ref(false);
const fieldErrors = reactive({});
const purchaseDateValue = ref(dayjs());
const startDateValue = ref(dayjs());
const existingAssetCodes = ref(new Set());
const userModifiedDecreciationRate = ref(false); 
const userModifiedUsefulLife = ref(false); 

// Format date thành dd/MM/yyyy
const formatDate = (date) => {
    if (!date) return ''
    const d = dayjs(date)
    if (!d.isValid()) return ''
    return d.format('DD/MM/YYYY')
}

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
    purchaseDate: formatDate(new Date()),
    startDate: formatDate(new Date()),
    usefulLife: '',
    annualDecreciation: '',
    decreciationRate: '',
    trackingYear: new Date().getFullYear().toString(),
    createdDate: new Date().toISOString()
});

// Tải danh sách mã tài sản hiện có
const loadExistingAssetCodes = async () => {
    try {
        const response = await assetService.getAssetsPaged({ PageSize: 1000 });
        if (response && response.success && response.data) {
            const assetCodes = response.data.data.map(item => item.assetCode);
            existingAssetCodes.value = new Set(assetCodes);
        }
    } catch (error) {
        console.error('Error loading existing asset codes:', error);
    }
};

// Validate mã tài sản không được trùng
const validateAssetCode = async () => {
    if (!form.assetCode || props.mode === 'edit') return true;
    
    // Kiểm tra mã tài sản có tồn tại không
    if (existingAssetCodes.value.has(form.assetCode)) {
        fieldErrors.assetCode = 'Mã tài sản đã tồn tại trong hệ thống';
        return false;
    }
    
    // Kiểm tra mã tài sản format
    const assetCodePattern = /^[A-Za-z0-9]+$/;
    if (!assetCodePattern.test(form.assetCode)) {
        fieldErrors.assetCode = 'Mã tài sản chỉ được chứa chữ cái và số';
        return false;
    }
    
    fieldErrors.assetCode = '';
    return true;
};

// Load next code khi thêm mới
const loadNextCode = async () => {
    if (props.mode === 'add' && !form.assetCode) {
        loadingNextCode.value = true
        try {
            const response = await assetService.getNextAssetCode()
            if (response && response.success && response.data) {
                form.assetCode = response.data
            } else {
                form.assetCode = ''
                showError('Không thể lấy mã tài sản tiếp theo')
            }
        } catch (error) {
            console.error('Error loading next code:', error)
            form.assetCode = ''
            showError('Lỗi khi lấy mã tài sản')
        } finally {
            loadingNextCode.value = false
        }
    }
}

// Xử lý khi chọn department
const handleDepartmentChange = (value) => {
    const selected = departmentOptionsForForm.value?.find(opt => opt.value === value);
    if (selected && selected.data) {
        form.departmentName = selected.data.departmentName || '';
    } else {
        form.departmentName = '';
    }
};

// Xử lý khi chọn asset type
const handleAssetTypeChange = (value) => {
    const selected = assetTypeOptionsForForm.value?.find(opt => opt.value === value);
    if (selected && selected.data) {
        form.assetTypeName = selected.data.assetTypeName || '';
        
        // QUAN TRỌNG: Chỉ apply giá trị mặc định trong các trường hợp sau:
        // 1. Mode ADD và người dùng CHƯA chỉnh sửa
        // 2. Mode EDIT nhưng giá trị hiện tại là rỗng/chưa có
        if (props.mode === 'add') {
            // Mode ADD: chỉ điền nếu người dùng chưa chỉnh sửa
            if (!userModifiedDecreciationRate.value && selected.data.decreciationRate) {
                form.decreciationRate = selected.data.decreciationRate?.toString() || '';
                calculateAnnualDecreciation();
            }
            if (!userModifiedUsefulLife.value && selected.data.usefulLife) {
                form.usefulLife = selected.data.usefulLife?.toString() || '';
            }
        } else {
            // Mode EDIT: chỉ điền nếu form hiện tại đang trống
            // KHÔNG ghi đè dữ liệu từ database
            if ((!form.decreciationRate || form.decreciationRate === '') && selected.data.decreciationRate) {
                form.decreciationRate = selected.data.decreciationRate?.toString() || '';
                calculateAnnualDecreciation();
            }
            if ((!form.usefulLife || form.usefulLife === '') && selected.data.usefulLife) {
                form.usefulLife = selected.data.usefulLife?.toString() || '';
            }
        }
        
        // Tính toán tỉ lệ hao mòn nếu cần
        if (form.usefulLife && !form.decreciationRate) {
            calculateDecreciationRateFromUsefulLife();
        }
    } else {
        form.assetTypeName = '';
    }
};

// Xử lý khi người dùng thay đổi tỷ lệ hao mòn
const handleDecreciationRateChange = () => {
    userModifiedDecreciationRate.value = true;
    calculateAnnualDecreciation();
};

// Xử lý khi người dùng thay đổi số năm sử dụng
const handleUsefulLifeChange = () => {
    userModifiedUsefulLife.value = true;
    calculateDecreciationRateFromUsefulLife();
};

// Tính tỷ lệ hao mòn từ số năm sử dụng
const calculateDecreciationRateFromUsefulLife = () => {
    if (form.usefulLife && parseFloat(form.usefulLife) > 0) {
        const decreciationRate = (100 / parseFloat(form.usefulLife)).toFixed(2);
        form.decreciationRate = decreciationRate;
        calculateAnnualDecreciation();
    }
};

// Tính toán giá trị hao mòn năm
const calculateAnnualDecreciation = () => {
    if (form.originalPrice && form.decreciationRate) {
        const originalPrice = parseFloat(form.originalPrice.toString().replace(/\./g, '').replace(',', '.'))
        const decreciationRate = parseFloat(form.decreciationRate.toString().replace(',', '.'))

        if (!isNaN(originalPrice) && !isNaN(decreciationRate)) {
            const annualDecreciation = (originalPrice * decreciationRate) / 100
            form.annualDecreciation = Math.round(annualDecreciation).toString()
        } else {
            form.annualDecreciation = ''
        }
    } else {
        form.annualDecreciation = ''
    }
}

// Tính tỷ lệ hao mòn khi số năm sử dụng thay đổi
const calculateDecreciationRate = () => {
    if (form.usefulLife && parseFloat(form.usefulLife) > 0) {
        const decreciationRate = (100 / parseFloat(form.usefulLife)).toFixed(2);
        form.decreciationRate = decreciationRate;
        calculateAnnualDecreciation();
    }
};

// Hàm tìm và điền tên từ mã khi có dữ liệu
const fillDepartmentNameFromCode = () => {
    if (form.departmentCode && departmentOptionsForForm.value?.length > 0) {
        const selected = departmentOptionsForForm.value.find(opt => opt.value === form.departmentCode);
        if (selected && selected.data) {
            form.departmentName = selected.data.departmentName || '';
        }
    }
};

// Hàm tìm và điền tên loại tài sản từ mã khi có dữ liệu
const fillAssetTypeInfoFromCode = () => {
    // QUAN TRỌNG: Nếu mode là edit và form đã có giá trị từ database, KHÔNG điền
    if (props.mode === 'edit' && form.decreciationRate && form.decreciationRate !== '') {
        // Nếu đã có giá trị từ database, chỉ điền tên, không điền số
        if (form.assetTypeCode && assetTypeOptionsForForm.value?.length > 0) {
            const selected = assetTypeOptionsForForm.value.find(opt => opt.value === form.assetTypeCode);
            if (selected && selected.data) {
                form.assetTypeName = selected.data.assetTypeName || '';
            }
        }
        return; // Thoát sớm
    }
    
    // Phần code cũ cho mode add
    if (form.assetTypeCode && assetTypeOptionsForForm.value?.length > 0) {
        const selected = assetTypeOptionsForForm.value.find(opt => opt.value === form.assetTypeCode);
        if (selected && selected.data) {
            form.assetTypeName = selected.data.assetTypeName || '';
            
            // Mode ADD: giữ nguyên logic cũ
            if (!userModifiedUsefulLife.value && selected.data.usefulLife) {
                form.usefulLife = selected.data.usefulLife?.toString() || '';
            }
            
            if (!userModifiedDecreciationRate.value && selected.data.decreciationRate) {
                form.decreciationRate = selected.data.decreciationRate?.toString() || '';
                calculateAnnualDecreciation();
            }
            
            if (form.usefulLife && !form.decreciationRate) {
                calculateDecreciationRateFromUsefulLife();
            }
        }
    }
};

// Reset form về giá trị mặc định
const resetForm = () => {
    Object.assign(form, {
        assetCode: '',
        assetName: '',
        departmentCode: '',
        departmentName: '',
        assetTypeCode: '',
        assetTypeName: '',
        quantity: '',
        originalPrice: '',
        purchaseDate: formatDate(new Date()),
        startDate: formatDate(new Date()),
        usefulLife: '',
        annualDecreciation: '',
        decreciationRate: '',
        trackingYear: new Date().getFullYear().toString(),
        createdDate: new Date().toISOString()
    });

    purchaseDateValue.value = dayjs()
    startDateValue.value = dayjs()

    userModifiedDecreciationRate.value = false;
    userModifiedUsefulLife.value = false;

    // Reset errors
    Object.keys(fieldErrors).forEach(key => {
        fieldErrors[key] = ''
    })
}

// Validate form
const validateForm = async () => {
    let isValid = true

    // Reset errors
    Object.keys(fieldErrors).forEach(key => {
        fieldErrors[key] = ''
    })

    const requiredFields = [
        { key: 'assetCode', name: 'Mã tài sản' },
        { key: 'assetName', name: 'Tên tài sản' },
        { key: 'departmentCode', name: 'Bộ phận sử dụng' },
        { key: 'assetTypeCode', name: 'Loại tài sản' },
        { key: 'quantity', name: 'Số lượng' },
        { key: 'originalPrice', name: 'Nguyên giá' },
        { key: 'purchaseDate', name: 'Ngày mua' },
        { key: 'startDate', name: 'Ngày bắt đầu sử dụng' },
        { key: 'usefulLife', name: 'Số năm sử dụng' },
        { key: 'decreciationRate', name: 'Tỷ lệ hao mòn' },
    ]

    for (const field of requiredFields) {
        // Kiểm tra assetCode chỉ bắt buộc khi edit
        if (field.key === 'assetCode' && props.mode === 'add') {
            if (!form[field.key]) {
                fieldErrors[field.key] = `${field.name} là bắt buộc`
                isValid = false
            } else {
                // Validate mã tài sản không trùng
                const isCodeValid = await validateAssetCode();
                if (!isCodeValid) {
                    isValid = false;
                }
            }
        } else if (!form[field.key] && form[field.key] !== 0) {
            fieldErrors[field.key] = `${field.name} là bắt buộc`
            isValid = false
        }
    }

    // Validate số lượng và nguyên giá phải lớn hơn 0
    if (form.quantity && parseFloat(form.quantity) <= 0) {
        fieldErrors.quantity = 'Số lượng phải lớn hơn 0'
        isValid = false
    }

    if (form.originalPrice && parseFloat(form.originalPrice.toString().replace(/\./g, '').replace(',', '.')) <= 0) {
        fieldErrors.originalPrice = 'Nguyên giá phải lớn hơn 0'
        isValid = false
    }

    // Validate số năm sử dụng phải lớn hơn 0
    if (form.usefulLife && parseFloat(form.usefulLife) <= 0) {
        fieldErrors.usefulLife = 'Số năm sử dụng phải lớn hơn 0'
        isValid = false
    }

    // Validate tỷ lệ hao mòn từ 0-100
    if (form.decreciationRate) {
        const rate = parseFloat(form.decreciationRate.toString().replace(',', '.'));
        if (rate < 0 || rate > 100) {
            fieldErrors.decreciationRate = 'Tỷ lệ hao mòn phải từ 0 đến 100%'
            isValid = false
        }
    }

    return isValid
}

// Xử lý click nút hủy
const handleCancelClick = () => {
    emit('close');
};

// Lưu tài sản
const save = async () => {
    const isValid = await validateForm();
    if (!isValid) {
        showError('Vui lòng kiểm tra lại các trường bắt buộc')
        return
    }

    saving.value = true

    try {
        // Chuẩn bị dữ liệu gửi lên backend theo DTO
        const assetData = {
            assetCode: form.assetCode,
            assetName: form.assetName,
            departmentCode: form.departmentCode,
            assetTypeCode: form.assetTypeCode,
            quantity: parseFloat(form.quantity),
            originalPrice: parseFloat(form.originalPrice.toString().replace(/\./g, '').replace(',', '.')),
            decreciationRate: parseFloat(form.decreciationRate.toString().replace(',', '.')),
            purchaseDate: dayjs(form.purchaseDate, 'DD/MM/YYYY').format('YYYY-MM-DD'),
            startDate: dayjs(form.startDate, 'DD/MM/YYYY').format('YYYY-MM-DD'),
            usefulLife: parseInt(form.usefulLife),
            trackingStartYear: parseInt(form.trackingYear),
            createdDate: new Date().toISOString()
        }

        emit('save', assetData)

    } catch (error) {
        console.error('Save error:', error)
        showError('Có lỗi xảy ra khi lưu tài sản')
    } finally {
        saving.value = false
    }
}

// Xử lý ngày mua thay đổi
const handlePurchaseDateChange = (date) => {
    if (date) {
        form.purchaseDate = date.format('DD/MM/YYYY')
        form.trackingYear = date.year().toString()
    } else {
        form.purchaseDate = ''
        form.trackingYear = ''
    }
}

// Xử lý ngày bắt đầu sử dụng thay đổi
const handleStartDateChange = (date) => {
    if (date) {
        form.startDate = date.format('DD/MM/YYYY')
    } else {
        form.startDate = ''
    }
}

// Vô hiệu hóa ngày trong tương lai
const disabledDate = (current) => {
    return current && current > dayjs().endOf('day')
}

// Theo dõi original price và decreciation rate để tính annual decreciation
watch(() => [form.originalPrice, form.decreciationRate], () => {
    calculateAnnualDecreciation()
}, { deep: true })

// Theo dõi số năm sử dụng để tính tỷ lệ hao mòn
watch(() => form.usefulLife, (newValue) => {
    if (newValue) {
        calculateDecreciationRate();
    }
}, { deep: true })

// Theo dõi khi departmentOptionsForForm thay đổi để điền tên
watch(departmentOptionsForForm, (newOptions) => {
    if (newOptions && newOptions.length > 0 && form.departmentCode) {
        fillDepartmentNameFromCode();
    }
});

// Theo dõi khi assetTypeOptionsForForm thay đổi để điền tên
watch(assetTypeOptionsForForm, (newOptions) => {
    if (newOptions && newOptions.length > 0 && form.assetTypeCode) {
        fillAssetTypeInfoFromCode();
    }
});

// Khởi tạo
onMounted(async () => {
    // Load dropdown data
    await loadData()
    
    // Load danh sách mã tài sản hiện có
    await loadExistingAssetCodes()

    // Load next code nếu là thêm mới
    if (props.mode === 'add') {
        await loadNextCode()
    }
})

// Theo dõi assetData để điền form khi edit/copy
watch(() => props.assetData, async (newData) => {
    if (newData) {
        // Điền dữ liệu từ assetData
        form.assetCode = newData.assetCode || '';
        form.assetName = newData.assetName || '';
        form.departmentCode = newData.departmentCode || '';
        form.assetTypeCode = newData.assetTypeCode || '';
        form.departmentName = newData.departmentName || '';
        form.assetTypeName = newData.assetTypeName || '';
        form.quantity = newData.quantity?.toString() || '';
        form.originalPrice = newData.originalPrice?.toString() || '';

        // Format ngày
        if (newData.purchaseDate) {
            const purchaseDate = dayjs(newData.purchaseDate);
            if (purchaseDate.isValid()) {
                form.purchaseDate = purchaseDate.format('DD/MM/YYYY');
                purchaseDateValue.value = purchaseDate;
                form.trackingYear = purchaseDate.year().toString();
            }
        }

        if (newData.startDate) {
            const startDate = dayjs(newData.startDate);
            if (startDate.isValid()) {
                form.startDate = startDate.format('DD/MM/YYYY');
                startDateValue.value = startDate;
            }
        }

        // Điền các thông tin khác
        form.usefulLife = newData.usefulLife?.toString() || '';
        form.decreciationRate = newData.decreciationRate?.toString() || '';
        form.annualDecreciation = newData.annualDecreciation?.toString() || '';

        // Tính toán lại annual decreciation
        calculateAnnualDecreciation();

        // Đợi một chút để options đã được load xong
        await nextTick();

        // Gọi hàm điền tên từ mã
        fillDepartmentNameFromCode();
        fillAssetTypeInfoFromCode();

        // QUAN TRỌNG: Khi là mode EDIT, đánh dấu rằng người dùng ĐÃ chỉnh sửa
        // để khi chọn loại tài sản khác không bị ghi đè
        if (props.mode === 'edit') {
            if (form.decreciationRate && form.decreciationRate !== '') {
                userModifiedDecreciationRate.value = true;
            }
            if (form.usefulLife && form.usefulLife !== '') {
                userModifiedUsefulLife.value = true;
            }
        } else {
            // Mode ADD: reset trạng thái chỉnh sửa
            userModifiedDecreciationRate.value = false;
            userModifiedUsefulLife.value = false;
        }

        // Nếu là copy, load mã mới
        if (props.mode === 'add' && newData.assetCode) {
            await loadExistingAssetCodes(); // Cập nhật danh sách mã
            await loadNextCode();
        }
    } else {
        resetForm();
    }
}, { immediate: true })

// Theo dõi mode để reset form khi chuyển từ edit sang add
watch(() => props.mode, (newMode) => {
    if (newMode === 'add') {
        resetForm()
        loadNextCode()
    }
})
</script>

<style scoped>
/* Giữ nguyên các style cũ */
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
}

/* Style cho date picker */
:deep(.ant-picker) {
    width: 100%;
    height: 36px;
    border-radius: 4px;
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