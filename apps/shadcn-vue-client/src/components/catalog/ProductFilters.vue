<script setup lang="ts">
import { ref, watch, onUnmounted, computed } from 'vue'
import { useCatalogStore } from '@/stores/catalogStore'
import { categoryLabels } from '@/api/catalog'
import type { ProductCategory } from '@/types/product'
import { FormItemText, FormItemRadioGroup } from '@/components/form-items'

const catalogStore = useCatalogStore()

const categories: ProductCategory[] = ['cpu', 'gpu', 'motherboard', 'ram', 'storage', 'powersupply', 'cooler', 'case']

const categoryOptions = [
  { value: 'all' as const, label: 'All' },
  ...categories.map(c => ({ value: c, label: categoryLabels[c] }))
]

const selectedCategory = computed({
  get: () => (catalogStore.selectedCategory as ProductCategory | null) ?? 'all',
  set: (v: ProductCategory | 'all') => catalogStore.setCategory(v === 'all' ? null : v)
})

const statusFilter = ref<'all' | 'draft' | 'published'>(catalogStore.statusFilter)
const searchQuery = ref(catalogStore.searchQuery)

const statusOptions = [
  { value: 'all' as const, label: 'All' },
  { value: 'published' as const, label: 'Published' },
  { value: 'draft' as const, label: 'Draft' }
]

let searchDebounceTimeout: ReturnType<typeof setTimeout> | null = null

watch(searchQuery, (newValue) => {
  if (searchDebounceTimeout) {
    clearTimeout(searchDebounceTimeout)
  }
  searchDebounceTimeout = setTimeout(() => {
    catalogStore.setSearch(newValue)
  }, 300)
})

onUnmounted(() => {
  if (searchDebounceTimeout) {
    clearTimeout(searchDebounceTimeout)
  }
})

watch(statusFilter, (newValue) => {
  catalogStore.setStatus(newValue)
})
</script>

<template>
  <div class="space-y-6 pr-4">
    <FormItemText
      label="Search"
      v-model="searchQuery"
      placeholder="Search products..."
    />

    <FormItemRadioGroup
      label="Category"
      v-model="selectedCategory"
      :options="categoryOptions"
    />

    <FormItemRadioGroup
      label="Status"
      v-model="statusFilter"
      :options="statusOptions"
    />
  </div>
</template>
