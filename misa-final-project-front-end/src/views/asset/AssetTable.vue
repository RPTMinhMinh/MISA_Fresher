<template>
    <div ref="tableContainerRef" class="ms-table-wrapper h-full flex flex-col overflow-hidden">
        <!-- Container chính cho bảng -->
        <div class="table-container flex-1 flex flex-col overflow-hidden">
            <!-- Wrapper để xử lý scroll ngang -->
            <div class="table-scroll-wrapper overflow-x-auto flex-1 flex flex-col" ref="scrollWrapperRef" @scroll="handleContainerScroll">
                <!-- Header của bảng -->
                <div class="table-header bg-gray-50 border-b border-gray-200 flex" ref="headerRef" :style="headerStyle">
                    <div v-for="column in processedColumns" :key="column.key"
                        class="table-header-cell relative flex-shrink-0 border-r border-gray-200 last:border-r-0"
                        :class="column.align === 'center' ? 'justify-center' : column.align === 'right' ? 'justify-end' : 'justify-start'"
                        :style="getColumnStyle(column)">
                        <div class="header-content px-3 py-3 font-semibold text-gray-700 text-sm truncate select-none">
                            {{ column.title }}
                        </div>

                        <!-- Resize handle -->
                        <div v-if="column.resizable"
                            class="resize-handle absolute right-0 top-0 bottom-0 w-1 cursor-col-resize z-10 hover:bg-blue-400 active:bg-blue-600"
                            @mousedown="(e) => startResize(e, column.key)"></div>
                    </div>
                </div>

                <!-- Body của bảng -->
                <div class="table-body flex-1 overflow-hidden" ref="bodyRef">
                    <!-- Loading state -->
                    <div v-if="loading" class="flex items-center justify-center h-full">
                        <div class="text-gray-500">Đang tải...</div>
                    </div>

                    <!-- Empty state -->
                    <div v-else-if="tableData.length === 0" class="flex items-center justify-center h-full">
                        <div class="text-gray-500">Không có dữ liệu</div>
                    </div>

                    <!-- Data rows -->
                    <div v-else>
                        <div v-for="(record, index) in tableData" :key="record.key || index"
                            class="table-row border-b border-gray-100 hover:bg-gray-50 flex" :class="{
                                'bg-blue-50': selectedRowKeys.includes(record.key),
                                'cursor-pointer': true
                            }" @mouseenter="hoveredRowKey = record.key" @mouseleave="hoveredRowKey = null"
                            @click="handleRowClick(record)" :style="rowStyle">

                            <!-- Các cell trong hàng -->
                            <div v-for="column in processedColumns" :key="column.key"
                                class="table-cell flex-shrink-0 border-r border-gray-100 last:border-r-0 truncate"
                                :class="getCellClasses(column)" :style="getColumnStyle(column)">

                                <!-- Checkbox selection -->
                                <div v-if="column.key === 'selection'" class="flex items-center justify-center h-full">
                                    <MsCheckbox :modelValue="selectedRowKeys.includes(record.key)"
                                        @update:modelValue="toggleSelectRow(record.key)" />
                                </div>

                                <!-- STT -->
                                <div v-else-if="column.key === 'index'" class="flex items-center justify-center h-full">
                                    {{ (pagination.pageNumber - 1) * pagination.pageSize + index + 1 }}
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
                                    class="flex items-center justify-center h-full px-3 py-2">
                                    {{ record[column.dataIndex] }}
                                </div>

                                <!-- Các cột số tiền -->
                                <div v-else-if="['originalPrice', 'depreciation', 'remainingValue'].includes(column.dataIndex)"
                                    class="flex items-center justify-end h-full px-3 py-2">
                                    {{ formatCurrency(record[column.dataIndex]) }}
                                </div>

                                <!-- Các cột khác -->
                                <div v-else class="flex items-center h-full px-3 py-2 truncate"
                                    :title="getCellTooltip(record[column.dataIndex])">
                                    {{ record[column.dataIndex] || '-' }}
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Footer với Pagination và Statistics -->
        <div v-if="showPagination" class="table-footer bg-white border-t border-gray-200 py-3 px-4">
            <div class="flex items-center justify-between">
                <!-- Phần bên trái: Thông tin phân trang -->
                <div class="flex items-center gap-10">
                    <div class="pagination-info flex items-center text-gray-600 text-sm">
                        Tổng số: <strong class="mx-1">{{ totalRecords }}</strong> bản ghi
                    </div>

                    <div class="flex items-center gap-2">
                        <MsDropdown v-model="internalPageSize" :options="pageSizeOptions" class="w-20"
                            @change="handlePageSizeChange" :show-search="false" size="small" />

                        <div class="flex items-center gap-1">
                            <button :disabled="!hasPrevious" @click="handlePageChange(currentPage - 1)"
                                class="pagination-btn pagination-nav" title="Trang trước">
                                <div class="icon-previous"></div>
                            </button>

                            <template v-for="(page, index) in displayedPages">
                                <span v-if="page === '...'" :key="`ellipsis-${index}`" class="px-2 text-gray-400">
                                    ...
                                </span>
                                <button v-else :key="`page-${page}`" @click="handlePageChange(page)" :class="[
                                    'pagination-btn',
                                    page === currentPage ? 'pagination-active' : ''
                                ]">
                                    {{ page }}
                                </button>
                            </template>

                            <button :disabled="!hasNext" @click="handlePageChange(currentPage + 1)"
                                class="pagination-btn pagination-nav" title="Trang sau">
                                <div class="icon-next"></div>
                            </button>
                        </div>
                    </div>
                </div>

                <!-- Phần bên phải: Statistics với layout đúng cột -->
                <div class="statistics-wrapper overflow-x-auto flex-1 ml-4 max-w-[50vw]" ref="statisticsWrapperRef">
                    <div class="statistics-container flex" :style="headerStyle">
                        <!-- Placeholders cho các cột trước cột số lượng -->
                        <div v-for="(col, index) in columnsBeforeQuantity" :key="`placeholder-${index}`"
                            class="statistic-placeholder flex-shrink-0" :style="getColumnStyle(col)"></div>

                        <!-- Statistics cells -->
                        <div class="statistic-cell flex-shrink-0 px-3 py-1" :style="getStatisticColumnStyle('quantity')">
                            <div
                                class="statistic-value text-sm font-semibold text-gray-900 text-center bg-gray-100 rounded px-2 py-1">
                                {{ formatNumber(statistics.totalQuantity) }}
                            </div>
                        </div>

                        <div class="statistic-cell flex-shrink-0 px-3 py-1"
                            :style="getStatisticColumnStyle('originalPrice')">
                            <div
                                class="statistic-value text-sm font-semibold text-gray-900 text-right bg-gray-100 rounded px-2 py-1">
                                {{ formatCurrency(statistics.totalOriginalPrice) }}
                            </div>
                        </div>

                        <div class="statistic-cell flex-shrink-0 px-3 py-1"
                            :style="getStatisticColumnStyle('depreciation')">
                            <div
                                class="statistic-value text-sm font-semibold text-gray-900 text-right bg-gray-100 rounded px-2 py-1">
                                {{ formatCurrency(statistics.totalAnnualDecreciation) }}
                            </div>
                        </div>

                        <div class="statistic-cell flex-shrink-0 px-3 py-1"
                            :style="getStatisticColumnStyle('remainingValue')">
                            <div
                                class="statistic-value text-sm font-semibold text-gray-900 text-right bg-gray-100 rounded px-2 py-1">
                                {{ formatCurrency(statistics.totalRemainingValue) }}
                            </div>
                        </div>

                        <!-- Placeholders cho các cột sau cột giá trị còn lại -->
                        <div v-for="(col, index) in columnsAfterRemainingValue" :key="`placeholder-after-${index}`"
                            class="statistic-placeholder flex-shrink-0" :style="getColumnStyle(col)"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, computed, watch, nextTick } from 'vue'
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
    }
})

// Emits
const emit = defineEmits(['selection-change', 'row-click', 'edit', 'copy', 'page-change', 'page-size-change', 'column-resize'])

// State
const selectedRowKeys = ref([])
const hoveredRowKey = ref(null)
const tableContainerRef = ref(null)
const headerRef = ref(null)
const bodyRef = ref(null)
const statisticsWrapperRef = ref(null)
const scrollWrapperRef = ref(null)
const internalPageSize = ref(props.pagination.pageSize || 10)

// Resize state
const resizingColumn = ref(null)
const startX = ref(0)
const startWidth = ref(0)
const columnWidths = ref({})
const isResizing = ref(false)

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
    const pages = []

    if (total <= 5) {
        for (let i = 1; i <= total; i++) {
            pages.push(i)
        }
        return pages
    }

    pages.push(1)

    let startPage = Math.max(2, current - 1)
    let endPage = Math.min(total - 1, current + 1)

    if (startPage > 2) {
        pages.push('...')
    }

    for (let i = startPage; i <= endPage; i++) {
        pages.push(i)
    }

    if (endPage < total - 1) {
        pages.push('...')
    }

    if (total > 1) {
        pages.push(total)
    }

    return pages
})

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
            width: width
        }
    })
})

// Tính tổng chiều rộng của tất cả các cột
const totalWidth = computed(() => {
    return processedColumns.value.reduce((total, col) => {
        const width = columnWidths.value[col.key] || col.width || col.minWidth || 100
        return total + (typeof width === 'number' ? width : parseInt(width, 10))
    }, 0)
})

// Style cho header và row
const headerStyle = computed(() => {
    return {
        width: `${Math.max(totalWidth.value, 100)}px`,
        minWidth: `${Math.max(totalWidth.value, 100)}px`
    }
})

const rowStyle = computed(() => {
    return headerStyle.value
})

// Lấy style cho mỗi column
const getColumnStyle = (col) => {
    const width = columnWidths.value[col.key] || col.width || col.minWidth || 100
    return {
        width: `${width}px`,
        minWidth: `${width}px`,
        maxWidth: `${width}px`
    }
}

// Lấy style cho cột thống kê
const getStatisticColumnStyle = (dataIndex) => {
    const col = processedColumns.value.find(c => c.dataIndex === dataIndex)
    if (!col) return {}

    const width = columnWidths.value[col.key] || col.width || col.minWidth || 100
    return {
        width: `${width}px`,
        minWidth: `${width}px`,
        maxWidth: `${width}px`
    }
}

// Lấy các cột trước cột số lượng
const columnsBeforeQuantity = computed(() => {
    const cols = processedColumns.value
    const quantityIndex = cols.findIndex(col => col.dataIndex === 'quantity')
    return cols.slice(0, quantityIndex)
})

// Lấy các cột sau cột giá trị còn lại
const columnsAfterRemainingValue = computed(() => {
    const cols = processedColumns.value
    const remainingValueIndex = cols.findIndex(col => col.dataIndex === 'remainingValue')
    return cols.slice(remainingValueIndex + 1)
})

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
    if (!value && value !== 0) return '0'
    return new Intl.NumberFormat('vi-VN', {
        style: 'decimal',
        minimumFractionDigits: 0,
        maximumFractionDigits: 0
    }).format(value)
}

// Format number cho số lượng
const formatNumber = (value) => {
    if (!value && value !== 0) return '0'
    return new Intl.NumberFormat('vi-VN', {
        minimumFractionDigits: 0,
        maximumFractionDigits: 2
    }).format(value)
}

// Lấy tooltip cho cell
const getCellTooltip = (value) => {
    if (!value) return ''
    return value.toString()
}

// Xử lý scroll của container để đồng bộ với statistics
const handleContainerScroll = () => {
    if (scrollWrapperRef.value && statisticsWrapperRef.value) {
        statisticsWrapperRef.value.scrollLeft = scrollWrapperRef.value.scrollLeft
    }
}

// Resize handlers
const startResize = (e, columnKey) => {
    e.preventDefault()
    e.stopPropagation()

    resizingColumn.value = columnKey
    startX.value = e.clientX
    isResizing.value = true

    const column = processedColumns.value.find(col => col.key === columnKey)
    if (column) {
        startWidth.value = columnWidths.value[columnKey] || column.width || column.minWidth || 100
    }

    document.addEventListener('mousemove', handleMouseMove)
    document.addEventListener('mouseup', stopResize)

    document.body.classList.add('resizing-column')
    document.body.style.cursor = 'col-resize'
    document.body.style.userSelect = 'none'
}

const handleMouseMove = (e) => {
    if (!resizingColumn.value || !isResizing.value) return

    const diff = e.clientX - startX.value
    const newWidth = Math.max(80, startWidth.value + diff)

    columnWidths.value[resizingColumn.value] = newWidth

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

// Watch columnWidths để cập nhật layout
watch(columnWidths, () => {
    // Force re-render
    nextTick()
}, { deep: true })

onMounted(() => {
    // Khởi tạo column widths từ columns
    processedColumns.value.forEach(col => {
        if (!columnWidths.value[col.key] && col.width) {
            columnWidths.value[col.key] = col.width
        }
    })

    // Đảm bảo scroll handler được gắn
    if (scrollWrapperRef.value) {
        scrollWrapperRef.value.addEventListener('scroll', handleContainerScroll)
    }
})

onUnmounted(() => {
    if (scrollWrapperRef.value) {
        scrollWrapperRef.value.removeEventListener('scroll', handleContainerScroll)
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

.table-container {
    flex: 1;
    display: flex;
    flex-direction: column;
    overflow: hidden;
    min-width: 0;
}

/* Wrapper mới để xử lý scroll ngang */
.table-scroll-wrapper {
    flex: 1;
    display: flex;
    flex-direction: column;
    overflow-x: auto;
    overflow-y: hidden;
}

.table-header {
    flex-shrink: 0;
    display: flex;
    background-color: #f9fafb;
    border-bottom: 1px solid #e5e7eb;
    position: sticky;
    top: 0;
    z-index: 10;
    min-width: min-content;
}

.table-header-cell {
    height: 48px;
    display: flex;
    align-items: center;
    background-color: #f9fafb;
    font-weight: 600;
    color: #374151;
    font-size: 13px;
    position: relative;
    user-select: none;
    box-sizing: border-box;
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
}

.table-row {
    display: flex;
    background-color: white;
    transition: background-color 0.2s;
    box-sizing: border-box;
    min-width: min-content;
}

.table-row:hover {
    background-color: #f9fafb;
}

.table-cell {
    height: 48px;
    display: flex;
    align-items: center;
    color: #4b5563;
    font-size: 13px;
    overflow: hidden;
    box-sizing: border-box;
    white-space: nowrap;
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
    flex-shrink: 0;
    min-height: 60px;
}

.pagination-info {
    font-size: 14px;
    color: #6b7280;
}

/* Statistics wrapper */
.statistics-wrapper {
    height: 40px;
    flex: 1;
    margin-left: 1rem;
    max-width: 50vw;
    min-width: 0;
}

.statistics-container {
    height: 100%;
    display: flex;
    align-items: center;
    min-width: min-content;
}

.statistic-placeholder {
    height: 100%;
    flex-shrink: 0;
}

.statistic-cell {
    height: 100%;
    flex-shrink: 0;
    display: flex;
    align-items: center;
}

.statistic-value {
    white-space: nowrap;
    width: 100%;
    height: 32px;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 4px;
    padding: 0 8px;
}

/* Resize handle */
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

/* Scrollbar styling */
.table-scroll-wrapper::-webkit-scrollbar {
    height: 8px;
}

.table-scroll-wrapper::-webkit-scrollbar-track {
    background: #f1f1f1;
    border-radius: 4px;
}

.table-scroll-wrapper::-webkit-scrollbar-thumb {
    background: #c1c1c1;
    border-radius: 4px;
}

.table-scroll-wrapper::-webkit-scrollbar-thumb:hover {
    background: #a8a8a8;
}

.table-body::-webkit-scrollbar {
    width: 8px;
}

.table-body::-webkit-scrollbar-track {
    background: #f1f1f1;
    border-radius: 4px;
}

.table-body::-webkit-scrollbar-thumb {
    background: #c1c1c1;
    border-radius: 4px;
}

.table-body::-webkit-scrollbar-thumb:hover {
    background: #a8a8a8;
}

/* Scrollbar cho statistics wrapper */
.statistics-wrapper::-webkit-scrollbar {
    height: 6px;
}

.statistics-wrapper::-webkit-scrollbar-track {
    background: #f1f1f1;
    border-radius: 4px;
}

.statistics-wrapper::-webkit-scrollbar-thumb {
    background: #c1c1c1;
    border-radius: 4px;
}

.statistics-wrapper::-webkit-scrollbar-thumb:hover {
    background: #a8a8a8;
}

/* Phân trang */
.pagination-btn {
    min-width: 32px;
    height: 32px;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 4px;
    background: white;
    cursor: pointer;
    padding: 0 4px;
    font-size: 14px;
    color: rgba(0, 0, 0, 0.88);
    transition: all 0.2s ease;
    font-weight: 400;
    border: 1px solid #d1d5db;
}

.pagination-btn:hover:not(:disabled) {
    border-color: #C0C0C0;
    color: #C0C0C0;
}

.pagination-btn:disabled {
    cursor: not-allowed;
    opacity: 0.5;
    background-color: rgba(0, 0, 0, 0.04);
}

/* Nút trang hiện tại */
.pagination-active {
    background: #808080;
    border-color: #808080;
    color: white;
    font-weight: 500;
}

.pagination-active:hover {
    background: #808080;
    border-color: #808080;
    color: white;
}

/* Nút điều hướng */
.pagination-nav {
    min-width: 28px;
    width: 28px;
    padding: 0;
    display: flex;
    align-items: center;
    justify-content: center;
}

@media (max-width: 1400px) {
    .table-header-cell,
    .table-cell {
        font-size: 12px;
    }

    .statistic-value {
        font-size: 13px;
        padding: 0 6px;
    }

    .statistics-wrapper {
        max-width: 40vw;
    }
}

@media (max-width: 1200px) {
    .statistics-wrapper {
        max-width: 35vw;
    }
}

/* Fix cho Firefox */
.table-header,
.table-row {
    min-width: -moz-min-content;
}
</style>