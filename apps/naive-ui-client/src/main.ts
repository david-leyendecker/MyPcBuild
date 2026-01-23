import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import { router } from './router'
import { useThemeStore } from './stores/themeStore'

const app = createApp(App)

app.use(createPinia())
app.use(router)

// Initialize theme after pinia is installed
const themeStore = useThemeStore()
themeStore.initTheme()

app.mount('#app')
