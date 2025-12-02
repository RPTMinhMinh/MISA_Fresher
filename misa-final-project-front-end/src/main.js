import { createApp } from 'vue'
import App from './App.vue'

import '@/assets/styles/tailwind.css'

import Antd from 'ant-design-vue'
import 'ant-design-vue/dist/reset.css'

import '@/assets/styles/main.css'
import '@/assets/fonts/fonts.css'
import '@/assets/logos/icons.css'

const app = createApp(App)

app.use(Antd)
app.mount('#app')
