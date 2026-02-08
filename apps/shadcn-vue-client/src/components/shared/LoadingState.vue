<script setup lang="ts">
import { Loader2 } from 'lucide-vue-next'

interface Props {
  text?: string
}

withDefaults(defineProps<Props>(), {
  text: 'Loading...'
})
</script>

<template>
  <div class="flex flex-col items-center justify-center py-12">
    <Loader2 class="h-8 w-8 animate-spin text-muted-foreground mb-4" />
    <p class="text-sm text-muted-foreground">{{ text }}</p>
  </div>
</template>

<style scoped>
@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.animate-spin {
  animation: spin 1s linear infinite;
}
</style>
