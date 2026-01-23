<template>
  <n-flex vertical :size="12">
    <p style="font-size: 14px; opacity: 0.7;">Search and select a component to add to your build</p>

    <n-flex vertical :size="12">
      <n-input 
        v-model:value="searchQuery"
        placeholder="Search components..."
        @keyup.enter="handleSearch"
      />
      <n-flex wrap :size="8">
        <n-button 
          v-for="category in categories"
          :key="category"
          :type="selectedCategory === category ? 'primary' : 'default'"
          size="small"
          @click="selectCategory(category)"
        >
          {{ categoryDisplayMap[category] }}
        </n-button>
      </n-flex>
    </n-flex>

    <n-flex v-if="isLoading" justify="center" style="padding: 16px 0;">
      <n-spin />
    </n-flex>

    <n-empty v-else-if="filteredProducts.length === 0" description="No components found">
      <template #extra>
        <n-button @click="searchQuery = ''; selectedCategory = null">
          Clear Filters
        </n-button>
      </template>
    </n-empty>

    <n-scrollbar 
      v-else 
      style="max-height: 400px;"
    >
      <n-flex vertical :size="0">
        <div 
          v-for="product in filteredProducts"
          :key="product.id"
          class="product-item"
          style="padding: 12px; border-bottom: 1px solid rgba(255, 255, 255, 0.09); cursor: pointer;"
          @click="selectProduct(product.id)"
        >
          <n-flex justify="space-between" align="center">
            <div>
              <h4 style="font-size: 16px; margin-bottom: 4px;">{{ product.name }}</h4>
              <p style="color: #18a058; font-weight: 600; font-size: 14px;">${{ product.price.toFixed(2) }}</p>
            </div>
            <n-icon :component="Icons.ArrowForward" />
          </n-flex>
        </div>
      </n-flex>
    </n-scrollbar>

    <n-flex justify="end" :size="8" style="padding-top: 12px; border-top: 1px solid rgba(255, 255, 255, 0.09);">
      <n-button @click="$emit('close')">
        Cancel
      </n-button>
    </n-flex>
  </n-flex>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { NInput, NButton, NFlex, NSpin, NScrollbar, NEmpty, NIcon } from 'naive-ui';
import { useCatalogStore } from '@/stores/catalogStore';
import { ProductCategory, categoryLabels, getCategoryFromBackend } from '@/api/catalog';
import { Icons } from '@/utils/icons';

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
  background-color: rgba(255, 255, 255, 0.05);
}
</style>
