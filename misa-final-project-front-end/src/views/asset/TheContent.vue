<template>
    <main class="content flex-1 min-h-0 bg-gray-100">
        <div class="h-full flex flex-col p-4 space-y-4">

            <div class="header-content rounded-lg  flex-shrink-0 ">
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
                        <MsButton variant="main" class="px-3 h-[40px] whitespace-nowrap">
                            <span class="mr-2">+</span> Thêm tài sản
                        </MsButton>

                        <button type="button" class="ms-button-outline w-[40px] h-[40px] flex items-center justify-center border border-gray-300 rounded-md hover:bg-gray-50">
                            <div class="icon-export"></div>
                        </button>

                        <button type="button" class="ms-button-outline w-[40px] h-[40px] flex items-center justify-center border border-gray-300 rounded-md hover:bg-gray-50">
                            <div class="icon-delete"></div>
                        </button>
                    </div>
                </div>
            </div>

            <div class="main-content bg-white rounded-lg shadow-sm border border-gray-200 flex-1 overflow-hidden">
                <div class="h-full overflow-auto">
                    <table class="min-w-full divide-y divide-gray-200">
                        <thead class="bg-gray-50 sticky top-0 z-10">
                            <tr>
                                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    ID</th>
                                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    Tên</th>
                                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    Email</th>
                                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    Vai trò</th>
                                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    Trạng thái</th>
                                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    Ngày tạo</th>
                                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    Hành động</th>
                            </tr>
                        </thead>

                        <tbody class="bg-white divide-y divide-gray-200">
                            <tr v-for="item in tableData" :key="item.id" class="hover:bg-gray-50">
                                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{{ item.id }}</td>
                                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{{ item.name }}
                                </td>
                                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ item.email }}</td>
                                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ item.role }}</td>
                                <td class="px-6 py-4 whitespace-nowrap">
                                    <span
                                        :class="['inline-flex px-2 py-1 text-xs font-semibold rounded-full',
                                            item.status === 'Active' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800']">
                                        {{ item.status }}
                                    </span>
                                </td>
                                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ item.createdAt }}</td>
                                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                                    <button class="text-blue-600 hover:text-blue-900 mr-3">Sửa</button>
                                    <button class="text-red-600 hover:text-red-900">Xóa</button>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>

        </div>
    </main>
</template>
  
<script setup>
import { ref, onMounted } from 'vue';

// Import components
import MsButton from '@/components/MsButton.vue'
import MsSearch from '@/components/MsSearch.vue'
import MsDropdown from '@/components/MsDropdown.vue'

// Dữ liệu và trạng thái
const searchText = ref('');
const assetType = ref(undefined);
const department = ref(undefined);

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

// Dữ liệu mẫu cho bảng
const tableData = ref([]);

const generateSampleData = () => {
    const roles = ['Admin', 'User', 'Manager', 'Editor'];
    const statuses = ['Active', 'Inactive'];
    const tempTableData = [];

    // Tạo 50 dòng dữ liệu để test scroll
    for (let i = 1; i <= 50; i++) {
        tempTableData.push({
            id: i,
            name: `Người dùng ${i}`,
            email: `user${i}@example.com`,
            role: roles[Math.floor(Math.random() * roles.length)],
            status: statuses[Math.floor(Math.random() * statuses.length)],
            createdAt: `2024-01-${i.toString().padStart(2, '0')}`,
        });
    }
    tableData.value = tempTableData;
};

onMounted(() => {
    generateSampleData();
});

</script>
  
<style scoped>
thead {
    position: sticky;
    top: 0;
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

/* .header-content {
    min-height: 64px;
} */

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
    .header-content > div {
        flex-direction: column;
        align-items: stretch;
    }
    
    .header-content > div > div:first-child {
        width: 100%;
    }
    
    .header-content > div > div:last-child {
        width: 100%;
        justify-content: flex-start;
    }
}
</style>