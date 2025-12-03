[file name]: Asset.vue
<template>
    <main class="content flex-1 min-h-0 bg-gray-100">
        <div class="h-full flex flex-col p-4 space-y-4">

            <div class="header-content rounded-lg flex-shrink-0">
                <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-3">

                    <div class="flex flex-wrap items-center gap-3 w-full md:w-auto">
                        <MsSearch v-model="searchText" placeholder="Tìm kiếm tài sản" iconSearchClass="icon-search"
                            class="w-full md:w-[200px] flex-shrink-0" />

                        <MsDropdown v-model="assetType" :options="assetTypeOptions" placeholder="Loại tài sản"
                            iconPrefixClass="icon-filter" class="w-full md:w-[150px] flex-shrink-0" />

                        <MsDropdown v-model="department" :options="departmentOptions" placeholder="Bộ phận sử dụng"
                            iconPrefixClass="icon-filter" class="w-full md:w-[180px] flex-shrink-0" />
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
                            class="ms-button-outline w-[40px] h-[40px] flex items-center justify-center border border-gray-300 rounded-md hover:bg-gray-50">
                            <div class="icon-delete"></div>
                        </button>
                    </div>
                </div>
            </div>

            <div class="main-content bg-white rounded-lg shadow-sm border border-gray-200 flex-1 overflow-hidden">
                <MsTable @selection-change="handleSelectionChange" @row-click="handleRowClick" 
                    @edit="handleEdit" @delete="handleDelete" />
            </div>

            <!-- Popup Thêm/Sửa tài sản -->
            <AssetPopup 
                v-if="showPopup"
                :key="popupKey"
                :mode="popupMode"
                :assetData="selectedAsset"
                @close="closePopup"
                @save="handleSaveAsset" 
            />

        </div>
    </main>
</template>
  
<script setup>
import { ref, onMounted } from 'vue';
import MsButton from '@/components/MsButton.vue'
import MsSearch from '@/components/MsSearch.vue'
import MsDropdown from '@/components/MsDropdown.vue'
import MsTable from '@/components/MsTable.vue'
import AssetPopup from '@/views/asset/AssetPopup.vue'

// Dữ liệu và trạng thái
const searchText = ref('');
const assetType = ref(undefined);
const department = ref(undefined);
const showPopup = ref(false);
const popupMode = ref('add');
const selectedAsset = ref(null);
const popupKey = ref(0);

// Dữ liệu mẫu cho Dropdown
const assetTypeOptions = [
    { value: 'all', label: 'Tất cả' },
    { value: 'laptop', label: 'Laptop' },
    { value: 'monitor', label: 'Màn hình' },
];

const departmentOptions = [
    { value: 'it', label: 'Phòng IT' },
    { value: 'hr', label: 'Phòng Nhân sự' },
    { value: 'mkt', label: 'Phòng Marketing' },
];

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
    selectedAsset.value = record;
    popupKey.value++;
    showPopup.value = true;
};

// Xử lý xóa tài sản
const handleDelete = (record) => {
    console.log('Delete asset:', record);
    alert(`Đã xóa tài sản "${record.assetName}"`);
};

// Đóng popup
const closePopup = () => {
    showPopup.value = false;
    // Reset sau khi đóng
    setTimeout(() => {
        selectedAsset.value = null;
    }, 100);
};

// Xử lý sự kiện từ MsTable
const handleSelectionChange = (selectedKeys) => {
    console.log('Selected rows:', selectedKeys);
};

const handleRowClick = (record) => {
    console.log('Row clicked:', record);
};

// Xử lý lưu tài sản từ popup
const handleSaveAsset = (assetData) => {
    if (popupMode.value === 'add') {
        console.log('Adding new asset:', assetData);
        alert('Đã thêm tài sản mới!');
    } else {
        console.log('Updating asset:', assetData);
        alert('Đã cập nhật thông tin tài sản!');
    }
    closePopup();
};

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