<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import ThemeToggle from './ThemeToggle.vue'

const route = useRoute()

const breadcrumbs = computed(() => {
  const path = route.path
  const segments = path.split('/').filter(Boolean)
  
  const crumbs = [{ label: 'Home', path: '/' }]
  
  let currentPath = ''
  segments.forEach(segment => {
    currentPath += `/${segment}`
    const label = segment.charAt(0).toUpperCase() + segment.slice(1)
    crumbs.push({ label, path: currentPath })
  })
  
  return crumbs
})
</script>

<template>
  <header class="sticky top-0 z-50 w-full border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
    <div class="container flex h-14 items-center">
      <div class="mr-4 flex">
        <RouterLink to="/" class="mr-6 flex items-center space-x-2">
          <span class="font-bold">MyPcBuild</span>
        </RouterLink>
      </div>
      
      <div class="flex flex-1 items-center justify-center">
        <nav class="flex items-center space-x-2 text-sm text-muted-foreground">
          <template v-for="(crumb, index) in breadcrumbs" :key="crumb.path">
            <span v-if="index > 0" class="mx-1">/</span>
            <RouterLink 
              :to="crumb.path" 
              class="hover:text-foreground transition-colors"
              :class="{ 'text-foreground font-medium': crumb.path === route.path }"
            >
              {{ crumb.label }}
            </RouterLink>
          </template>
        </nav>
      </div>
      
      <div class="flex items-center justify-end">
        <ThemeToggle />
      </div>
    </div>
  </header>
</template>
