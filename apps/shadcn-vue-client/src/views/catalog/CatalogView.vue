<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useCatalogStore } from '@/stores/catalogStore'
import ProductFilters from '@/components/catalog/ProductFilters.vue'
import ProductCard from '@/components/catalog/ProductCard.vue'
import ProductTable from '@/components/catalog/ProductTable.vue'
import LoadingState from '@/components/shared/LoadingState.vue'
import ErrorState from '@/components/shared/ErrorState.vue'
import EmptyState from '@/components/shared/EmptyState.vue'
import Button from '@/components/ui/button/Button.vue'
import { LayoutGrid, List, Plus } from 'lucide-vue-next'

const catalogStore = useCatalogStore()
const router = useRouter()

const viewMode = ref<'grid' | 'table'>('grid')
const sortBy = ref('name')

onMounted(() => {
  catalogStore.loadProducts()
})

function handleSort(value: string) {
  sortBy.value = value
  const descending = value === 'price'
  catalogStore.setSorting(value, descending)
}

function handlePageChange(page: number) {
  catalogStore.setPage(page)
}

function createNewProduct() {
  router.push({ name: 'product-create' })
}
</script>

<template>
  <div class="container mx-auto p-6">
    <div class="mb-6 flex items-center justify-between">
      <div>
        <h1 class="text-3xl font-bold">Product Catalog</h1>
        <p class="text-muted-foreground">Browse and manage products</p>
      </div>
      <Button @click="createNewProduct">
        <Plus class="mr-2 h-4 w-4" />
        New Product
      </Button>
    </div>

    <div class="flex gap-6">
      <!-- Filters Sidebar -->
      <aside class="w-64 flex-shrink-0">
        <div class="sticky top-6">
          <ProductFilters />
        </div>
      </aside>

      <!-- Main Content -->
      <main class="flex-1">
        <!-- Toolbar -->
        <div class="mb-4 flex items-center justify-between">
          <div class="flex items-center gap-2">
            <label for="sort" class="text-sm font-medium">Sort by:</label>
            <select
              id="sort"
              v-model="sortBy"
              class="rounded-md border border-input bg-background px-3 py-2 text-sm"
              @change="handleSort(sortBy)"
            >
              <option value="name">Name</option>
              <option value="price">Price</option>
              <option value="category">Category</option>
            </select>
          </div>

          <div class="flex items-center gap-2">
            <Button
              variant="outline"
              size="icon"
              :class="{ 'bg-muted': viewMode === 'grid' }"
              @click="viewMode = 'grid'"
            >
              <LayoutGrid class="h-4 w-4" />
            </Button>
            <Button
              variant="outline"
              size="icon"
              :class="{ 'bg-muted': viewMode === 'table' }"
              @click="viewMode = 'table'"
            >
              <List class="h-4 w-4" />
            </Button>
          </div>
        </div>

        <!-- Loading State -->
        <LoadingState v-if="catalogStore.isLoading" message="Loading products..." />

        <!-- Error State -->
        <ErrorState
          v-else-if="catalogStore.error"
          :message="catalogStore.error"
          @retry="catalogStore.loadProducts()"
        />

        <!-- Empty State -->
        <EmptyState
          v-else-if="catalogStore.products.length === 0"
          title="No products found"
          message="Try adjusting your filters or create a new product"
        />

        <!-- Grid View -->
        <div
          v-else-if="viewMode === 'grid'"
          class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4"
        >
          <ProductCard
            v-for="product in catalogStore.products"
            :key="product.id"
            :product="product"
          />
        </div>

        <!-- Table View -->
        <ProductTable
          v-else
          :products="catalogStore.products"
        />

        <!-- Pagination -->
        <div v-if="catalogStore.products.length > 0" class="mt-6 flex items-center justify-center gap-2">
          <Button
            variant="outline"
            :disabled="catalogStore.currentPage === 1"
            @click="handlePageChange(catalogStore.currentPage - 1)"
          >
            Previous
          </Button>
          <span class="text-sm text-muted-foreground">
            Page {{ catalogStore.currentPage }} of {{ Math.ceil(catalogStore.totalProducts / catalogStore.itemsPerPage) }}
          </span>
          <Button
            variant="outline"
            :disabled="catalogStore.currentPage >= Math.ceil(catalogStore.totalProducts / catalogStore.itemsPerPage)"
            @click="handlePageChange(catalogStore.currentPage + 1)"
          >
            Next
          </Button>
        </div>
      </main>
    </div>
  </div>
</template>
