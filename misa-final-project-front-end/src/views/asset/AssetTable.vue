<template>
    <div ref="tableContainerRef" class="ms-table-wrapper h-full flex flex-col overflow-hidden">
        <!-- Bảng dữ liệu -->
        <div class="table-content flex-1 overflow-auto">
            <a-table :key="forceUpdateKey" :dataSource="tableData" :columns="processedColumns" :pagination="false"
                :scroll="{ x: '500px', y: scrollY }" :custom-row="customRow" class="ms-table" size="middle"
                :loading="loading" :locale="{ emptyText: 'Không có dữ liệu' }" @resizeColumn="handleResizeColumn">

                <!-- Header với resize handle -->
                <template #headerCell="{ column, title }">
                    <div class="table-header-cell" :data-column-key="column.key">
                        <div class="header-content relative">
                            <div class="header-text" :title="title">
                                {{ title }}
                            </div>
                            <div v-if="column.resizable"
                                class="resize-handle absolute right-0 top-0 bottom-0 w-3 cursor-col-resize z-10 hover:bg-blue-400 active:bg-blue-600"
                                @mousedown="(e) => startResize(e, column.key)"></div>
                        </div>
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
                        {{ (pagination.pageNumber - 1) * pagination.pageSize + index + 1 }}
                    </template>

                    <!-- Hành động (chỉ hiển thị khi hover/selected) -->
                    <template v-else-if="column.key === 'action'">
                        <div class="flex justify-center gap-1 action-buttons"
                            :class="{ 'visible': isActionVisible(record.key) }">
                            <button @click.stop="handleEdit(record)" class="action-btn" title="Sửa">
                                <div class="icon-edit"></div>
                            </button>
                            <button @click.stop="handleCopy(record)" class="action-btn" title="Xóa">
                                <div class="icon-copy-dark"></div>
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

                    <!-- Các cột khác - ĐÃ SỬA: Thêm tooltip -->
                    <template v-else>
                        <div class="table-cell-content" :title="getCellTooltip(record[column.dataIndex])">
                            <span class="truncate block w-full">
                                {{ record[column.dataIndex] || '-' }}
                            </span>
                        </div>
                    </template>
                </template>
            </a-table>
        </div>

        <!-- Pagination -->
        <div v-if="showPagination" class="table-footer">
            <div class="flex items-center justify-between">
                <div class="pagination-info">
                    Tổng số: <strong>{{ totalRecords }}</strong> bản ghi
                </div>
                <div class="flex items-center gap-2">
                    <select v-model="internalPageSize" @change="handlePageSizeChange"
                        class="border border-gray-300 rounded px-2 py-1 text-sm">
                        <option value="10">10 / trang</option>
                        <option value="20">20 / trang</option>
                        <option value="50">50 / trang</option>
                        <option value="100">100 / trang</option>
                    </select>

                    <div class="flex items-center gap-1">
                        <button :disabled="!hasPrevious" @click="handlePageChange(currentPage - 1)"
                            class="px-2 py-1 border border-gray-300 rounded disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50">
                            &lt;
                        </button>

                        <span class="px-3 py-1 text-sm">
                            {{ currentPage }} / {{ totalPages }}
                        </span>

                        <button :disabled="!hasNext" @click="handlePageChange(currentPage + 1)"
                            class="px-2 py-1 border border-gray-300 rounded disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50">
                            &gt;
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
  
<script setup>
import { ref, onMounted, onUnmounted, computed, watch, nextTick } from 'vue'
import { Table } from 'ant-design-vue'
import MsCheckbox from '@/components/MsCheckbox.vue'

const ATable = Table

// Props
const props = defineProps({
    loading: { type: Boolean, default: false },
    dataSource: {
        type: Array,
        default: () => []
    },
    columns: {
        type: Array,
        default: () => []
    },
    pagination: {
        type: Object,
        default: () => ({
            pageNumber: 1,
            pageSize: 10,
            totalPages: 0,
            totalRecords: 0,
            hasPrevious: false,
            hasNext: false
        })
    },
    showPagination: {
        type: Boolean,
        default: true
    }
})

// Emits
const emit = defineEmits(['selection-change', 'row-click', 'edit', 'delete', 'page-change', 'page-size-change', 'column-resize'])

// State
const selectedRowKeys = ref([])
const hoveredRowKey = ref(null)
const forceUpdateKey = ref(0)
const tableContainerRef = ref(null)
const scrollY = ref('calc(100% - 200px)')
const internalPageSize = ref(props.pagination.pageSize || 10)

// Resize state
const resizingColumn = ref(null)
const startX = ref(0)
const startWidth = ref(0)
const columnWidths = ref({})
const isResizing = ref(false)

// Computed
const tableData = computed(() => {
    return props.dataSource || []
})

const currentPage = computed(() => props.pagination.pageNumber || 1)
const totalPages = computed(() => props.pagination.totalPages || 1)
const totalRecords = computed(() => props.pagination.totalRecords || 0)
const hasPrevious = computed(() => props.pagination.hasPrevious || false)
const hasNext = computed(() => props.pagination.hasNext || false)

// Columns mặc định
const defaultColumns = [
    { title: '', key: 'selection', dataIndex: 'selection', width: 50, align: 'center', resizable: false, fixed: 'left' },
    { title: 'STT', key: 'index', width: 60, align: 'center', resizable: false, fixed: 'left' },
    { title: 'Mã tài sản', dataIndex: 'assetCode', key: 'assetCode', minWidth: 140, resizable: true },
    { title: 'Tên tài sản', dataIndex: 'assetName', key: 'assetName', minWidth: 180, resizable: true },
    { title: 'Loại tài sản', dataIndex: 'assetType', key: 'assetType', minWidth: 140, resizable: true },
    { title: 'Bộ phận sử dụng', dataIndex: 'department', key: 'department', minWidth: 160, resizable: true },
    { title: 'Số lượng', dataIndex: 'quantity', key: 'quantity', width: 80, align: 'center', resizable: true },
    { title: 'Nguyên giá', dataIndex: 'originalPrice', key: 'originalPrice', width: 120, align: 'right', resizable: true },
    { title: 'HM/KH lũy kế', dataIndex: 'depreciation', key: 'depreciation', width: 120, align: 'right', resizable: true },
    { title: 'Giá trị còn lại', dataIndex: 'remainingValue', key: 'remainingValue', width: 120, align: 'right', resizable: true },
    { title: 'Chức năng', key: 'action', width: 100, align: 'center', resizable: false, fixed: 'right' }
]

// Xử lý columns với resize
const processedColumns = computed(() => {
    const cols = props.columns && props.columns.length > 0 ? props.columns : defaultColumns

    return cols.map((col) => {
        const width = columnWidths.value[col.key] || col.width || col.minWidth || 100
        return {
            ...col,
            width: width,
            onHeaderCell: (column) => ({
                style: {
                    minWidth: `${column.minWidth || 100}px`,
                    width: `${width}px`,
                    position: 'relative',
                    overflow: 'hidden'
                }
            }),
            onCell: (record, rowIndex) => ({
                style: {
                    maxWidth: `${width}px`,
                    overflow: 'hidden',
                    textOverflow: 'ellipsis',
                    whiteSpace: 'nowrap'
                }
            })
        }
    })
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

const handleCopy = (record) => {
    emit('copy', record)
}

// Format currency
const formatCurrency = (value) => {
    if (!value && value !== 0) return '0'
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND',
        minimumFractionDigits: 0
    }).format(value)
}

// Lấy tooltip cho cell
const getCellTooltip = (value) => {
    if (!value) return ''
    return value.toString()
}

// Resize handlers
const startResize = (e, columnKey) => {
    e.preventDefault()
    e.stopPropagation()

    resizingColumn.value = columnKey
    startX.value = e.clientX
    isResizing.value = true

    const th = e.target.closest('th')
    if (th) {
        startWidth.value = th.offsetWidth
    }

    document.addEventListener('mousemove', handleMouseMove)
    document.addEventListener('mouseup', stopResize)

    // Thêm class để hiển thị resize cursor
    document.body.classList.add('resizing-column')
    document.body.style.cursor = 'col-resize'
    document.body.style.userSelect = 'none'
}

const handleMouseMove = (e) => {
    if (!resizingColumn.value || !isResizing.value) return

    const diff = e.clientX - startX.value
    const newWidth = Math.max(80, startWidth.value + diff) // Minimum width 80px

    columnWidths.value[resizingColumn.value] = newWidth
    forceUpdateKey.value++

    // Emit event để parent component có thể lưu column widths
    emit('column-resize', { columnKey: resizingColumn.value, width: newWidth })
}

const stopResize = () => {
    resizingColumn.value = null
    isResizing.value = false
    document.removeEventListener('mousemove', handleMouseMove)
    document.removeEventListener('mouseup', stopResize)
    document.body.classList.remove('resizing-column')
    document.body.style.cursor = ''
    document.body.style.userSelect = ''
}

// Force update table khi container thay đổi kích thước
const forceUpdateTable = () => {
    forceUpdateKey.value += 1
}

// Pagination handlers
const handlePageChange = (page) => {
    emit('page-change', page)
}

const handlePageSizeChange = () => {
    emit('page-size-change', parseInt(internalPageSize.value))
}

// Handle column resize event từ antd
const handleResizeColumn = (width, column) => {
    if (column.key) {
        columnWidths.value[column.key] = width
        emit('column-resize', { columnKey: column.key, width: width })
    }
}

// Tính toán scroll height
const calculateScrollHeight = () => {
    if (tableContainerRef.value) {
        const container = tableContainerRef.value
        const tableHeight = container.clientHeight
        const footerHeight = props.showPagination ? 60 : 0
        scrollY.value = `${tableHeight - footerHeight - 10}px`
    }
}

// Watch pageSize từ props
watch(() => props.pagination.pageSize, (newSize) => {
    internalPageSize.value = newSize || 10
}, { immediate: true })

// Watch dataSource để tính lại scroll height
watch(() => props.dataSource, () => {
    nextTick(() => {
        calculateScrollHeight()
    })
})

// Sử dụng ResizeObserver để theo dõi thay đổi kích thước container
let resizeObserver = null

onMounted(() => {
    // Tính toán chiều cao scroll ban đầu
    calculateScrollHeight()

    // Tạo ResizeObserver để theo dõi thay đổi kích thước container
    if ('ResizeObserver' in window) {
        resizeObserver = new ResizeObserver(() => {
            calculateScrollHeight()
            forceUpdateTable()
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

    // Cleanup event listeners
    document.removeEventListener('mousemove', handleMouseMove)
    document.removeEventListener('mouseup', stopResize)
})
</script>
  
<style scoped>
.ms-table-wrapper {
    height: 100%;
    display: flex;
    flex-direction: column;
    min-width: 0;
    position: relative;
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
    position: relative;
    user-select: none;
}

.table-header-cell .header-content {
    position: relative;
    height: 100%;
    width: 100%;
}

.header-text {
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
    padding-right: 8px;
    /* Space for resize handle */
}

.table-cell-content {
    padding: 12px 8px;
    color: #4b5563;
    font-size: 13px;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
    min-width: 0;
    cursor: default;
}

.table-cell-content:hover {
    position: relative;
    z-index: 1;
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
    min-width: 100% !important;
    table-layout: fixed !important;
}

.ms-table .ant-table {
    width: 100% !important;
    min-width: 100% !important;
    table-layout: fixed !important;
}

.ms-table .ant-table-container {
    width: 100% !important;
    min-width: 100% !important;
}

.ms-table .ant-table-thead>tr>th {
    border-right: none !important;
    border-bottom: 1px solid #e5e7eb;
    background-color: #f9fafb;
    position: relative;
    transition: background-color 0.2s;
    padding: 0 !important;
}

.ms-table .ant-table-thead>tr>th:hover {
    background-color: #f3f4f6;
}

.ms-table .ant-table-tbody>tr>td {
    border-right: none !important;
    border-bottom: 1px solid #f3f4f6;
    padding: 0 !important;
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

/* Custom styles for table content */
.ms-table .ant-table-tbody>tr>td>div {
    padding: 12px 8px;
}

/* Resize handle improvements */
.resize-handle {
    background: transparent;
    transition: background-color 0.2s;
}

.resize-handle:hover {
    background-color: #1890ff !important;
}

.resize-handle:active {
    background-color: #096dd9 !important;
}

/* Style cho resize indicator */
body.resizing-column {
    cursor: col-resize !important;
}

body.resizing-column * {
    cursor: col-resize !important;
}

/* Style cho empty state */
.ms-table .ant-table-placeholder {
    border-bottom: none !important;
}

.ms-table .ant-table-placeholder:hover>td {
    background-color: white !important;
}

/* Scrollbar styling */
.table-content::-webkit-scrollbar {
    width: 8px;
    height: 8px;
}

.table-content::-webkit-scrollbar-track {
    background: #f1f1f1;
    border-radius: 4px;
}

.table-content::-webkit-scrollbar-thumb {
    background: #c1c1c1;
    border-radius: 4px;
}

.table-content::-webkit-scrollbar-thumb:hover {
    background: #a8a8a8;
}

/* Fix for antd table scroll */
.ant-table-body::-webkit-scrollbar {
    width: 8px;
    height: 8px;
}

.ant-table-body::-webkit-scrollbar-track {
    background: #f1f1f1;
}

.ant-table-body::-webkit-scrollbar-thumb {
    background: #c1c1c1;
    border-radius: 4px;
}

.ant-table-body::-webkit-scrollbar-thumb:hover {
    background: #a8a8a8;
}

@media (max-width: 1400px) {

    .ms-table .ant-table-thead>tr>th,
    .ms-table .ant-table-tbody>tr>td {
        white-space: nowrap;
    }
}
</style>