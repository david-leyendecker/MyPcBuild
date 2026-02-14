<script setup lang="ts">
interface Props {
  amount: number
}

defineProps<Props>()

const currencyFormatter = new Intl.NumberFormat('en-US', {
  style: 'currency',
  currency: 'USD',
  minimumFractionDigits: 2,
  maximumFractionDigits: 2,
})

const formatPrice = (amount: number): string => currencyFormatter.format(amount)
</script>

<template>
  <span class="font-medium">{{ formatPrice(amount) }}</span>
</template>
