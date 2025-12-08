import { createApp } from 'vue'
import App from './App.vue'

import '@/assets/styles/tailwind.css'

import Antd from 'ant-design-vue'
import 'ant-design-vue/dist/reset.css'

import '@/assets/styles/main.css'
import '@/assets/fonts/fonts.css'
import '@/assets/logos/icons.css'
import * as services from './services';

import Vue3Toastify from 'vue3-toastify'
import 'vue3-toastify/dist/index.css'

const app = createApp(App)

// Cấu hình vue3-toastify
app.use(Vue3Toastify, {
  autoClose: 3000,
  position: 'top-right',
  theme: 'light',
  transition: 'slide',
  hideProgressBar: true,
  closeOnClick: true,
  pauseOnHover: true,
  draggable: true,
  draggablePercent: 0.6,
})

app.use(Antd)
app.mount('#app')