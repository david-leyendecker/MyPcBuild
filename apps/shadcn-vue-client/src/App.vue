<script setup lang="ts">
import { onMounted } from 'vue'
import { useThemeStore } from '@/stores/themeStore'
import DefaultLayout from '@/layouts/DefaultLayout.vue'

const themeStore = useThemeStore()

onMounted(() => {
  themeStore.initTheme()
})
</script>

<template>
  <div class="min-h-screen bg-background font-sans antialiased">
    <DefaultLayout />
  </div>
</template>

