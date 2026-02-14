<script setup lang="ts">
import { PackageOpen } from 'lucide-vue-next'
import { Button } from '@/components/ui/button'

interface Props {
  title: string
  description?: string
  actionLabel?: string
  actionTo?: string
}

defineProps<Props>()
</script>

<template>
  <div class="flex flex-col items-center justify-center py-12 text-center">
    <div class="rounded-full bg-muted p-6 mb-4">
      <PackageOpen class="h-12 w-12 text-muted-foreground" />
    </div>
    <h3 class="text-lg font-semibold mb-2">{{ title }}</h3>
    <p v-if="description" class="text-sm text-muted-foreground mb-6 max-w-md">
      {{ description }}
    </p>
    <Button v-if="actionLabel && actionTo" as-child>
      <RouterLink :to="actionTo">
        {{ actionLabel }}
      </RouterLink>
    </Button>
  </div>
</template>
