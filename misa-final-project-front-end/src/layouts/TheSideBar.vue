<template>
    <aside class="sidebar transition-all duration-300 ease-in-out" 
           :class="[isCollapsed ? 'w-[64px]' : 'w-64']">
        
        <div class="bg-gray-800 text-white h-full flex flex-col flex-shrink-0">
            
            <div class="flex-1 overflow-y-auto overflow-x-hidden" 
                 :class="[isCollapsed ? 'p-2' : 'p-6']">
                
                <div class="header-sidebar flex items-center mb-8" 
                     :class="{'justify-center': isCollapsed}">
                    
                    <div class="logo bg-blue-500 w-12 h-12 rounded-lg flex items-center justify-center flex-shrink-0 transition-all"
                         :class="[isCollapsed ? 'mb-2' : 'mr-4']">
                        <span class="text-xl font-bold">M</span>
                    </div>
                    
                    <div class="title whitespace-nowrap overflow-hidden transition-all" v-show="!isCollapsed">
                        <div class="text-2xl font-bold tracking-wide">MISA QLTS</div>
                    </div>
                </div>

                <nav>
                    <ul class="space-y-1">
                        <li v-for="(item, index) in menuItems" :key="index" class="menu-item"
                            :class="{ 'menu-item-active': selectedIndex === index }">
                            
                            <div class="menu-item-content flex items-center p-3 rounded cursor-pointer transition-all"
                                :class="[isCollapsed ? 'justify-center' : 'justify-between']"
                                @click="selectMenuItem(index)">
                                
                                <div class="flex items-center">
                                    <div class="w-6 h-6 flex items-center justify-center flex-shrink-0"
                                         :class="[isCollapsed ? '' : 'mr-3']">
                                        <span v-if="selectedIndex === index" :class="item.iconActive"></span>
                                        <span v-else :class="item.iconInactive"></span>
                                    </div>
                                    
                                    <span v-show="!isCollapsed" class="whitespace-nowrap">{{ item.name }}</span>
                                </div>
                                
                                <div v-if="item.hasArrow && !isCollapsed" class="icon-arrow-down"></div>
                            </div>
                        </li>
                    </ul>
                </nav>
            </div>

            <div class="p-4 border-t border-gray-700 flex-shrink-0">
                <div class="flex items-center cursor-pointer h-10 rounded hover:bg-gray-700 transition-colors"
                     :class="[isCollapsed ? 'justify-center' : 'justify-end px-2']"
                     @click="toggleSidebar">
                     
                    
                    <div :class="isCollapsed ? 'icon-expand' : 'icon-collapse'"></div>
                </div>
            </div>

        </div>
    </aside>
</template>

<script setup>
import { ref } from 'vue'
import { useSidebar } from '@/composables/useSideBar'
// Sử dụng state từ Composable
const { isCollapsed, toggleSidebar } = useSidebar()

const menuItems = ref([
    {
        name: 'Tổng quan',
        hasArrow: false,
        iconInactive: 'icon-dashboard-inactive',
        iconActive: 'icon-dashboard-active'
    },
    {
        name: 'Tài sản',
        hasArrow: true,
        iconInactive: 'icon-asset-inactive',
        iconActive: 'icon-asset-active'
    },
    {
        name: 'Tài sản HT-DB',
        hasArrow: true,
        iconInactive: 'icon-fixed-asset-inactive',
        iconActive: 'icon-fixed-asset-active'
    },
    {
        name: 'Công cụ dụng cụ',
        hasArrow: true,
        iconInactive: 'icon-tools-inactive',
        iconActive: 'icon-tools-active'
    },
    {
        name: 'Danh mục',
        hasArrow: false,
        iconInactive: 'icon-category-inactive',
        iconActive: 'icon-category-active'
    },
    {
        name: 'Tra cứu',
        hasArrow: true,
        iconInactive: 'icon-search-inactive',
        iconActive: 'icon-search-active'
    },
    {
        name: 'Báo cáo',
        hasArrow: true,
        iconInactive: 'icon-report-inactive',
        iconActive: 'icon-report-active'
    }
])

const selectedIndex = ref(0)

// Chọn một menu item
const selectMenuItem = (index) => {
    selectedIndex.value = index
    console.log(`Đã chọn: ${menuItems.value[index].name}`)
}
</script>

<style scoped>
.sidebar {
    height: 100vh;
    box-shadow: 2px 0 5px rgba(0, 0, 0, 0.1);
}

/* Custom CSS cho hover và active (Giữ nguyên code cũ) */
.menu-item-content {
    transition: all 0.2s ease;
    color: #d1d5db;
}

/* Hover effect */
.menu-item:hover .menu-item-content {
    background-color: #20A5C8;
    color: white;
}

/* Active effect */
.menu-item-active .menu-item-content {
    background-color: #20A5C8;
    color: white;
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
}

/* Click effect */
.menu-item-content:active {
    transform: translateY(1px);
}
</style>