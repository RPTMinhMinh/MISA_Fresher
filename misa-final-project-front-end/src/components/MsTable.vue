[file name]: MsTable.vue
<template>
    <div ref="tableContainerRef" class="ms-table-wrapper h-full flex flex-col overflow-hidden">
        <!-- Bảng dữ liệu -->
        <div class="table-content flex-1 overflow-auto">
            <a-table :key="forceUpdateKey" :dataSource="tableData" :columns="columns" :pagination="false"
                :scroll="{ y: 'calc(100vh - 254px)' }" :custom-row="customRow" class="ms-table" size="middle"
                :loading="loading">

                <!-- Header -->
                <template #headerCell="{ column }">
                    <div class="table-header-cell">
                        {{ column.title }}
                    </div>
                </template>

                <!-- Body cell -->
                <template #bodyCell="{ column, record, index }">
                    <!-- Checkbox selection -->
                    <template v-if="column.key === 'selection'">
                        <MsCheckbox :modelValue="selectedRowKeys.includes(record.key)"
                            @update:modelValue="toggleSelectRow(record.key)" />
                    </template>

                    <!-- STT -->
                    <template v-else-if="column.key === 'index'">
                        {{ index + 1 }}
                    </template>

                    <!-- Hành động (chỉ hiển thị khi hover/selected) -->
                    <template v-else-if="column.key === 'action'">
                        <div class="flex justify-center gap-1 action-buttons"
                            :class="{ 'visible': isActionVisible(record.key) }">
                            <button @click.stop="handleEdit(record)" class="action-btn">
                                <div class="icon-edit"></div>
                            </button>
                            <button @click.stop="handleDelete(record)" class="action-btn">
                                <div class="icon-delete-dark"></div>
                            </button>
                        </div>
                    </template>

                    <!-- Cột số tiền -->
                    <template v-else-if="['originalPrice', 'depreciation', 'remainingValue'].includes(column.dataIndex)">
                        <div class="text-right">
                            {{ formatCurrency(record[column.dataIndex]) }}
                        </div>
                    </template>

                    <!-- Cột số lượng -->
                    <template v-else-if="column.dataIndex === 'quantity'">
                        <div class="text-center">
                            {{ record[column.dataIndex] }}
                        </div>
                    </template>

                    <!-- Các cột khác -->
                    <template v-else>
                        <div class="table-cell-content">
                            {{ record[column.dataIndex] }}
                        </div>
                    </template>
                </template>
            </a-table>
        </div>

        <!-- Pagination -->
        <div class="table-footer">
            <div class="pagination-info">
                Tổng số: <strong>{{ tableData.length }}</strong> bản ghi
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, computed } from 'vue'
import { Table } from 'ant-design-vue'
import MsCheckbox from '@/components/MsCheckbox.vue'

const ATable = Table

// Props - Thêm props để nhận dữ liệu từ bên ngoài
const props = defineProps({
    loading: { type: Boolean, default: false },
    dataSource: {
        type: Array,
        default: () => []
    },
    columns: {
        type: Array,
        default: () => []
    }
})

// Emits
const emit = defineEmits(['selection-change', 'row-click', 'edit', 'delete'])

// State
const selectedRowKeys = ref([])
const hoveredRowKey = ref(null)
const forceUpdateKey = ref(0)
const tableContainerRef = ref(null)

// Dữ liệu mẫu (chỉ dùng khi không có dataSource từ props)
const defaultTableData = ref([
    { key: 1, assetCode: '55HTWNT2/2022', assetName: 'Dell Inspiron 3467', assetType: 'Máy vi tính xách tay', department: 'Phòng Hành chính Kế toán', quantity: 1, originalPrice: 20000000, depreciation: 894000, remainingValue: 19106000 },
    { key: 2, assetCode: 'MXT88618', assetName: 'Máy tính xách tay Fujitsu', assetType: 'Máy vi tính xách tay', department: 'Phòng Hành chính Kế toán', quantity: 1, originalPrice: 10000000, depreciation: 1225000, remainingValue: 8775000 },
    { key: 3, assetCode: '37HTWNT2/2022', assetName: 'Dell Inspiron 3467', assetType: 'Máy vi tính xách tay', department: 'Phòng Hành chính Kế toán', quantity: 4, originalPrice: 40000000, depreciation: 1730000, remainingValue: 38270000 },
    { key: 4, assetCode: 'MXT8866', assetName: 'Máy tính xách tay Fujitsu', assetType: 'Máy vi tính xách tay', department: 'Phòng Thư Ký', quantity: 1, originalPrice: 5000000, depreciation: 1646000, remainingValue: 3354000 },
    { key: 5, assetCode: '14HTWNT2/2019', assetName: 'Dell Latitude E 5450', assetType: 'Máy vi tính xách tay', department: 'Phòng Hành chính Kế toán', quantity: 1, originalPrice: 10000000, depreciation: 2456000, remainingValue: 7544000 },
    { key: 6, assetCode: 'DBP03F2/2017', assetName: 'DEL Inspiron 3467', assetType: 'Máy vi tính xách tay', department: 'Phòng Hành chính Kế toán', quantity: 20, originalPrice: 50000000, depreciation: 913000, remainingValue: 49087000 },
    { key: 7, assetCode: 'MXT8869', assetName: 'Máy tính xách tay Fujitsu', assetType: 'Máy vi tính xách tay', department: 'Phòng Hành chính Kế toán', quantity: 1, originalPrice: 50000000, depreciation: 3929000, remainingValue: 46071000 },
    { key: 8, assetCode: '49HTWNT2/2022', assetName: 'Dell Inspiron 3467', assetType: 'Máy vi tính xách tay', department: 'Phòng Tài chính Tổng hợp', quantity: 1, originalPrice: 4000000, depreciation: 432000, remainingValue: 3568000 },
    { key: 9, assetCode: '33HTWNT2/2022', assetName: 'Dell Inspiron 3467', assetType: 'Máy vi tính xách tay', department: 'Phòng Tài chính Tổng hợp', quantity: 1, originalPrice: 20000000, depreciation: 3400000, remainingValue: 16600000 },
    { key: 10, assetCode: '22HTWNT2/2019', assetName: 'Dell Latitude E 5450', assetType: 'Máy vi tính xách tay', department: 'Phòng Tài chính Tổng hợp', quantity: 1, originalPrice: 40000000, depreciation: 3091000, remainingValue: 36909000 }
])

// Columns mặc định (chỉ dùng khi không có columns từ props)
const defaultColumns = [
    { title: '', key: 'selection', dataIndex: 'selection', width: 50, align: 'center' },
    { title: 'STT', key: 'index', width: 60, align: 'center' },
    { title: 'Mã tài sản', dataIndex: 'assetCode', key: 'assetCode', minWidth: 140 },
    { title: 'Tên tài sản', dataIndex: 'assetName', key: 'assetName', minWidth: 180 },
    { title: 'Loại tài sản', dataIndex: 'assetType', key: 'assetType', minWidth: 140 },
    { title: 'Bộ phận sử dụng', dataIndex: 'department', key: 'department', minWidth: 160 },
    { title: 'Số lượng', dataIndex: 'quantity', key: 'quantity', width: 80, align: 'center' },
    { title: 'Nguyên giá', dataIndex: 'originalPrice', key: 'originalPrice', width: 120, align: 'right' },
    { title: 'HM/KH lũy kế', dataIndex: 'depreciation', key: 'depreciation', width: 120, align: 'right' },
    { title: 'Giá trị còn lại', dataIndex: 'remainingValue', key: 'remainingValue', width: 120, align: 'right' },
    { title: 'Chức năng', key: 'action', width: 100, align: 'center' }
]

// Computed để sử dụng dữ liệu từ props hoặc mặc định
const tableData = computed(() => {
    return props.dataSource && props.dataSource.length > 0 ? props.dataSource : defaultTableData.value
})

const columns = computed(() => {
    return props.columns && props.columns.length > 0 ? props.columns : defaultColumns
})

// Hành động hiển thị
const isActionVisible = (rowKey) => {
    return hoveredRowKey.value === rowKey || selectedRowKeys.value.includes(rowKey)
}

// Custom row với hover tracking
const customRow = (record) => ({
    onClick: (event) => {
        if (!event.target.closest('.action-btn')) {
            emit('row-click', record)
        }
    },
    onMouseenter: () => {
        hoveredRowKey.value = record.key
    },
    onMouseleave: () => {
        hoveredRowKey.value = null
    }
})

// Xử lý sự kiện
const toggleSelectRow = (key) => {
    const index = selectedRowKeys.value.indexOf(key)
    if (index > -1) {
        selectedRowKeys.value.splice(index, 1)
    } else {
        selectedRowKeys.value.push(key)
    }
    emit('selection-change', selectedRowKeys.value)
}

const handleEdit = (record) => {
    emit('edit', record)
}

const handleDelete = (record) => {
    if (confirm(`Bạn có chắc chắn muốn xóa tài sản "${record.assetName}"?`)) {
        emit('delete', record)
    }
}

// Format currency
const formatCurrency = (value) => {
    if (!value && value !== 0) return '0'
    return new Intl.NumberFormat('vi-VN').format(value)
}

// Force update table khi container thay đổi kích thước
const forceUpdateTable = () => {
    forceUpdateKey.value += 1
}

// Sử dụng ResizeObserver để theo dõi thay đổi kích thước container
let resizeObserver = null

onMounted(() => {
    // Tạo ResizeObserver để theo dõi thay đổi kích thước
    if ('ResizeObserver' in window) {
        resizeObserver = new ResizeObserver(() => {
            // Debounce để tránh gọi quá nhiều lần
            setTimeout(() => {
                forceUpdateTable()
            }, 100)
        })

        if (tableContainerRef.value) {
            resizeObserver.observe(tableContainerRef.value)
        }
    }

    // Cũng force update sau khi mounted để đảm bảo table render đúng
    setTimeout(() => {
        forceUpdateTable()
    }, 200)
})

onUnmounted(() => {
    if (resizeObserver && tableContainerRef.value) {
        resizeObserver.unobserve(tableContainerRef.value)
        resizeObserver.disconnect()
    }
})
</script>

<style scoped>
/* Giữ nguyên tất cả style */
.ms-table-wrapper {
    height: 100%;
    display: flex;
    flex-direction: column;
    min-width: 0;
}

.table-content {
    flex: 1;
    overflow: auto;
    min-width: 0;
}

.table-header-cell {
    font-weight: 600;
    color: #374151;
    background-color: #f9fafb;
    padding: 12px 8px;
    text-align: center;
    white-space: nowrap;
    min-width: 0;
    overflow: hidden;
    text-overflow: ellipsis;
}

.table-cell-content {
    padding: 12px 8px;
    color: #4b5563;
    font-size: 13px;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
    min-width: 0;
}

.action-buttons {
    opacity: 0;
    transition: opacity 0.2s ease;
}

.action-buttons.visible {
    opacity: 1;
}

.action-btn {
    width: 28px;
    height: 28px;
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 0;
    border-radius: 4px;
    border: 1px solid transparent;
    background: transparent;
    cursor: pointer;
    transition: all 0.2s;
}

.action-btn:hover {
    border-color: #d1d5db;
    background-color: #f3f4f6;
}

.table-footer {
    border-top: 1px solid #e5e7eb;
    background: white;
    padding: 12px 16px;
    flex-shrink: 0;
}

.pagination-info {
    font-size: 14px;
    color: #6b7280;
}

.ms-table {
    width: 100% !important;
    table-layout: auto !important;
}

.ms-table .ant-table {
    width: 100% !important;
    table-layout: auto !important;
}

.ms-table .ant-table-container {
    width: 100% !important;
}

.ms-table .ant-table-thead>tr>th {
    border-right: none !important;
    border-bottom: 1px solid #e5e7eb;
    background-color: #f9fafb;
}

.ms-table .ant-table-tbody>tr>td {
    border-right: none !important;
    border-bottom: 1px solid #f3f4f6;
}

.ms-table .ant-table-selection-column {
    display: none !important;
}

.ms-table .ant-table-selection-col {
    display: none !important;
}

.ms-table .ant-checkbox-wrapper {
    display: none !important;
}

.ms-table .ant-table-tbody>tr:hover>td {
    background-color: #f9fafb;
}

.ms-table .ant-table-cell[align='center'] .text-center {
    text-align: center;
}

.ms-table .ant-table-cell[align='right'] .text-right {
    text-align: right;
}

.ms-table .ant-table-thead>tr>th,
.ms-table .ant-table-tbody>tr>td {
    min-width: 0 !important;
    max-width: none !important;
}

.ms-table .ant-table-content {
    overflow: auto !important;
}

.ms-table .ant-table-body {
    overflow: auto !important;
}

@media (max-width: 1400px) {

    .ms-table .ant-table-thead>tr>th,
    .ms-table .ant-table-tbody>tr>td {
        white-space: nowrap;
    }
}
</style>