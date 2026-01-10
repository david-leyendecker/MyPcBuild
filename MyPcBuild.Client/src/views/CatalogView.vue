<template>
  <div class="fade-in">
    <ViewHeader
      :title="PRODUCT_CATALOG.title"
      :action-button="{
        text: 'Create Product',
        icon: 'mdi-plus',
        onClick: () => $router.push('/catalog/create')
      }"
    />

    <v-row>
      <!-- Category Filter -->
      <v-col cols="12" md="3" lg="2">
        <h3 class="text-subtitle-1 font-weight-medium mb-3">Categories</h3>
        <div class="d-flex flex-column ga-2">
          <v-btn 
            :variant="catalogStore.selectedCategory === null ? 'elevated' : 'outlined'"
            size="small"
            class="justify-start"
            @click="handleCategorySelect(null)"
          >
            All Categories
          </v-btn>
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

      <!-- Products Data Table -->
      <v-col cols="12" md="9" lg="10">
        <v-card>
          <v-card-title>
            <v-text-field 
              v-model="searchText"
              placeholder="Search products by name or manufacturer..."
              prepend-inner-icon="mdi-magnify"
              variant="outlined"
              density="compact"
              hide-details
              clearable
              @update:model-value="handleSearchDebounced"
            ></v-text-field>
          </v-card-title>

          <v-data-table
            :headers="headers"
            :items="catalogStore.products"
            :items-length="catalogStore.totalProducts"
            :loading="catalogStore.isLoading"
            :items-per-page="catalogStore.itemsPerPage"
            :page="catalogStore.currentPage"
            :sort-by="[{ key: catalogStore.sortBy, order: catalogStore.sortDesc ? 'desc' : 'asc' }]"
            class="elevation-0"
            item-value="id"
            @update:options="handleTableOptionsUpdate"
          >
            <template #item.name="{ item }">
              <span class="font-weight-medium">{{ item.name }}</span>
            </template>

            <template #item.price="{ item }">
              <span class="text-success font-weight-semibold">${{ item.price.toFixed(2) }}</span>
            </template>

            <template #item.categoryName="{ item }">
              <v-chip size="small" color="primary" variant="tonal">
                {{ item.categoryName }}
              </v-chip>
            </template>

            <template #item.actions="{ item }">
              <v-btn 
                icon="mdi-plus"
                size="small"
                variant="text"
                color="primary"
                @click="$emit('product-selected', item)"
              >
                <v-icon>mdi-plus</v-icon>
                <v-tooltip activator="parent" location="top">
                  Add to Build
                </v-tooltip>
              </v-btn>
            </template>

            <template #no-data>
              <div class="text-center py-8">
                <v-icon size="64" color="grey-lighten-1">mdi-package-variant</v-icon>
                <p class="text-h6 text-medium-emphasis mt-4">No products found</p>
                <p class="text-body-2 text-medium-emphasis">Try adjusting your search or filters</p>
              </div>
            </template>

            <template #loading>
              <div class="text-center py-8">
                <v-progress-circular indeterminate color="primary"></v-progress-circular>
              </div>
            </template>
          </v-data-table>
        </v-card>

        <v-alert v-if="catalogStore.error" type="error" class="mt-3">
          {{ catalogStore.error }}
        </v-alert>
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
const searchText = ref('');

const headers = [
  { title: 'Name', key: 'name', sortable: true },
  { title: 'Category', key: 'categoryName', sortable: true },
  { title: 'Manufacturer', key: 'manufacturer', sortable: true },
  { title: 'Price', key: 'price', sortable: true },
  { title: 'Actions', key: 'actions', sortable: false, align: 'center' as const }
];

onMounted(() => {
  catalogStore.loadProducts();
});

let searchTimeout: ReturnType<typeof setTimeout> | null = null;

function handleSearchDebounced(value: string | null) {
  if (searchTimeout) {
    clearTimeout(searchTimeout);
  }
  searchTimeout = setTimeout(() => {
    catalogStore.setSearch(value || '');
  }, 300);
}

function handleCategorySelect(category: string | null) {
  catalogStore.setCategory(category);
}

interface TableOptions {
  page: number;
  itemsPerPage: number;
  sortBy?: Array<{ key: string; order: 'asc' | 'desc' }>;
}

function handleTableOptionsUpdate(options: TableOptions) {
  const needsUpdate = 
    options.page !== catalogStore.currentPage ||
    options.itemsPerPage !== catalogStore.itemsPerPage ||
    (options.sortBy && options.sortBy.length > 0 && options.sortBy[0] &&
      (options.sortBy[0].key !== catalogStore.sortBy || 
       (options.sortBy[0].order === 'desc') !== catalogStore.sortDesc));

  if (!needsUpdate) {
    return;
  }

  if (options.page !== catalogStore.currentPage) {
    catalogStore.setPage(options.page);
  } else if (options.itemsPerPage !== catalogStore.itemsPerPage) {
    catalogStore.setItemsPerPage(options.itemsPerPage);
  } else if (options.sortBy && options.sortBy.length > 0 && options.sortBy[0]) {
    catalogStore.setSorting(
      options.sortBy[0].key,
      options.sortBy[0].order === 'desc'
    );
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
</style>
