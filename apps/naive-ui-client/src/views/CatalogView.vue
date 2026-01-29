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

    <!-- Category Filter -->
    <n-flex vertical style="margin-bottom: 16px;">
      <n-text strong style="font-size: 14px; margin-bottom: 12px;">Categories</n-text>
      <n-flex :size="8" wrap>
        <n-tag 
          :type="catalogStore.selectedCategory === '' || catalogStore.selectedCategory === null ? 'primary' : 'default'"
          :bordered="false"
          style="cursor: pointer;"
          @click="handleCategorySelect('')"
        >
          All Categories
        </n-tag>
        <n-tag 
          v-for="category in categories"
          :key="category"
          :type="catalogStore.selectedCategory === category ? 'primary' : 'default'"
          :bordered="false"
          style="cursor: pointer;"
          @click="handleCategorySelect(category)"
        >
          {{ categoryDisplayNames[category] }}
        </n-tag>
      </n-flex>
    </n-flex>

    <!-- Products Data Table -->
    <n-card>
      <template #header>
        <n-input 
          v-model:value="searchText"
          placeholder="Search products by name or manufacturer..."
          clearable
          @update:value="handleSearchDebounced"
        >
          <template #prefix>
            <n-icon :component="Icons.SearchIcon" />
          </template>
        </n-input>
      </template>

      <n-empty 
        v-if="!catalogStore.isLoading && catalogStore.products.length === 0"
        description="No products found"
      >
        <template #extra>
          <n-button type="primary" @click="() => $router.push('/catalog/create')">
            <template #icon>
              <n-icon :component="Icons.Add" />
            </template>
            Create Product
          </n-button>
        </template>
      </n-empty>

      <n-data-table
        v-else
        :columns="columns"
        :data="catalogStore.products"
        :loading="catalogStore.isLoading"
        :pagination="paginationReactive"
        :bordered="false"
        @update:page="handlePageChange"
        @update:page-size="handlePageSizeChange"
      />

      <n-alert v-if="catalogStore.error" type="error" style="margin-top: 12px;">
        {{ catalogStore.error }}
      </n-alert>
    </n-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, h } from 'vue';
import { useRouter } from 'vue-router';
import { NCard, NInput, NDataTable, NTag, NFlex, NAlert, NIcon, NButton, NEmpty, NH3, NText } from 'naive-ui';
import type { DataTableColumns, PaginationProps } from 'naive-ui';
import { useCatalogStore } from '@/stores/catalogStore';
import { catalogApi, ProductCategory, categoryLabels } from '@/api/catalog';
import ViewHeader from '@/components/ViewHeader.vue';
import { PRODUCT_CATALOG } from '@/config/navigation';
import { Icons } from '@/utils/icons';

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

const columns: DataTableColumns<any> = [
  {
    title: 'Name',
    key: 'name',
    sorter: 'default',
    render: (row) => h(
      'span',
      {
        style: 'cursor: pointer; font-weight: 500; color: var(--n-text-color);',
        onClick: () => viewProduct(row.id)
      },
      row.name
    )
  },
  {
    title: 'Category',
    key: 'categoryName',
    sorter: 'default',
    render: (row) => h(NTag, { type: 'primary', bordered: false, size: 'small' }, { default: () => row.categoryName })
  },
  {
    title: 'Manufacturer',
    key: 'manufacturer',
    sorter: 'default'
  },
  {
    title: 'Price',
    key: 'price',
    sorter: 'default',
    render: (row) => h('span', { style: 'font-weight: 600; color: #18a058;' }, `$${row.price.toFixed(2)}`)
  },
  {
    title: 'Status',
    key: 'isDraft',
    sorter: 'default',
    render: (row) => row.isDraft 
      ? h(NTag, { type: 'warning', bordered: false, size: 'small' }, { 
          default: () => [
            h(NIcon, { component: Icons.Pencil, style: 'margin-right: 4px;' }),
            'Draft'
          ]
        })
      : h(NTag, { type: 'success', bordered: false, size: 'small' }, { 
          default: () => [
            h(NIcon, { component: Icons.Check, style: 'margin-right: 4px;' }),
            'Published'
          ]
        })
  },
  {
    title: 'Actions',
    key: 'actions',
    width: 150,
    render: (row) => h(
      NFlex,
      { justify: 'end', size: 8 },
      {
        default: () => [
          row.isDraft ? h(
            NButton,
            {
              text: true,
              title: 'Publish product',
              onClick: () => publish(row.id)
            },
            { icon: () => h(NIcon, { component: Icons.Check }) }
          ) : null,
          h(
            NButton,
            {
              text: true,
              title: 'Edit',
              onClick: () => edit(row.id)
            },
            { icon: () => h(NIcon, { component: Icons.Edit }) }
          ),
          h(
            NButton,
            {
              text: true,
              type: 'error',
              title: 'Delete',
              onClick: () => remove(row.id)
            },
            { icon: () => h(NIcon, { component: Icons.Trash }) }
          )
        ]
      }
    )
  }
];

const paginationReactive = computed<PaginationProps>(() => ({
  page: catalogStore.currentPage,
  pageSize: catalogStore.itemsPerPage,
  showSizePicker: true,
  pageSizes: [10, 20, 30, 50],
  itemCount: catalogStore.totalProducts,
  prefix: ({ itemCount }) => `Total: ${itemCount}`
}));

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

function handlePageChange(page: number) {
  catalogStore.setPage(page);
}

function handlePageSizeChange(pageSize: number) {
  catalogStore.setItemsPerPage(pageSize);
}

async function publish(id: string) {
  try {
    await catalogApi.publishProduct(id);
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
