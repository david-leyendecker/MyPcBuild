<script setup lang="ts">
import { cn } from '@/lib/utils'

interface Props {
  isDraft: boolean
}

defineProps<Props>()
</script>

<template>
  <span
    :class="cn(
      'inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-semibold',
      isDraft
        ? 'bg-muted text-muted-foreground'
        : 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-100'
    )"
  >
    {{ isDraft ? 'Draft' : 'Published' }}
  </span>
</template>
