<script setup lang="ts">
import { Loader2 } from 'lucide-vue-next'

interface Props {
  message?: string
}

withDefaults(defineProps<Props>(), {
  message: 'Loading...'
})
</script>

<template>
  <div class="flex flex-col items-center justify-center py-12">
    <Loader2 class="h-8 w-8 animate-spin text-muted-foreground mb-4" />
    <p class="text-sm text-muted-foreground">{{ message }}</p>
  </div>
</template>
