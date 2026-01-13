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
      <v-col cols="12">
        <h3 class="text-subtitle-1 font-weight-medium mb-3">Categories</h3>
        <v-chip-group
          :model-value="catalogStore.selectedCategory"
          @update:model-value="handleCategorySelect"
        >
          <v-chip 
            value=""
            :color="catalogStore.selectedCategory === '' || catalogStore.selectedCategory === null ? 'primary' : undefined"
          >
            All Categories
          </v-chip>
          <v-chip 
            v-for="category in categories"
            :key="category"
            :value="category"
            :color="catalogStore.selectedCategory === category ? 'primary' : undefined"
          >
            {{ categoryDisplayNames[category] }}
          </v-chip>
        </v-chip-group>
      </v-col>

      <!-- Products Data Table -->
      <v-col cols="12">
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
              <span 
                class="font-weight-medium cursor-pointer text-primary" 
                @click="viewProduct(item.id)"
              >
                {{ item.name }}
              </span>
            </template>

            <template #item.isDraft="{ item }">
              <v-chip 
                v-if="item.isDraft" 
                size="small" 
                color="warning" 
                variant="tonal"
              >
                <v-icon start icon="mdi-pencil"></v-icon>
                Draft
              </v-chip>
              <v-chip 
                v-else 
                size="small" 
                color="success" 
                variant="tonal"
              >
                <v-icon start icon="mdi-check-circle"></v-icon>
                Published
              </v-chip>
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
              <div class="d-flex ga-2 justify-end">
                <v-tooltip v-if="item.isDraft" text="Publish product">
                  <template #activator="{ props }">
                    <v-icon 
                      v-bind="props"
                      color="success" 
                      icon="mdi-check-circle" 
                      size="small" 
                      @click="publish(item.id)"
                    ></v-icon>
                  </template>
                </v-tooltip>
                <v-icon color="medium-emphasis" icon="mdi-pencil" size="small" @click="edit(item.id)"></v-icon>
                <v-icon color="medium-emphasis" icon="mdi-delete" size="small" @click="remove(item.id)"></v-icon>
              </div>
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
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useCatalogStore } from '@/stores/catalogStore';
import { catalogApi, ProductCategory, categoryLabels } from '@/api/catalog';
import ViewHeader from '@/components/ViewHeader.vue';
import { PRODUCT_CATALOG } from '@/config/navigation';

const router = useRouter();

const catalogStore = useCatalogStore();
const categories = computed(() => Object.values(ProductCategory));
const categoryDisplayNames = computed(() => 
  Object.entries(categoryLabels).reduce((acc, [key, label]) => {
    acc[key] = label;
    return acc;
  }, {} as Record<string, string>)
);
const searchText = ref('');

const headers = [
  { title: 'Name', key: 'name', sortable: true },
  { title: 'Category', key: 'categoryName', sortable: true },
  { title: 'Manufacturer', key: 'manufacturer', sortable: true },
  { title: 'Price', key: 'price', sortable: true },
  { title: 'Status', key: 'isDraft', sortable: true },
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
  catalogStore.setCategory(category === '' ? null : category);
}

async function publish(id: string) {
  try {
    await catalogApi.publishProduct(id);
    // Reload products to show updated status
    await catalogStore.loadProducts();
  } catch (error) {
    console.error('Failed to publish product:', error);
  }
}

function viewProduct(id: string) {
  router.push(`/catalog/product/${id}`);
}

function edit(id: string) {
  router.push(`/catalog/product/${id}`);
}

function remove(id: string) {
  // TODO: Implement remove functionality
  console.log('Remove product:', id);
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

.cursor-pointer {
  cursor: pointer;
}
</style>
