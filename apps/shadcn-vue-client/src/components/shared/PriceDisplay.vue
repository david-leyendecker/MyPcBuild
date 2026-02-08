<script setup lang="ts">
interface Props {
  amount: number
}

defineProps<Props>()

const formatPrice = (amount: number): string => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(amount)
}
</script>

<template>
  <span class="font-medium">{{ formatPrice(amount) }}</span>
</template>
