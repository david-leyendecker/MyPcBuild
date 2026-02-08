<script setup lang="ts">
import { computed } from 'vue'
import { Sun, Moon, Monitor } from 'lucide-vue-next'
import { Button } from '@/components/ui/button'
import { useThemeStore, type ThemeMode } from '@/stores/themeStore'

const themeStore = useThemeStore()

const icon = computed(() => {
  switch (themeStore.mode) {
    case 'light':
      return Sun
    case 'dark':
      return Moon
    case 'system':
      return Monitor
  }
})

const cycleTheme = () => {
  const modes: ThemeMode[] = ['light', 'dark', 'system']
  const currentIndex = modes.indexOf(themeStore.mode)
  const nextIndex = (currentIndex + 1) % modes.length
  themeStore.setMode(modes[nextIndex]!)
}
</script>

<template>
  <Button variant="ghost" size="icon" @click="cycleTheme" title="Toggle theme">
    <component :is="icon" class="h-5 w-5" />
  </Button>
</template>
