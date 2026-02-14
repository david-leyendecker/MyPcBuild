<script setup lang="ts">
import PriceDisplay from '@/components/shared/PriceDisplay.vue'
import CompatibilityBadge from '@/components/shared/CompatibilityBadge.vue'

interface Props {
  totalCost: number
  partCount: number
  hasErrors: boolean
  hasWarnings: boolean
}

defineProps<Props>()
</script>

<template>
  <div class="bg-muted/50 border rounded-lg p-4">
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
      <div>
        <p class="text-sm text-muted-foreground mb-1">Total Cost</p>
        <PriceDisplay :amount="totalCost" class="text-lg font-bold" />
      </div>
      <div>
        <p class="text-sm text-muted-foreground mb-1">Parts</p>
        <p class="text-lg font-bold">{{ partCount }}</p>
      </div>
      <div>
        <p class="text-sm text-muted-foreground mb-1">Compatibility</p>
        <CompatibilityBadge :has-errors="hasErrors" :has-warnings="hasWarnings" />
      </div>
    </div>
  </div>
</template>
