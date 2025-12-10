<template>
    <div ref="tableContainerRef" class="ms-table-wrapper h-full flex flex-col overflow-hidden">
        <!-- Container chính cho bảng -->
        <div class="table-container flex-1 flex flex-col overflow-hidden">
            <!-- Wrapper để xử lý scroll ngang -->
            <div class="table-scroll-wrapper overflow-x-auto flex-1 flex flex-col" 
                 ref="scrollWrapperRef" 
                 @scroll="handleContainerScroll">
                
                <!-- Header của bảng -->
                <div class="table-header bg-gray-50 border-b border-gray-200 grid" 
                     ref="headerRef">
                    <div v-for="column in processedColumns" :key="column.key"
                        class="table-header-cell relative border-r border-gray-200 last:border-r-0"
                        :class="getHeaderCellClasses(column)"
                        :style="getColumnStyle(column)">
                        <div class="header-content px-3 py-3 font-semibold text-gray-700 text-sm truncate select-none">
                            {{ column.title }}
                        </div>
                    </div>
                </div>

                <!-- Body của bảng -->
                <div class="table-body flex-1 overflow-auto" ref="bodyRef">
                    <!-- Loading state -->
                    <div v-if="loading" class="flex items-center justify-center h-full min-h-[300px] col-span-full">
                        <div class="text-gray-500">Đang tải...</div>
                    </div>

                    <!-- Empty state -->
                    <div v-else-if="tableData.length === 0" class="flex items-center justify-center h-full min-h-[300px] col-span-full">
                        <div class="text-gray-500">Không có dữ liệu</div>
                    </div>

                    <!-- Data rows -->
                    <template v-else>
                        <div v-for="(record, index) in tableData" :key="getRowKey(record, index)"
                            class="table-row border-b border-gray-100 hover:bg-gray-50 grid" 
                            :class="getRowClasses(record)"
                            @mouseenter="hoveredRowKey = record.key" 
                            @mouseleave="hoveredRowKey = null"
                            @click="handleRowClick(record)">
                            
                            <!-- Các cell trong hàng -->
                            <div v-for="column in processedColumns" :key="`${column.key}-${record.key}`"
                                class="table-cell border-r border-gray-100 last:border-r-0"
                                :class="getCellClasses(column)"
                                :style="getColumnStyle(column)">

                                <!-- Checkbox selection -->
                                <div v-if="column.key === 'selection'" class="flex items-center justify-center h-full">
                                    <MsCheckbox :modelValue="selectedRowKeys.includes(record.key)"
                                        @update:modelValue="toggleSelectRow(record.key)" />
                                </div>

                                <!-- STT -->
                                <div v-else-if="column.key === 'index'" class="flex items-center justify-center h-full px-2 py-2">
                                    {{ getRowIndex(index) }}
                                </div>

                                <!-- Action buttons -->
                                <div v-else-if="column.key === 'action'" class="flex items-center justify-center h-full">
                                    <div class="flex justify-center gap-1 action-buttons"
                                        :class="{ 'visible': isActionVisible(record.key) }">
                                        <button @click.stop="handleEdit(record)" class="action-btn" title="Sửa">
                                            <div class="icon-edit"></div>
                                        </button>
                                        <button @click.stop="handleCopy(record)" class="action-btn" title="Xóa">
                                            <div class="icon-copy-dark"></div>
                                        </button>
                                    </div>
                                </div>

                                <!-- Số lượng -->
                                <div v-else-if="column.dataIndex === 'quantity'"
                                    class="flex items-center justify-center h-full px-2 py-2">
                                    {{ formatNumber(record[column.dataIndex]) }}
                                </div>

                                <!-- Các cột số tiền -->
                                <div v-else-if="['originalPrice', 'decreciation', 'remainingValue'].includes(column.dataIndex)"
                                    class="flex items-center justify-end h-full px-2 py-2">
                                    {{ formatCurrency(record[column.dataIndex]) }}
                                </div>

                                <!-- Các cột khác -->
                                <div v-else class="flex items-center h-full px-2 py-2 truncate"
                                    :title="getCellTooltip(record[column.dataIndex])">
                                    {{ record[column.dataIndex] || '-' }}
                                </div>
                            </div>
                        </div>
                    </template>
                </div>
            </div>
        </div>

        <!-- Footer với Pagination và Statistics -->
        <div v-if="showPagination" class="table-footer bg-white border-t border-gray-200 py-2 px-3">
            <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-2">
                <!-- Phần bên trái: Thông tin phân trang -->
                <div class="flex items-center flex-wrap gap-2 md:gap-4">
                    <div class="pagination-info flex items-center text-gray-600 text-xs md:text-sm">
                        Tổng số: <strong class="mx-1">{{ totalRecords }}</strong> bản ghi
                    </div>

                    <div class="flex items-center gap-2">
                        <MsDropdown v-model="internalPageSize" :options="pageSizeOptions" class="w-16 md:w-20"
                            @change="handlePageSizeChange" :show-search="false" size="small" />

                        <div class="flex items-center gap-1">
                            <button :disabled="!hasPrevious" @click="handlePageChange(currentPage - 1)"
                                class="pagination-btn pagination-nav" title="Trang trước">
                                <div class="icon-previous"></div>
                            </button>

                            <template v-for="(page, index) in displayedPages" :key="`page-${index}`">
                                <span v-if="page === '...'" class="px-1 text-gray-400 text-xs">
                                    ...
                                </span>
                                <button v-else @click="handlePageChange(page)" :class="[
                                    'pagination-btn',
                                    page === currentPage ? 'pagination-active' : ''
                                ]">
                                    <span class="text-xs">{{ page }}</span>
                                </button>
                            </template>

                            <button :disabled="!hasNext" @click="handlePageChange(currentPage + 1)"
                                class="pagination-btn pagination-nav" title="Trang sau">
                                <div class="icon-next"></div>
                            </button>
                        </div>
                    </div>
                </div>

                <!-- Phần bên phải: Statistics -->
                <div class="statistics-wrapper flex-1 mt-2 md:mt-0 md:ml-4 min-w-0">
                    <div class="statistics-content flex items-center overflow-x-auto" ref="statisticsRef">
                        <!-- Chỉ hiển thị các cột có thống kê -->
                        <div v-for="column in statisticColumns" :key="`stat-${column.dataIndex}`"
                            class="statistic-item flex-shrink-0 flex items-center px-1 md:px-2"
                            :style="{ width: getStatisticWidth(column) }">
                            
                            <div class="statistic-value text-xs md:text-sm font-semibold text-gray-900 rounded px-1 md:px-2 py-1 bg-gray-100 whitespace-nowrap"
                                :class="getStatisticValueClasses(column)">
                                {{ getStatisticValue(column.dataIndex) }}
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, computed, watch } from 'vue'
import MsCheckbox from '@/components/MsCheckbox.vue'
import MsDropdown from '@/components/MsDropdown.vue'

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
    statistics: {
        type: Object,
        default: () => ({
            totalQuantity: 0,
            totalOriginalPrice: 0,
            totalAnnualDecreciation: 0,
            totalRemainingValue: 0
        })
    },
    showPagination: {
        type: Boolean,
        default: true
    },
    rowKey: {
        type: [String, Function],
        default: 'key'
    }
})

// Emits
const emit = defineEmits(['selection-change', 'row-click', 'edit', 'copy', 'page-change', 'page-size-change'])

// State
const selectedRowKeys = ref([])
const hoveredRowKey = ref(null)
const scrollWrapperRef = ref(null)
const statisticsRef = ref(null)
const internalPageSize = ref(props.pagination.pageSize || 10)

// Options cho dropdown số bản ghi/trang
const pageSizeOptions = [
    { label: '10', value: 10 },
    { label: '20', value: 20 },
    { label: '50', value: 50 },
    { label: '100', value: 100 }
]

// Computed
const tableData = computed(() => {
    return props.dataSource || []
})

const currentPage = computed(() => props.pagination.pageNumber || 1)
const totalPages = computed(() => props.pagination.totalPages || 1)
const totalRecords = computed(() => props.pagination.totalRecords || 0)
const hasPrevious = computed(() => props.pagination.hasPrevious || false)
const hasNext = computed(() => props.pagination.hasNext || false)

// Tính toán các trang cần hiển thị với dấu ...
const displayedPages = computed(() => {
    const current = currentPage.value
    const total = totalPages.value
    
    if (total <= 5) {
        return Array.from({ length: total }, (_, i) => i + 1)
    }
    
    const pages = [1]
    
    if (current > 3) {
        pages.push('...')
    }
    
    const start = Math.max(2, current - 1)
    const end = Math.min(total - 1, current + 1)
    
    for (let i = start; i <= end; i++) {
        pages.push(i)
    }
    
    if (current < total - 2) {
        pages.push('...')
    }
    
    if (total > 1) {
        pages.push(total)
    }
    
    return pages
})

// Columns mặc định - Sử dụng đơn vị fr và minmax để linh hoạt
const defaultColumns = [
    { title: '', key: 'selection', dataIndex: 'selection', width: '40px', align: 'center', fixed: 'left' },
    { title: 'STT', key: 'index', width: '50px', align: 'center', fixed: 'left' },
    { title: 'Mã tài sản', dataIndex: 'assetCode', key: 'assetCode', width: 'minmax(100px, 1fr)', minWidth: '100px' },
    { title: 'Tên tài sản', dataIndex: 'assetName', key: 'assetName', width: 'minmax(120px, 2fr)', minWidth: '120px' },
    { title: 'Loại tài sản', dataIndex: 'assetType', key: 'assetType', width: 'minmax(100px, 1fr)', minWidth: '100px' },
    { title: 'Bộ phận sử dụng', dataIndex: 'department', key: 'department', width: 'minmax(120px, 1.5fr)', minWidth: '120px' },
    { title: 'Số lượng', dataIndex: 'quantity', key: 'quantity', width: 'minmax(70px, 0.7fr)', align: 'center' },
    { title: 'Nguyên giá', dataIndex: 'originalPrice', key: 'originalPrice', width: 'minmax(100px, 1fr)', align: 'right' },
    { title: 'HM/KH lũy kế', dataIndex: 'decreciation', key: 'decreciation', width: 'minmax(100px, 1fr)', align: 'right' },
    { title: 'Giá trị còn lại', dataIndex: 'remainingValue', key: 'remainingValue', width: 'minmax(100px, 1fr)', align: 'right' },
    { title: 'Chức năng', key: 'action', width: '80px', align: 'center', fixed: 'right' }
]

// Xử lý columns
const processedColumns = computed(() => {
    const cols = props.columns && props.columns.length > 0 ? props.columns : defaultColumns
    return cols.map((col) => ({
        ...col,
        width: col.width || col.minWidth || 'minmax(80px, 1fr)',
        minWidth: col.minWidth || (typeof col.width === 'string' ? col.width.replace(/minmax\(|\)/g, '').split(',')[0] : '80px')
    }))
})

// Chỉ lấy các columns cần hiển thị thống kê
const statisticColumns = computed(() => {
    return processedColumns.value.filter(col => 
        ['quantity', 'originalPrice', 'decreciation', 'remainingValue'].includes(col.dataIndex)
    )
})

// Lấy style cho mỗi column
const getColumnStyle = (col) => {
    return {
        width: col.width,
        minWidth: col.minWidth,
        maxWidth: col.fixed ? col.width : 'none'
    }
}

// Lấy width cho statistic (nhỏ hơn column width)
const getStatisticWidth = (column) => {
    const width = column.width
    // Nếu là minmax, lấy giá trị min
    if (typeof width === 'string' && width.includes('minmax')) {
        const match = width.match(/minmax\(([^,]+),/)
        if (match) {
            return match[1].trim()
        }
    }
    return typeof width === 'number' ? `${width - 10}px` : width
}

// Lấy row key
const getRowKey = (record, index) => {
    if (typeof props.rowKey === 'function') {
        return props.rowKey(record, index)
    }
    return record[props.rowKey] || `row-${index}`
}

// Lấy số thứ tự
const getRowIndex = (index) => {
    return (currentPage.value - 1) * props.pagination.pageSize + index + 1
}

// Lấy classes cho header cell
const getHeaderCellClasses = (column) => {
    const classes = []
    
    if (column.align === 'center') {
        classes.push('justify-center')
    } else if (column.align === 'right') {
        classes.push('justify-end')
    } else {
        classes.push('justify-start')
    }
    
    return classes
}

// Lấy classes cho cell dựa trên column
const getCellClasses = (column) => {
    const classes = []
    
    if (column.align === 'center') {
        classes.push('justify-center')
    } else if (column.align === 'right') {
        classes.push('justify-end')
    } else {
        classes.push('justify-start')
    }
    
    return classes
}

// Lấy classes cho row
const getRowClasses = (record) => {
    const classes = ['cursor-pointer']
    
    if (selectedRowKeys.value.includes(record.key)) {
        classes.push('bg-blue-50')
    }
    
    return classes
}

// Lấy classes cho statistic value
const getStatisticValueClasses = (column) => {
    const classes = []
    
    if (column.align === 'center') {
        classes.push('text-center')
    } else if (column.align === 'right') {
        classes.push('text-right')
    } else {
        classes.push('text-left')
    }
    
    return classes
}

// Lấy giá trị thống kê
const getStatisticValue = (dataIndex) => {
    const stats = props.statistics
    
    switch (dataIndex) {
        case 'quantity':
            return formatNumber(stats.totalQuantity || 0)
        case 'originalPrice':
            return formatCurrency(stats.totalOriginalPrice || 0)
        case 'decreciation':
            return formatCurrency(stats.totalAnnualDecreciation || 0)
        case 'remainingValue':
            return formatCurrency(stats.totalRemainingValue || 0)
        default:
            return ''
    }
}

// Hành động hiển thị
const isActionVisible = (rowKey) => {
    return hoveredRowKey.value === rowKey || selectedRowKeys.value.includes(rowKey)
}

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

const handleRowClick = (record) => {
    emit('row-click', record)
}

// Format currency
const formatCurrency = (value) => {
    if (value === null || value === undefined) return '0'
    return new Intl.NumberFormat('vi-VN', {
        style: 'decimal',
        minimumFractionDigits: 0,
        maximumFractionDigits: 0
    }).format(value)
}

// Format number cho số lượng
const formatNumber = (value) => {
    if (value === null || value === undefined) return '0'
    return new Intl.NumberFormat('vi-VN', {
        minimumFractionDigits: 0,
        maximumFractionDigits: 2
    }).format(value)
}

// Lấy tooltip cho cell
const getCellTooltip = (value) => {
    if (!value) return ''
    return String(value)
}

// Xử lý scroll của container để đồng bộ với statistics
const handleContainerScroll = () => {
    if (scrollWrapperRef.value && statisticsRef.value) {
        statisticsRef.value.scrollLeft = scrollWrapperRef.value.scrollLeft
    }
}

// Pagination handlers
const handlePageChange = (page) => {
    if (page < 1 || page > totalPages.value) return
    emit('page-change', page)
}

const handlePageSizeChange = () => {
    emit('page-size-change', parseInt(internalPageSize.value))
}

// Watch pageSize từ props
watch(() => props.pagination.pageSize, (newSize) => {
    internalPageSize.value = newSize || 10
}, { immediate: true })

onMounted(() => {
    // Đảm bảo scroll handler được gắn
    if (scrollWrapperRef.value) {
        scrollWrapperRef.value.addEventListener('scroll', handleContainerScroll)
    }
})

onUnmounted(() => {
    if (scrollWrapperRef.value) {
        scrollWrapperRef.value.removeEventListener('scroll', handleContainerScroll)
    }
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

.table-container {
    flex: 1;
    display: flex;
    flex-direction: column;
    overflow: hidden;
    min-width: 0;
}

/* Wrapper để xử lý scroll ngang */
.table-scroll-wrapper {
    flex: 1;
    display: flex;
    flex-direction: column;
    overflow-x: auto;
    overflow-y: hidden;
    scrollbar-width: thin;
}

/* Sử dụng CSS Grid với grid-template-columns động */
.table-header {
    display: grid;
    background-color: #f9fafb;
    border-bottom: 1px solid #e5e7eb;
    position: sticky;
    top: 0;
    z-index: 20;
    min-width: min-content;
    flex-shrink: 0;
    /* Grid template columns mặc định */
    grid-template-columns: 
        40px    /* selection */
        50px    /* STT */
        minmax(100px, 1fr)    /* Mã tài sản */
        minmax(120px, 2fr)    /* Tên tài sản */
        minmax(100px, 1fr)    /* Loại tài sản */
        minmax(120px, 1.5fr)  /* Bộ phận sử dụng */
        minmax(70px, 0.7fr)   /* Số lượng */
        minmax(100px, 1fr)    /* Nguyên giá */
        minmax(100px, 1fr)    /* HM/KH lũy kế */
        minmax(100px, 1fr)    /* Giá trị còn lại */
        80px;   /* Chức năng */
}

.table-row {
    display: grid;
    background-color: white;
    transition: background-color 0.15s ease;
    box-sizing: border-box;
    min-width: min-content;
    flex-shrink: 0;
    /* Sử dụng cùng grid template với header */
    grid-template-columns: 
        40px    /* selection */
        50px    /* STT */
        minmax(100px, 1fr)    /* Mã tài sản */
        minmax(120px, 2fr)    /* Tên tài sản */
        minmax(100px, 1fr)    /* Loại tài sản */
        minmax(120px, 1.5fr)  /* Bộ phận sử dụng */
        minmax(70px, 0.7fr)   /* Số lượng */
        minmax(100px, 1fr)    /* Nguyên giá */
        minmax(100px, 1fr)    /* HM/KH lũy kế */
        minmax(100px, 1fr)    /* Giá trị còn lại */
        80px;   /* Chức năng */
}

.table-header-cell {
    height: 40px;
    display: flex;
    align-items: center;
    background-color: #f9fafb;
    font-weight: 600;
    color: #374151;
    font-size: 12px;
    position: relative;
    user-select: none;
    box-sizing: border-box;
    overflow: hidden;
}

.table-header-cell:hover {
    background-color: #f3f4f6;
}

.table-body {
    flex: 1;
    overflow-y: auto;
    overflow-x: hidden;
    min-width: 0;
    position: relative;
    z-index: 1;
    display: flex;
    flex-direction: column;
    scrollbar-width: thin;
}

.table-row:hover {
    background-color: #f9fafb;
}

.table-cell {
    height: 40px;
    display: flex;
    align-items: center;
    color: #4b5563;
    font-size: 12px;
    overflow: hidden;
    box-sizing: border-box;
    white-space: nowrap;
    text-overflow: ellipsis;
}

.action-buttons {
    opacity: 0;
    transition: opacity 0.2s ease;
    display: flex;
    gap: 4px;
}

.action-buttons.visible {
    opacity: 1;
}

.action-btn {
    width: 24px;
    height: 24px;
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 0;
    border-radius: 3px;
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
    flex-shrink: 0;
    min-height: 50px;
    z-index: 10;
}

.pagination-info {
    font-size: 13px;
    color: #6b7280;
}

/* Statistics wrapper */
.statistics-wrapper {
    flex: 1;
    margin-left: 0.5rem;
    min-width: 0;
    max-width: 100%;
}

.statistics-content {
    height: 36px;
    gap: 4px;
    overflow-x: auto;
    scrollbar-width: thin;
}

.statistic-item {
    min-width: 0;
}

.statistic-value {
    white-space: nowrap;
    height: 28px;
    display: flex;
    align-items: center;
    border-radius: 4px;
    padding: 0 8px;
    width: 100%;
    box-sizing: border-box;
    min-width: fit-content;
}

/* Scrollbar styling */
.table-scroll-wrapper::-webkit-scrollbar {
    height: 8px;
    display: block !important;
}

.table-scroll-wrapper::-webkit-scrollbar-track {
    background: #f1f1f1;
    border-radius: 4px;
}

.table-scroll-wrapper::-webkit-scrollbar-thumb {
    background: #d1d5db;
    border-radius: 4px;
    border: 2px solid #f1f1f1;
}

.table-scroll-wrapper::-webkit-scrollbar-thumb:hover {
    background: #9ca3af;
}

.table-body::-webkit-scrollbar {
    width: 8px;
    display: block !important;
}

.table-body::-webkit-scrollbar-track {
    background: #f1f1f1;
    border-radius: 4px;
}

.table-body::-webkit-scrollbar-thumb {
    background: #d1d5db;
    border-radius: 4px;
    border: 2px solid #f1f1f1;
}

.table-body::-webkit-scrollbar-thumb:hover {
    background: #9ca3af;
}

/* Scrollbar cho statistics */
.statistics-content::-webkit-scrollbar {
    height: 4px;
    display: block !important;
}

.statistics-content::-webkit-scrollbar-track {
    background: #f1f1f1;
    border-radius: 2px;
}

.statistics-content::-webkit-scrollbar-thumb {
    background: #d1d5db;
    border-radius: 2px;
}

.statistics-content::-webkit-scrollbar-thumb:hover {
    background: #9ca3af;
}

/* Phân trang */
.pagination-btn {
    min-width: 28px;
    height: 28px;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 4px;
    background: white;
    cursor: pointer;
    padding: 0 3px;
    font-size: 13px;
    transition: all 0.2s ease;
    font-weight: 400;
}

.pagination-btn:hover:not(:disabled) {
    border-color: #9ca3af;
    color: #374151;
}

.pagination-btn:disabled {
    cursor: not-allowed;
    opacity: 0.5;
    background-color: rgba(0, 0, 0, 0.04);
}

/* Nút trang hiện tại */
.pagination-active {
    background: #4b5563;
    border-color: #4b5563;
    color: white;
    font-weight: 500;
}

.pagination-active:hover {
    background: #4b5563;
    border-color: #4b5563;
    color: white;
}

/* Nút điều hướng */
.pagination-nav {
    min-width: 24px;
    width: 24px;
    padding: 0;
    display: flex;
    align-items: center;
    justify-content: center;
}

/* ==================== RESPONSIVE BREAKPOINTS ==================== */

/* 1. Màn hình 1366px và nhỏ hơn */
@media (max-width: 1366px) {
    .table-header,
    .table-row {
        grid-template-columns: 
            35px    /* selection */
            45px    /* STT */
            minmax(90px, 1fr)     /* Mã tài sản */
            minmax(110px, 1.5fr)  /* Tên tài sản */
            minmax(90px, 0.8fr)   /* Loại tài sản */
            minmax(100px, 1fr)    /* Bộ phận sử dụng */
            minmax(60px, 0.5fr)   /* Số lượng */
            minmax(90px, 0.9fr)   /* Nguyên giá */
            minmax(90px, 0.9fr)   /* HM/KH lũy kế */
            minmax(90px, 0.9fr)   /* Giá trị còn lại */
            70px;   /* Chức năng */
    }
    
    .table-header-cell {
        height: 36px;
        font-size: 11px;
        padding: 0 6px;
    }
    
    .table-cell {
        height: 36px;
        font-size: 11px;
        padding: 0 6px;
    }
    
    .header-content {
        padding: 8px 6px !important;
    }
    
    .statistic-value {
        font-size: 11px;
        padding: 0 6px;
        height: 26px;
    }
    
    .pagination-btn {
        min-width: 26px;
        height: 26px;
        font-size: 12px;
    }
    
    .pagination-nav {
        min-width: 22px;
        width: 22px;
    }
    
    .action-btn {
        width: 22px;
        height: 22px;
    }
}

/* 2. Màn hình 1200px và nhỏ hơn */
@media (max-width: 1200px) {
    .table-header,
    .table-row {
        grid-template-columns: 
            35px    /* selection */
            45px    /* STT */
            minmax(80px, 1fr)     /* Mã tài sản */
            minmax(100px, 1.2fr)  /* Tên tài sản */
            minmax(80px, 0.7fr)   /* Loại tài sản */
            minmax(90px, 0.9fr)   /* Bộ phận sử dụng */
            minmax(55px, 0.5fr)   /* Số lượng */
            minmax(80px, 0.8fr)   /* Nguyên giá */
            minmax(80px, 0.8fr)   /* HM/KH lũy kế */
            minmax(80px, 0.8fr)   /* Giá trị còn lại */
            65px;   /* Chức năng */
    }
}

/* 3. Màn hình 1024px (Tablet Landscape) */
@media (max-width: 1024px) {
    .table-footer > div {
        flex-direction: column;
        align-items: stretch;
        gap: 8px;
    }
    
    .statistics-wrapper {
        max-width: 100%;
        margin-left: 0;
        margin-top: 8px;
        width: 100%;
    }
    
    .statistics-content {
        justify-content: flex-start;
        padding: 2px 0;
        gap: 6px;
    }
    
    .pagination-info {
        font-size: 12px;
    }
    
    .table-header,
    .table-row {
        grid-template-columns: 
            35px    /* selection */
            45px    /* STT */
            minmax(70px, 1fr)     /* Mã tài sản */
            minmax(90px, 1fr)     /* Tên tài sản */
            minmax(70px, 0.6fr)   /* Loại tài sản */
            minmax(80px, 0.8fr)   /* Bộ phận sử dụng */
            minmax(50px, 0.4fr)   /* Số lượng */
            minmax(70px, 0.7fr)   /* Nguyên giá */
            minmax(70px, 0.7fr)   /* HM/KH lũy kế */
            minmax(70px, 0.7fr)   /* Giá trị còn lại */
            60px;   /* Chức năng */
    }
    
    .table-header-cell,
    .table-cell {
        font-size: 10px;
        padding: 0 4px;
    }
}

/* 4. Màn hình 900px - Ẩn cột "Loại tài sản" */
@media (max-width: 900px) {
    .table-header,
    .table-row {
        grid-template-columns: 
            35px    /* selection */
            45px    /* STT */
            minmax(70px, 1fr)     /* Mã tài sản */
            minmax(90px, 1.2fr)   /* Tên tài sản */
            /* Loại tài sản - Ẩn */
            minmax(80px, 0.9fr)   /* Bộ phận sử dụng */
            minmax(50px, 0.4fr)   /* Số lượng */
            minmax(70px, 0.8fr)   /* Nguyên giá */
            minmax(70px, 0.8fr)   /* HM/KH lũy kế */
            minmax(70px, 0.8fr)   /* Giá trị còn lại */
            60px;   /* Chức năng */
    }
    
    /* Ẩn cột Loại tài sản */
    .table-header-cell:nth-child(5),
    .table-cell:nth-child(5) {
        display: none;
    }
}

/* 5. Màn hình 768px (Tablet Portrait) - Ẩn cột "Bộ phận sử dụng" */
@media (max-width: 768px) {
    .table-container {
        font-size: 10px;
    }
    
    .table-header,
    .table-row {
        grid-template-columns: 
            35px    /* selection */
            40px    /* STT */
            minmax(60px, 1fr)     /* Mã tài sản */
            minmax(80px, 1.5fr)   /* Tên tài sản */
            /* Loại tài sản - Ẩn */
            /* Bộ phận sử dụng - Ẩn */
            minmax(45px, 0.4fr)   /* Số lượng */
            minmax(60px, 0.8fr)   /* Nguyên giá */
            minmax(60px, 0.8fr)   /* HM/KH lũy kế */
            minmax(60px, 0.8fr)   /* Giá trị còn lại */
            55px;   /* Chức năng */
    }
    
    .table-header-cell {
        height: 32px;
        font-size: 10px;
    }
    
    .table-cell {
        height: 32px;
        font-size: 10px;
    }
    
    /* Ẩn các cột Loại tài sản và Bộ phận sử dụng */
    .table-header-cell:nth-child(5),
    .table-cell:nth-child(5),
    .table-header-cell:nth-child(6),
    .table-cell:nth-child(6) {
        display: none;
    }
    
    .action-btn {
        width: 22px;
        height: 22px;
    }
    
    .pagination-btn {
        min-width: 24px;
        height: 24px;
        font-size: 11px;
        padding: 0 2px;
    }
    
    .pagination-nav {
        min-width: 20px;
        width: 20px;
    }
    
    .statistic-value {
        font-size: 10px;
        padding: 0 4px;
        height: 24px;
    }
}

/* 6. Màn hình 640px (Mobile lớn) - Ẩn thêm cột số tiền */
@media (max-width: 640px) {
    .table-header,
    .table-row {
        grid-template-columns: 
            30px    /* selection */
            35px    /* STT */
            minmax(50px, 1fr)     /* Mã tài sản */
            minmax(70px, 1.5fr)   /* Tên tài sản */
            /* Loại tài sản - Ẩn */
            /* Bộ phận sử dụng - Ẩn */
            minmax(40px, 0.4fr)   /* Số lượng */
            minmax(50px, 0.8fr)   /* Nguyên giá */
            /* HM/KH lũy kế - Ẩn */
            minmax(50px, 0.8fr)   /* Giá trị còn lại */
            50px;   /* Chức năng */
    }
    
    /* Ẩn các cột không cần thiết */
    .table-header-cell:nth-child(5),
    .table-cell:nth-child(5),
    .table-header-cell:nth-child(6),
    .table-cell:nth-child(6),
    .table-header-cell:nth-child(9),
    .table-cell:nth-child(9) {
        display: none;
    }
    
    .statistics-content {
        gap: 2px;
    }
    
    .statistic-item {
        padding: 0 2px;
    }
}

/* 7. Màn hình 480px (Mobile nhỏ) */
@media (max-width: 480px) {
    .table-header,
    .table-row {
        grid-template-columns: 
            25px    /* selection */
            30px    /* STT */
            minmax(40px, 1fr)     /* Mã tài sản */
            minmax(60px, 1.5fr)   /* Tên tài sản */
            /* Loại tài sản - Ẩn */
            /* Bộ phận sử dụng - Ẩn */
            minmax(35px, 0.3fr)   /* Số lượng */
            minmax(45px, 0.7fr)   /* Nguyên giá */
            /* HM/KH lũy kế - Ẩn */
            minmax(45px, 0.7fr)   /* Giá trị còn lại */
            45px;   /* Chức năng */
    }
    
    .table-header-cell {
        height: 28px;
        font-size: 9px;
        padding: 0 2px;
    }
    
    .table-cell {
        height: 28px;
        font-size: 9px;
        padding: 0 2px;
    }
    
    .pagination-info {
        font-size: 10px;
    }
}

/* Đảm bảo không bị tràn */
.table-container {
    contain: layout style;
    will-change: transform;
}

/* Fix cho Firefox */
@supports (-moz-appearance:none) {
    .table-header,
    .table-row {
        min-width: -moz-min-content;
    }
}

/* Đảm bảo grid không wrap */
.table-header,
.table-row {
    grid-auto-flow: column;
}

/* Tối ưu khi in */
@media print {
    .table-scroll-wrapper {
        overflow: visible !important;
    }
    
    .table-header,
    .table-row {
        grid-template-columns: 40px 50px 100px 140px 100px 120px 70px 100px 100px 100px 80px !important;
    }
}
</style>