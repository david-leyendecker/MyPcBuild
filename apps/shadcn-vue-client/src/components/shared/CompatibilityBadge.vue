<script setup lang="ts">
import { computed } from 'vue'
import { AlertCircle, AlertTriangle, CheckCircle } from 'lucide-vue-next'
import { cn } from '@/lib/utils'

interface Props {
  hasErrors: boolean
  hasWarnings: boolean
}

const props = defineProps<Props>()

const status = computed(() => {
  if (props.hasErrors) {
    return {
      icon: AlertCircle,
      class: 'text-destructive',
      label: 'Has errors',
    }
  }
  if (props.hasWarnings) {
    return {
      icon: AlertTriangle,
      class: 'text-yellow-500',
      label: 'Has warnings',
    }
  }
  return {
    icon: CheckCircle,
    class: 'text-green-500',
    label: 'Valid',
  }
})
</script>

<template>
  <div class="inline-flex items-center gap-1.5" :title="status.label">
    <component :is="status.icon" :class="cn('h-4 w-4', status.class)" />
    <span :class="cn('text-sm font-medium', status.class)">
      {{ status.label }}
    </span>
  </div>
</template>
