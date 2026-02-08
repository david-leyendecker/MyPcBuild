<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import AppHeader from '@/components/layout/AppHeader.vue'
import AppSidebar from '@/components/layout/AppSidebar.vue'

const isMobile = ref(false)
const isMobileMenuOpen = ref(false)

const checkMobile = () => {
  isMobile.value = window.innerWidth < 768
  if (!isMobile.value) {
    isMobileMenuOpen.value = false
  }
}

onMounted(() => {
  checkMobile()
  window.addEventListener('resize', checkMobile)
})

onUnmounted(() => {
  window.removeEventListener('resize', checkMobile)
})

const toggleMobileMenu = () => {
  isMobileMenuOpen.value = !isMobileMenuOpen.value
}
</script>

<template>
  <div class="min-h-screen bg-background">
    <AppHeader />
    
    <div class="flex h-[calc(100vh-3.5rem)]">
      <!-- Desktop Sidebar -->
      <div v-if="!isMobile" class="flex-shrink-0">
        <AppSidebar />
      </div>
      
      <!-- Mobile Drawer Overlay -->
      <Transition name="fade">
        <div
          v-if="isMobile && isMobileMenuOpen"
          class="fixed inset-0 z-40 bg-black/50 md:hidden"
          @click="toggleMobileMenu"
        />
      </Transition>
      
      <!-- Mobile Drawer -->
      <Transition name="slide">
        <div
          v-if="isMobile && isMobileMenuOpen"
          class="fixed left-0 top-14 bottom-0 z-50 md:hidden"
        >
          <AppSidebar />
        </div>
      </Transition>
      
      <!-- Main Content -->
      <main class="flex-1 overflow-auto">
        <div class="container py-6">
          <RouterView />
        </div>
      </main>
    </div>
    
    <!-- Mobile Menu Button -->
    <button
      v-if="isMobile"
      @click="toggleMobileMenu"
      class="fixed bottom-4 right-4 z-30 rounded-full bg-primary p-3 text-primary-foreground shadow-lg md:hidden"
    >
      <svg
        xmlns="http://www.w3.org/2000/svg"
        width="24"
        height="24"
        viewBox="0 0 24 24"
        fill="none"
        stroke="currentColor"
        stroke-width="2"
        stroke-linecap="round"
        stroke-linejoin="round"
      >
        <line x1="4" x2="20" y1="12" y2="12" />
        <line x1="4" x2="20" y1="6" y2="6" />
        <line x1="4" x2="20" y1="18" y2="18" />
      </svg>
    </button>
  </div>
</template>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

.slide-enter-active,
.slide-leave-active {
  transition: transform 0.3s ease;
}

.slide-enter-from,
.slide-leave-to {
  transform: translateX(-100%);
}
</style>
