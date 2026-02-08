<script setup lang="ts">
import { onMounted } from 'vue'
import { useThemeStore } from '@/stores/themeStore'
import DefaultLayout from '@/layouts/DefaultLayout.vue'
import Toast from '@/components/ui/toast/Toast.vue'

const themeStore = useThemeStore()

onMounted(() => {
  themeStore.initTheme()
})
</script>

<template>
  <div class="min-h-screen bg-background font-sans antialiased">
    <DefaultLayout />
    <Toast />
  </div>
</template>

