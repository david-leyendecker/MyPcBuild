<template>
  <div class="flex flex-column gap-3">
    <p class="text-sm text-500 m-0 pb-2">Search and select a component to add to your build</p>

    <div class="flex flex-column gap-3">
      <InputText 
        v-model="searchQuery"
        placeholder="Search components..."
        @keyup.enter="handleSearch"
      />
      <div class="flex flex-wrap gap-2">
        <Button 
          v-for="category in categories"
          :key="category"
          :label="category"
          :outlined="selectedCategory !== category"
          @click="selectCategory(category)"
          size="small"
        />
      </div>
    </div>

    <div v-if="isLoading" class="flex justify-content-center py-4">
      <ProgressSpinner />
    </div>

    <div v-else-if="filteredProducts.length === 0" class="text-center py-4">
      <p class="text-500 m-0">No components found</p>
    </div>

    <div 
      v-else 
      class="border-1 surface-border border-round overflow-y-auto"
      style="max-height: 400px;"
    >
      <div 
        v-for="product in filteredProducts"
        :key="product.id"
        class="p-3 border-bottom-1 surface-border cursor-pointer hover:surface-hover transition-colors transition-duration-200"
        @click="selectProduct(product.id)"
      >
        <div class="flex justify-content-between align-items-center">
          <div>
            <h4 class="mt-0 mb-1 text-base">{{ product.name }}</h4>
            <p class="my-0 text-green-500 font-semibold text-sm">${{ product.price.toFixed(2) }}</p>
          </div>
          <i class="pi pi-arrow-right text-500"></i>
        </div>
      </div>
    </div>

    <div class="flex justify-content-end gap-2 pt-3 border-top-1 surface-border">
      <Button 
        label="Cancel"
        icon="pi pi-times"
        @click="$emit('close')"
        text
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useCatalogStore } from '@/stores/catalogStore';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';
import ProgressSpinner from 'primevue/progressspinner';

const emit = defineEmits<{
  'part-selected': [productId: string];
  'close': [];
}>();

const catalogStore = useCatalogStore();
const categories = ['CPU', 'Motherboard', 'GPU', 'RAM', 'Storage', 'PSU', 'Case', 'Cooler'];

const searchQuery = ref('');
const selectedCategory = ref<string | null>(null);
const isLoading = ref(false);

const filteredProducts = computed(() => {
  return catalogStore.products.filter(p => {
    const matchesSearch = p.name.toLowerCase().includes(searchQuery.value.toLowerCase());
    const matchesCategory = !selectedCategory.value || p.category === selectedCategory.value;
    return matchesSearch && matchesCategory;
  });
});

onMounted(() => {
  catalogStore.searchProducts();
});

function handleSearch() {
  catalogStore.searchProducts(searchQuery.value, selectedCategory.value || undefined);
}

function selectCategory(category: string) {
  selectedCategory.value = selectedCategory.value === category ? null : category;
  handleSearch();
}

function selectProduct(productId: string) {
  emit('part-selected', productId);
}
</script>

<style scoped>
.hover\:surface-hover:hover {
  background-color: var(--surface-hover);
}

.cursor-pointer {
  cursor: pointer;
}
</style>
