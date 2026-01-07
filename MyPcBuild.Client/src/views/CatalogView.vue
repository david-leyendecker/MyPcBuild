<template>
  <div class="fadein animation-duration-300">
    <div class="mb-4">
      <div class="flex justify-content-between align-items-center mb-3">
        <h2 class="mt-0 mb-0 text-primary">Product Catalog</h2>
        <Button 
          label="Create Product"
          icon="pi pi-plus"
          @click="$router.push('/catalog/create')"
        />
      </div>
      <div class="flex gap-2">
        <InputText 
          v-model="catalogStore.searchQuery"
          placeholder="Search products..."
          @keyup.enter="handleSearch"
          class="flex-grow-1"
        />
        <Button 
          icon="pi pi-search"
          @click="handleSearch"
        />
      </div>
    </div>

    <div class="grid">
      <!-- Category Filter -->
      <div class="col-12 md:col-3 lg:col-2">
        <h3 class="mt-0 mb-3 text-sm font-medium">Categories</h3>
        <div class="flex flex-column gap-2">
          <Button 
            v-for="category in categories"
            :key="category"
            :label="category"
            @click="handleCategorySelect(category)"
            :outlined="catalogStore.selectedCategory !== category"
            size="small"
            class="justify-content-start"
          />
        </div>
      </div>

      <!-- Products Grid -->
      <div class="col-12 md:col-9 lg:col-10">
        <div v-if="catalogStore.isLoading" class="flex justify-content-center py-8">
          <ProgressSpinner />
        </div>

        <div v-else-if="catalogStore.error" class="mb-3">
          <Message severity="error" :text="catalogStore.error" />
        </div>

        <div v-else-if="catalogStore.products.length === 0" class="text-center py-8">
          <p class="text-xl p-text-secondary">No products found. Try a different search.</p>
        </div>

        <div v-else class="grid">
          <div 
            v-for="product in catalogStore.products"
            :key="product.id"
            class="col-12 sm:col-6 lg:col-4 xl:col-3"
          >
            <Card class="product-card h-full">
              <template #content>
                <h4 class="mt-0 mb-2">{{ product.name }}</h4>
                <p class="my-1 text-primary text-sm">{{ product.category }}</p>
                <p class="my-2 p-text-success font-semibold text-lg">${{ product.price.toFixed(2) }}</p>
                
                <div class="pt-3 mt-3 border-top-1 surface-border">
                  <div 
                    v-for="(value, key) in product.specifications"
                    :key="key"
                    class="flex justify-content-between text-xs text-500 mb-1"
                  >
                    <span class="font-medium">{{ key }}:</span>
                    <span>{{ value }}</span>
                  </div>
                </div>
              </template>
              <template #footer>
                <Button 
                  label="Add to Build"
                  icon="pi pi-plus"
                  @click="$emit('product-selected', product)"
                  class="w-full"
                  size="small"
                />
              </template>
            </Card>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useCatalogStore } from '@/stores/catalogStore';
import Button from 'primevue/button';
import Card from 'primevue/card';
import InputText from 'primevue/inputtext';
import Message from 'primevue/message';
import ProgressSpinner from 'primevue/progressspinner';

const catalogStore = useCatalogStore();
const categories = ref(['CPU', 'Motherboard', 'GPU', 'RAM', 'Storage', 'PSU', 'Case', 'Cooler']);

onMounted(() => {
  catalogStore.searchProducts();
});

function handleSearch() {
  catalogStore.searchProducts(catalogStore.searchQuery, catalogStore.selectedCategory || undefined);
}

async function handleCategorySelect(category: string) {
  if (catalogStore.selectedCategory === category) {
    catalogStore.selectedCategory = null;
    await catalogStore.searchProducts(catalogStore.searchQuery);
  } else {
    await catalogStore.getProductsByCategory(category);
  }
}
</script>

<style scoped>
.product-card {
  transition: all 0.3s ease;
}

.product-card:hover {
  border-color: var(--primary-color);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
  transform: translateY(-2px);
}
</style>
