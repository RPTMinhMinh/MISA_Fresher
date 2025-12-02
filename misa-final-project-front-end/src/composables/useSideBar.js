// src/composables/useSidebar.js
import { ref } from 'vue'

// Khai báo state ở ngoài hàm để trở thành Global State (trạng thái dùng chung)
const isCollapsed = ref(false)

export function useSidebar() {
  const toggleSidebar = () => {
    isCollapsed.value = !isCollapsed.value
  }

  return {
    isCollapsed,
    toggleSidebar,
  }
}
