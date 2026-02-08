<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { catalogApi, getCategoryFromBackend, getCategoryDisplayName } from '@/api/catalog'
import type { Product } from '@/api/catalog'
import type { ProductResponse } from '@/types/product'
import LoadingState from '@/components/shared/LoadingState.vue'
import ErrorState from '@/components/shared/ErrorState.vue'
import CategoryIcon from '@/components/shared/CategoryIcon.vue'
import StatusBadge from '@/components/shared/StatusBadge.vue'
import PriceDisplay from '@/components/shared/PriceDisplay.vue'
import ProductViewer3D from '@/components/spatial/ProductViewer3D.vue'
import Button from '@/components/ui/button/Button.vue'
import Card from '@/components/ui/card/Card.vue'
import CardHeader from '@/components/ui/card/CardHeader.vue'
import CardTitle from '@/components/ui/card/CardTitle.vue'
import CardContent from '@/components/ui/card/CardContent.vue'
import Dialog from '@/components/ui/dialog/Dialog.vue'
import { Edit, Trash2, Upload, ArrowLeft } from 'lucide-vue-next'

const route = useRoute()
const router = useRouter()

const product = ref<Product | null>(null)
const productData = ref<ProductResponse | null>(null)
const isLoading = ref(false)
const error = ref<string | null>(null)
const showDeleteDialog = ref(false)

const category = computed(() => {
  if (!product.value) return null
  return getCategoryFromBackend(product.value.category)
})

const categoryDisplay = computed(() => {
  if (!product.value) return ''
  return getCategoryDisplayName(product.value.category)
})

const hasSpatialData = computed(() => {
  if (!productData.value) return false
  const p = productData.value as any
  return !!(p.dimensions || p.slots?.length || p.chambers?.length)
})

onMounted(async () => {
  await loadProduct()
})

async function loadProduct() {
  isLoading.value = true
  error.value = null
  try {
    const id = route.params.id as string
    const response = await catalogApi.getProduct(id)
    product.value = response
    productData.value = response as any
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Failed to load product'
  } finally {
    isLoading.value = false
  }
}

function handleEdit() {
  router.push({ name: 'product-edit', params: { id: product.value?.id } })
}

async function handlePublish() {
  if (!product.value) return
  try {
    await catalogApi.publishProduct(product.value.id)
    await loadProduct()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Failed to publish product'
  }
}

function handleDelete() {
  showDeleteDialog.value = true
}

function confirmDelete() {
  // TODO: Implement delete functionality when API is ready
  showDeleteDialog.value = false
  router.push({ name: 'catalog' })
}

function goBack() {
  router.push({ name: 'catalog' })
}
</script>

<template>
  <div class="container mx-auto p-6">
    <LoadingState v-if="isLoading" message="Loading product..." />

    <ErrorState
      v-else-if="error"
      :message="error"
      @retry="loadProduct"
    />

    <div v-else-if="product">
      <!-- Header -->
      <div class="mb-6">
        <Button variant="ghost" @click="goBack" class="mb-4">
          <ArrowLeft class="mr-2 h-4 w-4" />
          Back to Catalog
        </Button>

        <div class="flex items-start justify-between">
          <div class="flex items-start gap-4">
            <CategoryIcon v-if="category" :category="category" class="h-8 w-8 text-muted-foreground" />
            <div>
              <h1 class="text-3xl font-bold">{{ product.name }}</h1>
              <p class="text-muted-foreground">{{ product.manufacturer }}</p>
            </div>
          </div>

          <div class="flex gap-2">
            <Button v-if="product.isDraft" variant="outline" @click="handlePublish">
              <Upload class="mr-2 h-4 w-4" />
              Publish
            </Button>
            <Button variant="outline" @click="handleEdit">
              <Edit class="mr-2 h-4 w-4" />
              Edit
            </Button>
            <Button variant="destructive" @click="handleDelete">
              <Trash2 class="mr-2 h-4 w-4" />
              Delete
            </Button>
          </div>
        </div>
      </div>

      <!-- Product Information -->
      <div class="grid gap-6 md:grid-cols-2">
        <!-- Basic Information -->
        <Card>
          <CardHeader>
            <CardTitle>Basic Information</CardTitle>
          </CardHeader>
          <CardContent class="space-y-4">
            <div>
              <div class="text-sm font-medium text-muted-foreground">Category</div>
              <div class="flex items-center gap-2 mt-1">
                <CategoryIcon v-if="category" :category="category" class="h-4 w-4" />
                {{ categoryDisplay }}
              </div>
            </div>
            <div>
              <div class="text-sm font-medium text-muted-foreground">Price</div>
              <div class="mt-1">
                <PriceDisplay :amount="product.price" />
              </div>
            </div>
            <div>
              <div class="text-sm font-medium text-muted-foreground">Status</div>
              <div class="mt-1">
                <StatusBadge :is-draft="product.isDraft" />
              </div>
            </div>
            <div v-if="product.publishedAt">
              <div class="text-sm font-medium text-muted-foreground">Published At</div>
              <div class="mt-1">{{ new Date(product.publishedAt).toLocaleDateString() }}</div>
            </div>
          </CardContent>
        </Card>

        <!-- Specifications -->
        <Card>
          <CardHeader>
            <CardTitle>Specifications</CardTitle>
          </CardHeader>
          <CardContent class="space-y-3">
            <div
              v-for="[key, value] in Object.entries(product.specifications)"
              :key="key"
              class="flex justify-between"
            >
              <div class="text-sm font-medium text-muted-foreground capitalize">
                {{ key.replace(/([A-Z])/g, ' $1').trim() }}
              </div>
              <div class="text-sm">{{ value }}</div>
            </div>
          </CardContent>
        </Card>
      </div>

      <!-- 3D Visualization (if spatial data exists) -->
      <Card v-if="hasSpatialData && productData" class="mt-6">
        <CardHeader>
          <CardTitle>3D Preview</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="h-[500px]">
            <ProductViewer3D
              :dimensions="(productData as any).dimensions"
              :slots="(productData as any).slots"
              :chambers="(productData as any).chambers"
              :title="`${product.name} - 3D Preview`"
            />
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- Delete Confirmation Dialog -->
    <Dialog v-model:open="showDeleteDialog">
      <div class="p-6">
        <h2 class="text-lg font-semibold mb-4">Confirm Delete</h2>
        <p class="mb-6 text-muted-foreground">
          Are you sure you want to delete this product? This action cannot be undone.
        </p>
        <div class="flex justify-end gap-2">
          <Button variant="outline" @click="showDeleteDialog = false">
            Cancel
          </Button>
          <Button variant="destructive" @click="confirmDelete">
            Delete
          </Button>
        </div>
      </div>
    </Dialog>
  </div>
</template>
