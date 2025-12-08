<template>
    <main class="content flex-1 min-h-0 bg-gray-100">
        <div class="h-full flex flex-col p-4">

            <div class="header-content rounded-lg flex-shrink-0 mb-3">
                <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-3">

                    <div class="flex flex-wrap items-center gap-3 w-full md:w-auto">
                        <MsSearch v-model="searchText" placeholder="Tìm kiếm tài sản" iconSearchClass="icon-search"
                            class="w-full md:w-[200px] flex-shrink-0" @search="handleSearch" />

                        <MsDropdown v-model="assetTypeFilter" :options="assetTypeOptions" placeholder="Loại tài sản"
                            iconPrefixClass="icon-filter" class="w-full md:w-[150px] flex-shrink-0"
                            @change="handleFilterChange" />

                        <MsDropdown v-model="departmentFilter" :options="departmentOptions" placeholder="Bộ phận sử dụng"
                            iconPrefixClass="icon-filter" class="w-full md:w-[180px] flex-shrink-0"
                            @change="handleFilterChange" />
                    </div>

                    <div class="flex items-center gap-2 flex-shrink-0 mt-3 md:mt-0 flex-nowrap">
                        <MsButton variant="main" class="px-3 h-[40px] whitespace-nowrap" @click="openAddPopup">
                            <span class="mr-2">+</span> Thêm tài sản
                        </MsButton>

                        <button type="button"
                            class="ms-button-outline w-[40px] h-[40px] flex items-center justify-center border border-gray-300 rounded-md hover:bg-gray-50">
                            <div class="icon-export"></div>
                        </button>

                        <button type="button"
                            class="ms-button-outline w-[40px] h-[40px] flex items-center justify-center border border-gray-300 rounded-md hover:bg-gray-50"
                            @click="handleBulkDelete" :disabled="selectedRowKeys.length === 0">
                            <div class="icon-delete"></div>
                        </button>
                    </div>
                </div>
            </div>

            <div class="main-content bg-white rounded-lg shadow-sm border border-gray-200 flex-1 overflow-hidden">
                <AssetTable :dataSource="assets" :loading="loading || dropdownLoading" :pagination="pagination"
                    :statistics="statistics" @selection-change="handleSelectionChange" @row-click="handleRowClick"
                    @edit="handleEdit" @copy="handleCopy" @delete="handleDelete" @page-change="handlePageChange"
                    @page-size-change="handlePageSizeChange" />
            </div>

            <!-- Popup Thêm/Sửa tài sản -->
            <AssetPopup v-if="showPopup" :key="popupKey" :mode="popupMode" :assetData="selectedAsset" @close="closePopup"
                @save="handleSaveAsset" />

            <!-- Popup xác nhận xóa -->
            <MsConfirmDialog :visible="showDeleteConfirm" @update:visible="showDeleteConfirm = $event"
                :message="deleteConfirmMessage" cancel-text="Hủy" confirm-text="Xóa" @confirm="confirmDelete"
                @cancel="cancelDelete" />

            <!-- Popup xác nhận hủy form -->
            <MsConfirmDialog :visible="showCancelFormConfirm" @update:visible="showCancelFormConfirm = $event"
                :message="popupMode === 'edit' ? 'Bạn muốn hủy bỏ sửa tài sản' : 'Bạn muốn hủy thêm tài sản'"
                cancel-text="Không" confirm-text="Hủy bỏ" @confirm="confirmCancelForm" @cancel="cancelCancelForm" />

        </div>
    </main>
</template>

<script setup>
import { ref, onMounted, watch, computed } from 'vue';
import MsButton from '@/components/MsButton.vue'
import MsSearch from '@/components/MsSearch.vue'
import MsDropdown from '@/components/MsDropdown.vue'
import AssetTable from '@/views/asset/AssetTable.vue'
import AssetPopup from '@/views/asset/AssetPopup.vue'
import MsConfirmDialog from '@/components/MsConfirmDialog.vue'
import { useDropdownData } from '@/composables/useDropdownData'
import { useAsset } from '@/composables/useAsset'
import { useDebounceFn } from '@vueuse/core'
import { assetService } from '@/services'
import { useToast } from '@/composables/useToast'

// Khởi tạo toast
const { showSuccess, showError, showWarning } = useToast()

// Dữ liệu và trạng thái
const searchText = ref('');
const assetTypeFilter = ref(undefined);
const departmentFilter = ref(undefined);
const showPopup = ref(false);
const popupMode = ref('add');
const selectedAsset = ref(null);
const popupKey = ref(0);
const selectedRowKeys = ref([]);
const showDeleteConfirm = ref(false);
const showCancelFormConfirm = ref(false);
const deleteType = ref('single');
const assetToDelete = ref(null);

const { assetTypeOptions, departmentOptions, isLoading: dropdownLoading, loadData } = useDropdownData();
const {
    assets,
    loading,
    pagination,
    filters,
    statistics,
    fetchAssets,
    setFilter,
    changePage,
    changePageSize,
    resetFilters
} = useAsset();

// Computed để lấy filter values
const assetTypeFilterValue = () => {
    if (!assetTypeFilter.value) return '';
    const option = assetTypeOptions.value?.find(opt => opt.value === assetTypeFilter.value);
    return option?.data?.assetTypeCode || '';
};

const departmentFilterValue = () => {
    if (!departmentFilter.value) return '';
    const option = departmentOptions.value?.find(opt => opt.value === departmentFilter.value);
    return option?.data?.departmentCode || '';
};

// Computed message cho popup xác nhận xóa
const deleteConfirmMessage = computed(() => {
    if (deleteType.value === 'single' && assetToDelete.value) {
        return `Bạn có chắc chắn muốn xóa tài sản "${assetToDelete.value.assetName}"`;
    } else if (deleteType.value === 'multiple') {
        return `Bạn chắc chắn muốn xóa ${selectedRowKeys.value.length} tài sản đã chọn`;
    }
    return 'Bạn có chắc chắn muốn xóa';
});

// Hàm load dữ liệu
const loadAssets = async () => {
    // Cập nhật filters
    setFilter('searchKeyword', searchText.value?.trim() || '');
    setFilter('assetTypeCode', assetTypeFilterValue());
    setFilter('departmentCode', departmentFilterValue());

    await fetchAssets();
};

// Debounce cho search
const debouncedSearch = useDebounceFn(() => {
    loadAssets();
}, 500);

// Xử lý search
const handleSearch = () => {
    debouncedSearch();
};

// Xử lý filter change
const handleFilterChange = () => {
    loadAssets();
};

// Xử lý thay đổi trang
const handlePageChange = async (page) => {
    changePage(page);
    await loadAssets();
};

// Xử lý thay đổi page size
const handlePageSizeChange = async (size) => {
    changePageSize(size);
    await loadAssets();
};

// Xử lý selection change
const handleSelectionChange = (keys) => {
    selectedRowKeys.value = keys;
};

// Xử lý bulk delete
const handleBulkDelete = () => {
    if (selectedRowKeys.value.length === 0) {
        showWarning('Vui lòng chọn ít nhất một tài sản để xóa');
        return;
    }
    deleteType.value = 'multiple';
    showDeleteConfirm.value = true;
};

// Xử lý copy tài sản
const handleCopy = (record) => {
    popupMode.value = 'add';
    selectedAsset.value = {
        ...record,
        assetCode: '',
        departmentCode: record.departmentCode,
        departmentName: record.departmentName,
        assetTypeCode: record.assetTypeCode,
        assetTypeName: record.assetTypeName
    };
    popupKey.value++;
    showPopup.value = true;
};

// Xử lý xóa tài sản
const handleDelete = (record) => {
    deleteType.value = 'single';
    assetToDelete.value = record;
    showDeleteConfirm.value = true;
};

// Xác nhận xóa
const confirmDelete = async () => {
    try {
        let assetCodes = [];

        if (deleteType.value === 'single' && assetToDelete.value) {
            assetCodes = [assetToDelete.value.assetCode];
        } else if (deleteType.value === 'multiple') {
            assetCodes = selectedRowKeys.value.map(key => {
                const asset = assets.value.find(a => a.key === key);
                return asset?.assetCode;
            }).filter(code => code);
        }

        if (assetCodes.length > 0) {
            const response = await assetService.deleteAssets(assetCodes);
            if (response && response.success) {
                showSuccess(`Đã xóa ${assetCodes.length} tài sản thành công!`);
                selectedRowKeys.value = [];
                await loadAssets();
            } else {
                showError(response?.message || 'Có lỗi xảy ra khi xóa tài sản!');
            }
        }
    } catch (error) {
        console.error('Delete error:', error);
        showError(error.message || 'Có lỗi xảy ra khi xóa tài sản!');
    } finally {
        showDeleteConfirm.value = false;
        assetToDelete.value = null;
    }
};

// Hủy xóa
const cancelDelete = () => {
    showDeleteConfirm.value = false;
    assetToDelete.value = null;
};

// Mở popup thêm tài sản
const openAddPopup = () => {
    popupMode.value = 'add';
    selectedAsset.value = null;
    popupKey.value++;
    showPopup.value = true;
};

// Mở popup sửa tài sản
const handleEdit = (record) => {
    popupMode.value = 'edit';
    selectedAsset.value = {
        ...record,
        departmentCode: record.departmentCode,
        departmentName: record.departmentName,
        assetTypeCode: record.assetTypeCode,
        assetTypeName: record.assetTypeName
    };
    popupKey.value++;
    showPopup.value = true;
};

// Đóng popup
const closePopup = () => {
    // Luôn hiển thị popup xác nhận hủy cho cả ADD và EDIT
    showCancelFormConfirm.value = true;
};

// Xác nhận hủy form
const confirmCancelForm = () => {
    doClosePopup();
    showCancelFormConfirm.value = false;
};

// Hủy bỏ việc hủy form
const cancelCancelForm = () => {
    showCancelFormConfirm.value = false;
};

// Thực hiện đóng popup
const doClosePopup = () => {
    showPopup.value = false;
    setTimeout(() => {
        selectedAsset.value = null;
    }, 100);
};

// Xử lý lưu tài sản từ popup
const handleSaveAsset = async (assetData) => {
    try {
        let response;
        if (popupMode.value === 'add') {
            response = await assetService.createAsset(assetData);
            if (response && response.success) {
                showSuccess('Thêm tài sản thành công!');
                await loadAssets();
                doClosePopup();
            } else {
                showError(response?.message || 'Có lỗi xảy ra khi thêm tài sản!');
            }
        } else {
            // Sử dụng assetCode từ assetData
            response = await assetService.updateAsset(assetData.assetCode, assetData);
            if (response && response.success) {
                showSuccess('Cập nhật tài sản thành công!');
                await loadAssets();
                doClosePopup();
            } else {
                showError(response?.message || 'Có lỗi xảy ra khi cập nhật tài sản!');
            }
        }
    } catch (error) {
        console.error('Save error:', error);
        showError(error.message || 'Có lỗi xảy ra khi lưu tài sản!');
    }
};

// Xử lý click row
const handleRowClick = (record) => {
    console.log('Row clicked:', record);
};

// Watch các filter thay đổi
watch([searchText, assetTypeFilter, departmentFilter], () => {
    changePage(1);
    handleFilterChange();
});

// Khởi tạo dữ liệu
onMounted(async () => {
    // Load dropdown data trước
    await loadData();
    // Sau đó load assets
    await loadAssets();
});
</script>

<style scoped>
/* Giữ nguyên các style CSS đã có */
.main-content {
    overflow: hidden;
}

.main-content ::-webkit-scrollbar {
    width: 6px;
    height: 6px;
}

.main-content ::-webkit-scrollbar-track {
    background: #f1f1f1;
    border-radius: 3px;
}

.main-content ::-webkit-scrollbar-thumb {
    background: #c1c1c1;
    border-radius: 3px;
}

.main-content ::-webkit-scrollbar-thumb:hover {
    background: #a8a8a8;
}

.flex-wrap {
    flex-wrap: wrap;
}

.ms-button-outline {
    background-color: white;
    transition: background-color 0.2s;
}

.ms-button-outline:hover {
    background-color: #f9fafb;
}

.ms-button-outline:disabled {
    opacity: 0.5;
    cursor: not-allowed;
}

.ms-button-outline:disabled:hover {
    background-color: white;
}

/* Responsive */
@media (max-width: 768px) {
    .header-content>div {
        flex-direction: column;
        align-items: stretch;
    }

    .header-content>div>div:first-child {
        width: 100%;
    }

    .header-content>div>div:last-child {
        width: 100%;
        justify-content: flex-start;
    }
}
</style>