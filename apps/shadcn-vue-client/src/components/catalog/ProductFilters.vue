<script setup lang="ts">
import { ref, watch, onUnmounted } from 'vue'
import { useCatalogStore } from '@/stores/catalogStore'
import { categoryLabels } from '@/api/catalog'
import type { ProductCategory } from '@/types/product'
import Input from '@/components/ui/input/Input.vue'
import Label from '@/components/ui/label/Label.vue'

const catalogStore = useCatalogStore()

const categories: ProductCategory[] = ['cpu', 'gpu', 'motherboard', 'ram', 'storage', 'powersupply', 'cooler', 'case']

const selectedCategories = ref<Set<ProductCategory>>(new Set())
const statusFilter = ref<'all' | 'draft' | 'published'>(catalogStore.statusFilter)
const searchQuery = ref(catalogStore.searchQuery)

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

watch(selectedCategories, () => {
  if (selectedCategories.value.size === 0) {
    catalogStore.setCategory(null)
  } else if (selectedCategories.value.size === 1) {
    const category = Array.from(selectedCategories.value)[0]
    catalogStore.setCategory(category || null)
  }
}, { deep: true })

watch(statusFilter, (newValue) => {
  catalogStore.setStatus(newValue)
})

function toggleCategory(category: ProductCategory) {
  if (selectedCategories.value.has(category)) {
    selectedCategories.value.delete(category)
  } else {
    selectedCategories.value.clear()
    selectedCategories.value.add(category)
  }
}
</script>

<template>
  <div class="space-y-6 pr-4">
    <div>
      <h3 class="mb-3 text-sm font-medium">Search</h3>
      <Input
        v-model="searchQuery"
        type="text"
        placeholder="Search products..."
        class="w-full"
      />
    </div>

    <div>
      <h3 class="mb-3 text-sm font-medium">Category</h3>
      <div class="space-y-2">
        <div
          v-for="category in categories"
          :key="category"
          class="flex items-center space-x-2"
        >
          <input
            :id="`category-${category}`"
            type="checkbox"
            :checked="selectedCategories.has(category)"
            class="h-4 w-4 rounded border-gray-300"
            @change="toggleCategory(category)"
          />
          <Label
            :for="`category-${category}`"
            class="text-sm font-normal cursor-pointer"
          >
            {{ categoryLabels[category] }}
          </Label>
        </div>
      </div>
    </div>

    <div>
      <h3 class="mb-3 text-sm font-medium">Status</h3>
      <div class="space-y-2">
        <div class="flex items-center space-x-2">
          <input
            id="status-all"
            v-model="statusFilter"
            type="radio"
            value="all"
            class="h-4 w-4"
          />
          <Label for="status-all" class="text-sm font-normal cursor-pointer">
            All
          </Label>
        </div>
        <div class="flex items-center space-x-2">
          <input
            id="status-published"
            v-model="statusFilter"
            type="radio"
            value="published"
            class="h-4 w-4"
          />
          <Label for="status-published" class="text-sm font-normal cursor-pointer">
            Published
          </Label>
        </div>
        <div class="flex items-center space-x-2">
          <input
            id="status-draft"
            v-model="statusFilter"
            type="radio"
            value="draft"
            class="h-4 w-4"
          />
          <Label for="status-draft" class="text-sm font-normal cursor-pointer">
            Draft
          </Label>
        </div>
      </div>
    </div>
  </div>
</template>
