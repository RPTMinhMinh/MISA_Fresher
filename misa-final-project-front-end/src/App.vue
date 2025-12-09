<script setup>
import TheHeader from '@/layouts/TheHeader.vue';
import TheSideBar from '@/layouts/TheSideBar.vue';
import Asset from '@/views/asset/Asset.vue';
</script>

<template>
  <div class="app-layout">
    <!-- Layout chính -->
    <div class="flex h-screen">

      <!-- Sidebar -->
      <TheSideBar />

      <!-- Main content area bên phải -->
      <div class="flex flex-col flex-1 min-h-0 min-w-0">

        <!-- Header -->
        <TheHeader />

        <!-- Content hiển thị trực tiếp ở đây (không cần placeholder) -->
        <Asset />

      </div>

    </div>
  </div>
</template>

<style>
html,
body,
#app {
  height: 100%;
  margin: 0;
  padding: 0;
  overflow: hidden;
  font-size: 14px;
}

/* RESPONSIVE FONT SIZE */
@media (max-width: 1366px) {
  html, body {
    font-size: 13px;
  }
}

@media (max-width: 1024px) {
  html, body {
    font-size: 12px;
  }
}

@media (max-width: 768px) {
  html, body {
    font-size: 11px;
  }
}

/* Tối ưu cho màn hình 1366x768 */
@media (max-width: 1366px) {
  .app-layout {
    min-width: 1366px;
  }
  
  /* Sidebar tự động collapse trên màn hình nhỏ */
  .sidebar:not(.collapsed) {
    width: 64px !important;
  }
  
  .sidebar.collapsed + .flex-col {
    margin-left: 64px;
  }
}

/* Đảm bảo không có horizontal scroll không cần thiết */
.app-layout {
  overflow-x: hidden;
}
</style>