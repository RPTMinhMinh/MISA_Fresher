<script setup>
import TheHeader from '@/layouts/TheHeader.vue';
import TheSideBar from '@/layouts/TheSideBar.vue';
import TheContent from '@/views/asset/TheContent.vue';
</script>

<template>
  <div class="app-layout">
    <!-- Layout chính -->
    <div class="flex h-screen">

      <!-- Sidebar -->
      <TheSideBar />

      <!-- Main content area bên phải -->
      <div class="flex flex-col flex-1 min-h-0">

        <!-- Header -->
        <TheHeader />

        <!-- Content hiển thị trực tiếp ở đây (không cần placeholder) -->
        <TheContent />

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
}
</style>