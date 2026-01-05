import { defineStore } from 'pinia';
import { ref } from 'vue';
import { type Product } from '@/api/catalog';
import { catalogApi } from '@/api/catalog';

export const useCatalogStore = defineStore('catalog', () => {
  const products = ref<Product[]>([]);
  const selectedCategory = ref<string | null>(null);
  const searchQuery = ref('');
  const isLoading = ref(false);
  const error = ref<string | null>(null);

  async function searchProducts(query: string = '', category?: string) {
    isLoading.value = true;
    error.value = null;
    try {
      products.value = await catalogApi.searchProducts({
        search: query || undefined,
        category: category || undefined,
        limit: 50
      });
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to search products';
    } finally {
      isLoading.value = false;
    }
  }

  async function getProductsByCategory(category: string) {
    isLoading.value = true;
    error.value = null;
    selectedCategory.value = category;
    try {
      products.value = await catalogApi.getProductsByCategory(category);
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load products';
    } finally {
      isLoading.value = false;
    }
  }

  function setSearchQuery(query: string) {
    searchQuery.value = query;
  }

  function clearError() {
    error.value = null;
  }

  return {
    products,
    selectedCategory,
    searchQuery,
    isLoading,
    error,
    searchProducts,
    getProductsByCategory,
    setSearchQuery,
    clearError
  };
});
