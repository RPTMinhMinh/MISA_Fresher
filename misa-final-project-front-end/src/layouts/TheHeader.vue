<template>
    <header class="header">
        <!-- Header theo thiết kế ảnh -->
        <div class="bg-white border-b border-gray-200 px-6 py-2">
            <div class="flex items-center justify-between">
                <!-- Phần bên trái: Tiêu đề và breadcrumb -->
                <div>
                    <div class="flex items-center space-x-4">
                        <div>
                            <div class="text-lg font-bold text-gray-800">Danh sách tài sản</div>

                        </div>
                    </div>
                </div>

                <!-- Phần bên phải: Các nút chức năng -->
                <div class="flex flex-row">
                    <div class="flex items-center space-x-4">
                        <!-- Ô "Sổ tài chính" -->
                        <div class="flex items-center rounded-sm px-4 py-2">
                            <span class="text-black font-medium mr-2">Sổ tài chính</span>
                        </div>
                    </div>

                    <!-- Ô "Năm" -->
                    <div class="input-color flex items-center bg-gray-100 rounded-lg px-4 py-2">
                        <span class="text-black me-2">Năm</span>
                        <span class="text-gray-700 font-medium mr-2">2021</span>
                        <div class="icon-up-and-down">
                        </div>
                    </div>

                    <!-- Nút hình tròn (có thể là nút filter hoặc view) -->
                    <div class="flex items-center me-3 ms-8">
                        <div class="icon-notification"></div>
                    </div>
                    <div class="flex items-center me-3 ms-3">
                        <div class="icon-feature"></div>
                    </div>
                    <div class="flex items-center me-3 ms-3">
                        <div class="icon-question"></div>
                    </div>
                    <div class="flex items-center me-3 ms-3">
                        <div class="icon-user"></div>
                    </div>
                    <div class="flex items-center me-3">
                        <div class="icon-small-arrow-down"></div>
                    </div>
                    <!-- <div class="w-10 h-10 flex items-center justify-center 
                                bg-blue-50 text-blue-600 rounded-full hover:bg-blue-100 
                                transition-colors mx-3">
                        <span class="header-icon-filter"></span>
                    </div>
                    <div class="w-10 h-10 flex items-center justify-center 
                                bg-blue-50 text-blue-600 rounded-full hover:bg-blue-100 
                                transition-colors mx-3">
                        <span class="header-icon-filter"></span> -->
                </div>
            </div>
        </div>
    </header>
</template>

<script setup>
// Component logic nếu cần
</script>

<style scoped>
.input-color {
    background-color: #CFEAF1;
}
</style>