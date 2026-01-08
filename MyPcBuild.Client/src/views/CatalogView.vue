<template>
  <div class="fade-in">
    <ViewHeader
      :title="PRODUCT_CATALOG.title"
      :action-button="{
        text: 'Create Product',
        icon: 'mdi-plus',
        onClick: () => $router.push('/catalog/create')
      }"
    >
      <div class="d-flex ga-2">
        <v-text-field 
          v-model="catalogStore.searchQuery"
          placeholder="Search products..."
          @keyup.enter="handleSearch"
        ></v-text-field>
        <v-btn 
          icon="mdi-magnify"
          color="primary"
          @click="handleSearch"
        ></v-btn>
      </div>
    </ViewHeader>

    <v-row>
      <!-- Category Filter -->
      <v-col cols="12" md="3" lg="2">
        <h3 class="text-subtitle-1 font-weight-medium mb-3">Categories</h3>
        <div class="d-flex flex-column ga-2">
          <v-btn 
            v-for="category in categories"
            :key="category"
            :variant="catalogStore.selectedCategory === category ? 'elevated' : 'outlined'"
            size="small"
            class="justify-start"
            @click="handleCategorySelect(category)"
          >
            {{ category }}
          </v-btn>
        </div>
      </v-col>

      <!-- Products Grid -->
      <v-col cols="12" md="9" lg="10">
        <div v-if="catalogStore.isLoading" class="d-flex justify-center py-8">
          <v-progress-circular indeterminate color="primary"></v-progress-circular>
        </div>

        <v-alert v-else-if="catalogStore.error" type="error" class="mb-3">
          {{ catalogStore.error }}
        </v-alert>

        <div v-else-if="catalogStore.products.length === 0" class="text-center py-8">
          <p class="text-h6 text-medium-emphasis">No products found. Try a different search.</p>
        </div>

        <v-row v-else>
          <v-col 
            v-for="product in catalogStore.products"
            :key="product.id"
            cols="12" sm="6" lg="4" xl="3"
          >
            <v-card class="product-card h-100">
              <v-card-text>
                <h4 class="text-h6 mb-2">{{ product.name }}</h4>
                <p class="text-primary text-body-2 my-1">{{ product.category }}</p>
                <p class="text-success font-weight-semibold text-h6 my-2">${{ product.price.toFixed(2) }}</p>
                
                <v-divider class="my-3"></v-divider>
                
                <div 
                  v-for="(value, key) in product.specifications"
                  :key="key"
                  class="d-flex justify-space-between text-caption text-medium-emphasis mb-1"
                >
                  <span class="font-weight-medium">{{ key }}:</span>
                  <span>{{ value }}</span>
                </div>
              </v-card-text>
              <v-card-actions>
                <v-btn 
                  prepend-icon="mdi-plus"
                  color="primary"
                  size="small"
                  block
                  @click="$emit('product-selected', product)"
                >
                  Add to Build
                </v-btn>
              </v-card-actions>
            </v-card>
          </v-col>
        </v-row>
      </v-col>
    </v-row>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useCatalogStore } from '@/stores/catalogStore';
import ViewHeader from '@/components/ViewHeader.vue';
import { PRODUCT_CATALOG } from '@/config/navigation';

const catalogStore = useCatalogStore();
const categories = ref(['CPU', 'Motherboard', 'GPU', 'RAM', 'Storage', 'PSU', 'PCCase', 'Cooler']);

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
.fade-in {
  animation: fadeIn 0.3s ease-in;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

.product-card {
  transition: all 0.3s ease;
}

.product-card:hover {
  border-color: rgb(var(--v-theme-primary));
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
  transform: translateY(-2px);
}
</style>
