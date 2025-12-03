<template>
    <div class="ms-table-container h-full flex flex-col">
        <!-- Bảng dữ liệu với virtual scroll để tối ưu hiệu suất -->
        <a-table :dataSource="tableData" :columns="resizableColumns" :pagination="false"
            :scroll="{ x: 'max-content', y: 'calc(100vh - 240px)' }" :custom-row="customRow" class="custom-ant-table flex-1"
            size="middle" :loading="loading" bordered>

            <!-- Custom header với chức năng checkbox và resize handles -->
            <template #headerCell="{ column }">
                <div class="header-cell-wrapper">
                    <template v-if="column.key === 'selection'">
                        <MsCheckbox :modelValue="isAllSelected" @update:modelValue="toggleSelectAll" />
                    </template>
                    <template v-else>
                        <span class="header-title">{{ column.title }}</span>
                    </template>
                    <div v-if="column.key !== 'selection' && column.key !== 'functionality'" class="resize-handle"
                        @mousedown="startResize($event, column.key)">
                    </div>
                </div>
            </template>

            <!-- Custom cell cho checkbox -->
            <template #bodyCell="{ column, record }">
                <template v-if="column.key === 'selection'">
                    <MsCheckbox :modelValue="selectedRowKeys.includes(record.key)"
                        @update:modelValue="(checked) => toggleSelectRow(record.key, checked)" />
                </template>
                <template v-else-if="column.key === 'functionality'">
                    <div class="flex justify-center">
                        <span class="text-green-600">✅</span>
                    </div>
                </template>
                <template
                    v-else-if="column.dataIndex === 'remainingValue' || column.dataIndex === 'depreciation' || column.dataIndex === 'originalPrice'">
                    <div class="text-right">{{ formatCurrency(record[column.dataIndex]) }}</div>
                </template>
                <template v-else>
                    <div class="truncate" :title="record[column.dataIndex]">
                        {{ record[column.dataIndex] }}
                    </div>
                </template>
            </template>
        </a-table>

        <!-- Phân trang custom -->
        <div class="pagination-custom border-t border-gray-200 bg-white px-4 py-3 flex items-center justify-between">
            <div class="flex items-center space-x-2 text-sm text-gray-700">
                <span>Tổng số: <strong>{{ totalRecords }}</strong> bản ghi</span>
            </div>
            <div class="flex items-center space-x-2">
                <span class="text-sm text-gray-700 mr-2">20</span>
                <button @click="prevPage" :disabled="currentPage === 1"
                    class="p-1 rounded border border-gray-300 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed">
                    &lt;
                </button>
                <button v-for="page in visiblePages" :key="page" @click="goToPage(page)" :class="[
                    'px-3 py-1 rounded text-sm',
                    currentPage === page
                        ? 'bg-blue-600 text-white'
                        : 'border border-gray-300 hover:bg-gray-50'
                ]">
                    {{ page }}
                </button>
                <button @click="nextPage" :disabled="currentPage === totalPages"
                    class="p-1 rounded border border-gray-300 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed">
                    &gt;
                </button>
            </div>
        </div>
    </div>
</template>
  
<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { Table } from 'ant-design-vue'
import MsCheckbox from '@/components/MsCheckbox.vue'

const ATable = Table

// Props và emits
const props = defineProps({
    data: {
        type: Array,
        default: () => []
    },
    loading: {
        type: Boolean,
        default: false
    }
})

const emit = defineEmits(['selection-change', 'row-click'])

// State
const tableData = ref([])
const selectedRowKeys = ref([])
const currentPage = ref(1)
const pageSize = 20
const totalRecords = ref(200)

// Column widths state (có thể kéo giãn)
const columnWidths = ref({
    selection:40,
    index: 70,
    assetCode: 130,
    assetName: 160,
    assetType: 120,
    department: 150,
    quantity: 70,
    originalPrice: 130,
    depreciation: 130,
    remainingValue: 130,
    functionality: 90
})

// Resize state
const isResizing = ref(false)
const resizingColumn = ref('')
const startX = ref(0)
const startWidth = ref(0)

// Columns definition với độ rộng động
const columns = ref([
    {
        title: '',
        key: 'selection',
        dataIndex: 'selection',
        fixed: 'left',
        align: 'center'
    },
    {
        title: 'STT',
        dataIndex: 'index',
        key: 'index',
        fixed: 'left',
        align: 'center'
    },
    {
        title: 'Mã tài sản',
        dataIndex: 'assetCode',
        key: 'assetCode'
    },
    {
        title: 'Tên tài sản',
        dataIndex: 'assetName',
        key: 'assetName'
    },
    {
        title: 'Loại tài sản',
        dataIndex: 'assetType',
        key: 'assetType'
    },
    {
        title: 'Bộ phận sử dụng',
        dataIndex: 'department',
        key: 'department'
    },
    {
        title: 'Số lượng',
        dataIndex: 'quantity',
        key: 'quantity',
        align: 'center'
    },
    {
        title: 'Nguyên giá',
        dataIndex: 'originalPrice',
        key: 'originalPrice',
        align: 'right'
    },
    {
        title: 'HM/KH lũy kế',
        dataIndex: 'depreciation',
        key: 'depreciation',
        align: 'right'
    },
    {
        title: 'Giá trị còn lại',
        dataIndex: 'remainingValue',
        key: 'remainingValue',
        align: 'right'
    },
    {
        title: 'Chức năng',
        key: 'functionality',
        fixed: 'right',
        align: 'center'
    }
])

// Computed columns với độ rộng động
const resizableColumns = computed(() => {
    return columns.value.map(column => ({
        ...column,
        width: columnWidths.value[column.key] || 100,
        ellipsis: column.key !== 'selection' && column.key !== 'functionality'
    }))
})

// Computed
const totalPages = computed(() => Math.ceil(totalRecords.value / pageSize))
const visiblePages = computed(() => {
    const pages = []
    const start = Math.max(1, currentPage.value - 1)
    const end = Math.min(totalPages.value, start + 2)

    for (let i = start; i <= end; i++) {
        pages.push(i)
    }
    return pages
})

const isAllSelected = computed(() => {
    if (tableData.value.length === 0) return false
    return tableData.value.every(item => selectedRowKeys.value.includes(item.key))
})

// Methods
const formatCurrency = (value) => {
    if (!value) return '0'
    return new Intl.NumberFormat('vi-VN').format(value)
}

const generateSampleData = () => {
    const sampleData = []
    // Tạo 200 bản ghi để test scroll
    for (let i = 1; i <= 200; i++) {
        const record = {
            key: i,
            index: i,
            assetCode: `ASSET${1000 + i}`,
            assetName: `Tài sản ${i}`,
            assetType: i % 3 === 0 ? 'Máy vi tính' : i % 3 === 1 ? 'Thiết bị văn phòng' : 'Nội thất',
            department: `Phòng ${['A', 'B', 'C', 'D'][i % 4]}`,
            quantity: Math.floor(Math.random() * 10) + 1,
            originalPrice: Math.floor(Math.random() * 100000000) + 1000000,
            depreciation: Math.floor(Math.random() * 20000000) + 100000,
            remainingValue: Math.floor(Math.random() * 80000000) + 1000000
        }
        // Tính toán giá trị còn lại
        record.remainingValue = record.originalPrice - record.depreciation
        sampleData.push(record)
    }
    tableData.value = sampleData
    totalRecords.value = 200
}

// Resize methods
const startResize = (event, columnKey) => {
    isResizing.value = true
    resizingColumn.value = columnKey
    startX.value = event.clientX
    startWidth.value = columnWidths.value[columnKey]

    event.preventDefault()
    event.stopPropagation()

    document.addEventListener('mousemove', handleResize)
    document.addEventListener('mouseup', stopResize)
}

const handleResize = (event) => {
    if (!isResizing.value) return

    const diff = event.clientX - startX.value
    const newWidth = Math.max(50, startWidth.value + diff) // Minimum width 50px

    columnWidths.value[resizingColumn.value] = newWidth
}

const stopResize = () => {
    isResizing.value = false
    resizingColumn.value = ''
    document.removeEventListener('mousemove', handleResize)
    document.removeEventListener('mouseup', stopResize)
}

const toggleSelectAll = (checked) => {
    if (checked) {
        selectedRowKeys.value = tableData.value.map(item => item.key)
    } else {
        selectedRowKeys.value = []
    }
    emitSelectionChange()
}

const toggleSelectRow = (key, checked) => {
    if (checked) {
        if (!selectedRowKeys.value.includes(key)) {
            selectedRowKeys.value.push(key)
        }
    } else {
        const index = selectedRowKeys.value.indexOf(key)
        if (index > -1) {
            selectedRowKeys.value.splice(index, 1)
        }
    }
    emitSelectionChange()
}

const emitSelectionChange = () => {
    emit('selection-change', selectedRowKeys.value)
}

const customRow = (record) => {
    return {
        onClick: () => {
            emit('row-click', record)
        }
    }
}

const goToPage = (page) => {
    currentPage.value = page
    // Thực tế sẽ gọi API để lấy dữ liệu trang mới
}

const prevPage = () => {
    if (currentPage.value > 1) {
        currentPage.value--
        // Thực tế sẽ gọi API để lấy dữ liệu trang mới
    }
}

const nextPage = () => {
    if (currentPage.value < totalPages.value) {
        currentPage.value++
        // Thực tế sẽ gọi API để lấy dữ liệu trang mới
    }
}

// Cleanup
onUnmounted(() => {
    document.removeEventListener('mousemove', handleResize)
    document.removeEventListener('mouseup', stopResize)
})

// Lifecycle
onMounted(() => {
    generateSampleData()
})
</script>
  
<style scoped>
.ms-table-container {
    height: 100%;
    display: flex;
    flex-direction: column;
}

.custom-ant-table {
    flex: 1;
    overflow: hidden;
}

/* Header với resize handle */
.header-cell-wrapper {
    position: relative;
    display: flex;
    align-items: center;
    justify-content: center;
    height: 100%;
    padding: 0 4px;
}

.header-title {
    flex: 1;
    text-align: center;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
}

.resize-handle {
    position: absolute;
    right: 0;
    top: 0;
    bottom: 0;
    width: 5px;
    cursor: col-resize;
    background-color: transparent;
    transition: background-color 0.2s;
}

.resize-handle:hover {
    background-color: #1890ff;
}

/* Cải thiện hiển thị table */
.custom-ant-table :deep(.ant-table) {
    font-size: 12px;
}

.custom-ant-table :deep(.ant-table-thead > tr > th) {
    background-color: #f8fafc;
    font-weight: 600;
    color: #374151;
    border-bottom: 1px solid #e5e7eb;
    padding: 8px 4px;
    white-space: nowrap;
    position: relative;
    user-select: none;
}

.custom-ant-table :deep(.ant-table-tbody > tr > td) {
    padding: 8px 4px;
    border-bottom: 1px solid #f3f4f6;
    color: #4b5563;
    font-size: 12px;
}

.custom-ant-table :deep(.ant-table-tbody > tr:hover > td) {
    background-color: #f9fafb;
}

.custom-ant-table :deep(.ant-table-pagination) {
    display: none !important;
}

/* Fixed columns styling */
.custom-ant-table :deep(.ant-table-cell-fix-left),
.custom-ant-table :deep(.ant-table-cell-fix-right) {
    background-color: white;
    z-index: 1;
}

/* Scrollbar styling */
.custom-ant-table :deep(.ant-table-body) {
    scrollbar-width: thin;
    scrollbar-color: #c1c1c1 #f1f1f1;
}

.custom-ant-table :deep(.ant-table-body::-webkit-scrollbar) {
    height: 8px;
    width: 8px;
}

.custom-ant-table :deep(.ant-table-body::-webkit-scrollbar-track) {
    background: #f1f1f1;
}

.custom-ant-table :deep(.ant-table-body::-webkit-scrollbar-thumb) {
    background: #c1c1c1;
    border-radius: 4px;
}

.custom-ant-table :deep(.ant-table-body::-webkit-scrollbar-thumb:hover) {
    background: #a8a8a8;
}

/* Cell content */
.truncate {
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
}

/* Pagination */
.pagination-custom {
    flex-shrink: 0;
    border-top: 1px solid #e5e7eb;
    background-color: white;
    padding: 12px 16px;
    white-space: nowrap;
}

/* Resizing cursor */
.resizing * {
    cursor: col-resize !important;
}
</style>
