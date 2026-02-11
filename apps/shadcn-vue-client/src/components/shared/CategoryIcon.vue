<script setup lang="ts">
import { computed, type Component } from 'vue'
import { Cpu, Gpu, CircuitBoard, MemoryStick, HardDrive, Zap, Fan, Box } from 'lucide-vue-next'
import type { ProductCategory } from '@/types/product'

interface Props {
  category: ProductCategory
  class?: string
}

const props = defineProps<Props>()

const iconMap: Record<ProductCategory, Component> = {
  cpu: Cpu,
  gpu: Gpu,
  motherboard: CircuitBoard,
  ram: MemoryStick,
  storage: HardDrive,
  powersupply: Zap,
  cooler: Fan,
  case: Box,
}

const iconComponent = computed(() => iconMap[props.category])
</script>

<template>
  <component :is="iconComponent" :class="props.class" />
</template>
