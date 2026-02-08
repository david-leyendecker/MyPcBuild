<script setup lang="ts">
import { AlertCircle } from 'lucide-vue-next'
import { Button } from '@/components/ui/button'

interface Props {
  message: string
  onRetry?: () => void
}

defineProps<Props>()
</script>

<template>
  <div class="flex flex-col items-center justify-center py-12">
    <div class="rounded-full bg-destructive/10 p-4 mb-4">
      <AlertCircle class="h-8 w-8 text-destructive" />
    </div>
    <h3 class="text-lg font-semibold mb-2">Something went wrong</h3>
    <p class="text-sm text-muted-foreground mb-6 max-w-md text-center">
      {{ message }}
    </p>
    <Button v-if="onRetry" @click="onRetry" variant="outline">
      Try Again
    </Button>
  </div>
</template>
