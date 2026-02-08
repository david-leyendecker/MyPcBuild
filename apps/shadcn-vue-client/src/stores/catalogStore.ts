/**
 * Catalog store for managing product listings
 */

import { defineStore } from 'pinia';
import { ref } from 'vue';
import type { 
  ProductSummary, 
  GetProductsResponse 
} from '@/api/catalog';
import { catalogApi, getCategoryBackendValue } from '@/api/catalog';

export const useCatalogStore = defineStore('catalog', () => {
  const products = ref<ProductSummary[]>([]);
  const totalProducts = ref(0);
  const selectedCategory = ref<string | null>(null);
  const searchQuery = ref('');
  const currentPage = ref(1);
  const itemsPerPage = ref(10);
  const sortBy = ref('name');
  const sortDesc = ref(false);
  const isLoading = ref(false);
  const error = ref<string | null>(null);

  async function loadProducts() {
    isLoading.value = true;
    error.value = null;
    try {
      const filters: string[] = [];
      
      if (selectedCategory.value) {
        const filterValue = getCategoryBackendValue(selectedCategory.value);
        filters.push(`ProductCategory=${filterValue}`);
      }
      
      const response: GetProductsResponse = await catalogApi.getProducts({
        filters: filters.length > 0 ? filters.join(',') : undefined,
        search: searchQuery.value || undefined,
        page: currentPage.value,
        itemsPerPage: itemsPerPage.value,
        sortBy: sortBy.value,
        sortDesc: sortDesc.value
      });
      
      products.value = response.items;
      totalProducts.value = response.pagination.total;
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load products';
      products.value = [];
      totalProducts.value = 0;
    } finally {
      isLoading.value = false;
    }
  }

  function setCategory(category: string | null) {
    selectedCategory.value = category;
    currentPage.value = 1; // Reset to first page when filtering
    loadProducts();
  }

  function setSearch(query: string) {
    searchQuery.value = query;
    currentPage.value = 1; // Reset to first page when searching
    loadProducts();
  }

  function setPage(page: number) {
    currentPage.value = page;
    loadProducts();
  }

  function setItemsPerPage(count: number) {
    itemsPerPage.value = count;
    currentPage.value = 1; // Reset to first page when changing items per page
    loadProducts();
  }

  function setSorting(column: string, descending: boolean) {
    sortBy.value = column;
    sortDesc.value = descending;
    loadProducts();
  }

  function clearError() {
    error.value = null;
  }

  return {
    products,
    totalProducts,
    selectedCategory,
    searchQuery,
    currentPage,
    itemsPerPage,
    sortBy,
    sortDesc,
    isLoading,
    error,
    loadProducts,
    setCategory,
    setSearch,
    setPage,
    setItemsPerPage,
    setSorting,
    clearError
  };
});
