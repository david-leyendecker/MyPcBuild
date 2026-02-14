<script setup lang="ts">
import { watch, onUnmounted } from 'vue'

interface Props {
  open: boolean
  title?: string
}

interface Emits {
  (e: 'update:open', value: boolean): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

function close() {
  emit('update:open', false)
}

function handleKeydown(e: KeyboardEvent) {
  if (e.key === 'Escape') {
    close()
  }
}

watch(() => props.open, (isOpen) => {
  if (isOpen) {
    document.addEventListener('keydown', handleKeydown)
  } else {
    document.removeEventListener('keydown', handleKeydown)
  }
})

onUnmounted(() => {
  document.removeEventListener('keydown', handleKeydown)
})
</script>

<template>
  <Teleport to="body">
    <Transition
      enter-active-class="transition-opacity duration-200"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition-opacity duration-150"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div
        v-if="open"
        class="fixed inset-0 z-50 bg-background/80 backdrop-blur-sm"
        @click="close"
      />
    </Transition>
    
    <Transition
      enter-active-class="transition-all duration-200"
      enter-from-class="opacity-0 scale-95"
      enter-to-class="opacity-100 scale-100"
      leave-active-class="transition-all duration-150"
      leave-from-class="opacity-100 scale-100"
      leave-to-class="opacity-0 scale-95"
    >
      <div
        v-if="open"
        role="dialog"
        aria-modal="true"
        :aria-label="title"
        class="fixed left-1/2 top-1/2 z-50 w-full max-w-lg -translate-x-1/2 -translate-y-1/2 p-6"
        @click.stop
      >
        <div class="bg-background border rounded-lg shadow-lg p-6">
          <div v-if="title" class="mb-4">
            <h2 class="text-lg font-semibold">{{ title }}</h2>
          </div>
          <slot />
        </div>
      </div>
    </Transition>
  </Teleport>
</template>
