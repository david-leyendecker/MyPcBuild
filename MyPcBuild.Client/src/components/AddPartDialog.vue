<template>
  <div class="d-flex flex-column ga-3">
    <p class="text-body-2 text-medium-emphasis">Search and select a component to add to your build</p>

    <div class="d-flex flex-column ga-3">
      <v-text-field 
        v-model="searchQuery"
        placeholder="Search components..."
        @keyup.enter="handleSearch"
      ></v-text-field>
      <div class="d-flex flex-wrap ga-2">
        <v-btn 
          v-for="category in categories"
          :key="category"
          :variant="selectedCategory === category ? 'elevated' : 'outlined'"
          size="small"
          @click="selectCategory(category)"
        >
          {{ categoryDisplayMap[category] }}
        </v-btn>
      </div>
    </div>

    <div v-if="isLoading" class="d-flex justify-center py-4">
      <v-progress-circular indeterminate color="primary"></v-progress-circular>
    </div>

    <div v-else-if="filteredProducts.length === 0" class="text-center py-4">
      <p class="text-medium-emphasis">No components found</p>
    </div>

    <div 
      v-else 
      class="overflow-y-auto"
      style="max-height: 400px; border: 1px solid rgba(var(--v-border-color), var(--v-border-opacity)); border-radius: 4px;"
    >
      <div 
        v-for="product in filteredProducts"
        :key="product.id"
        class="pa-3 product-item"
        style="border-bottom: 1px solid rgba(var(--v-border-color), var(--v-border-opacity)); cursor: pointer;"
        @click="selectProduct(product.id)"
      >
        <div class="d-flex justify-space-between align-center">
          <div>
            <h4 class="text-subtitle-1 mb-1">{{ product.name }}</h4>
            <p class="text-success font-weight-semibold text-body-2">${{ product.price.toFixed(2) }}</p>
          </div>
          <v-icon>mdi-arrow-right</v-icon>
        </div>
      </div>
    </div>

    <div class="d-flex justify-end ga-2 pt-3" style="border-top: 1px solid rgba(var(--v-border-color), var(--v-border-opacity));">
      <v-btn 
        prepend-icon="mdi-close"
        variant="text"
        @click="$emit('close')"
      >
        Cancel
      </v-btn>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useCatalogStore } from '@/stores/catalogStore';
import { ProductCategory, categoryLabels, getCategoryFromBackend } from '@/api/catalog';

const emit = defineEmits<{
  'part-selected': [productId: string];
  'close': [];
}>();

const catalogStore = useCatalogStore();
const categories = computed(() => Object.values(ProductCategory));
const categoryDisplayMap = computed(() => categoryLabels);

const searchQuery = ref('');
const selectedCategory = ref<string | null>(null);
const isLoading = ref(false);

const filteredProducts = computed(() => {
  return catalogStore.products.filter(p => {
    const matchesSearch = p.name.toLowerCase().includes(searchQuery.value.toLowerCase());
    const productCategoryEnum = getCategoryFromBackend(p.categoryName);
    const matchesCategory = !selectedCategory.value || productCategoryEnum === selectedCategory.value;
    return matchesSearch && matchesCategory;
  });
});

onMounted(() => {
  catalogStore.loadProducts();
});

function handleSearch() {
  catalogStore.setSearch(searchQuery.value);
}

function selectCategory(category: string) {
  selectedCategory.value = selectedCategory.value === category ? null : category;
  catalogStore.setCategory(selectedCategory.value);
}

function selectProduct(productId: string) {
  emit('part-selected', productId);
}
</script>

<style scoped>
.product-item {
  transition: background-color 0.2s ease;
}

.product-item:hover {
  background-color: rgba(var(--v-theme-on-surface), 0.05);
}
</style>
