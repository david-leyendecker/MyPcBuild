<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { Home, Cpu, Package, ChevronLeft, ChevronRight } from 'lucide-vue-next'
import { Button } from '@/components/ui/button'
import { cn } from '@/lib/utils'

interface NavItem {
  label: string
  path: string
  icon: any
}

const route = useRoute()
const isCollapsed = ref(false)

const navItems: NavItem[] = [
  { label: 'Dashboard', path: '/', icon: Home },
  { label: 'Builds', path: '/builds', icon: Cpu },
  { label: 'Catalog', path: '/catalog', icon: Package },
]

onMounted(() => {
  const stored = localStorage.getItem('sidebar-collapsed')
  if (stored !== null) {
    isCollapsed.value = stored === 'true'
  }
})

const toggleCollapsed = () => {
  isCollapsed.value = !isCollapsed.value
  localStorage.setItem('sidebar-collapsed', String(isCollapsed.value))
}

const isActive = (path: string) => {
  if (path === '/') {
    return route.path === '/'
  }
  return route.path.startsWith(path)
}
</script>

<template>
  <aside 
    :class="cn(
      'h-full border-r bg-background transition-all duration-300',
      isCollapsed ? 'w-16' : 'w-64'
    )"
  >
    <div class="flex h-full flex-col">
      <nav class="flex-1 space-y-1 p-2">
        <RouterLink
          v-for="item in navItems"
          :key="item.path"
          :to="item.path"
          :class="cn(
            'flex items-center gap-3 rounded-lg px-3 py-2 text-sm transition-colors',
            isActive(item.path)
              ? 'bg-secondary text-secondary-foreground font-medium'
              : 'text-muted-foreground hover:bg-secondary/50 hover:text-foreground'
          )"
        >
          <component :is="item.icon" class="h-5 w-5 flex-shrink-0" />
          <span v-if="!isCollapsed">{{ item.label }}</span>
        </RouterLink>
      </nav>
      
      <div class="border-t p-2">
        <Button
          variant="ghost"
          size="sm"
          @click="toggleCollapsed"
          :class="cn('w-full', isCollapsed ? 'justify-center' : 'justify-start')"
        >
          <component :is="isCollapsed ? ChevronRight : ChevronLeft" class="h-4 w-4" />
          <span v-if="!isCollapsed" class="ml-2">Collapse</span>
        </Button>
      </div>
    </div>
  </aside>
</template>
