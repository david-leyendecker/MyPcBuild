<script setup lang="ts">
import { ref, computed, markRaw, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import type { ProductCategory } from '@/types/product'
import { categoryLabels, catalogApi } from '@/api/catalog'
import type { CreateProductRequest } from '@/api/catalog'
import LoadingState from '@/components/shared/LoadingState.vue'
import ErrorState from '@/components/shared/ErrorState.vue'
import Card from '@/components/ui/card/Card.vue'
import CardHeader from '@/components/ui/card/CardHeader.vue'
import CardTitle from '@/components/ui/card/CardTitle.vue'
import CardContent from '@/components/ui/card/CardContent.vue'
import Button from '@/components/ui/button/Button.vue'
import ProductFormShell from '@/components/products/ProductFormShell.vue'
import ProductViewer3D from '@/components/spatial/ProductViewer3D.vue'
import CpuForm from '@/components/products/CpuForm.vue'
import GpuForm from '@/components/products/GpuForm.vue'
import MotherboardForm from '@/components/products/MotherboardForm.vue'
import RamForm from '@/components/products/RamForm.vue'
import StorageForm from '@/components/products/StorageForm.vue'
import PsuForm from '@/components/products/PsuForm.vue'
import CoolerForm from '@/components/products/CoolerForm.vue'
import PcCaseForm from '@/components/products/PcCaseForm.vue'
import { ArrowLeft } from 'lucide-vue-next'

const router = useRouter()
const route = useRoute()

const productId = route.params.id as string
const isLoading = ref(true)
const selectedCategory = ref<ProductCategory | null>(null)
const productName = ref('')
const productManufacturer = ref('')
const productPrice = ref(0)
const categoryFormData = ref<any>({})
const isSubmitting = ref(false)
const error = ref<string | null>(null)

const categoryFormRef = ref<any>(null)

const categoryFormComponents = {
  cpu: markRaw(CpuForm),
  gpu: markRaw(GpuForm),
  motherboard: markRaw(MotherboardForm),
  ram: markRaw(RamForm),
  storage: markRaw(StorageForm),
  powersupply: markRaw(PsuForm),
  cooler: markRaw(CoolerForm),
  case: markRaw(PcCaseForm)
}

const currentFormComponent = computed(() => {
  if (!selectedCategory.value) return null
  return categoryFormComponents[selectedCategory.value]
})

const hasSpatialData = computed(() => {
  const data = categoryFormData.value as any
  if (!data) return false
  const hasDimensions = data.dimensions && (data.dimensions.length || data.dimensions.width || data.dimensions.height)
  const hasSlots = data.slots && data.slots.length > 0
  const hasChambers = data.chambers && data.chambers.length > 0
  return !!(hasDimensions || hasSlots || hasChambers)
})

onMounted(async () => {
  await loadProduct()
})

async function loadProduct() {
  try {
    const product = await catalogApi.getProduct(productId)
    // product.category is already in frontend format (ProductCategory)
    selectedCategory.value = product.category as ProductCategory
    productName.value = product.name
    productManufacturer.value = product.manufacturer
    productPrice.value = product.price
    categoryFormData.value = product.specifications || {}
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Failed to load product'
  } finally {
    isLoading.value = false
  }
}

function goBack() {
  router.push({ name: 'product-detail', params: { id: productId } })
}

async function handleSubmit(commonData: { name: string; manufacturer: string; price: number; isDraft: boolean }) {
  if (!selectedCategory.value || !categoryFormRef.value) return

  isSubmitting.value = true
  error.value = null

  try {
    const categoryData = categoryFormRef.value.getFormData()

    const fields: Record<string, string> = {}
    Object.entries(categoryData).forEach(([key, value]) => {
      if (typeof value === 'object' && value !== null) {
        fields[key] = JSON.stringify(value)
      } else {
        fields[key] = String(value)
      }
    })

    const request: CreateProductRequest = {
      category: selectedCategory.value,
      name: commonData.name,
      price: commonData.price,
      manufacturer: commonData.manufacturer,
      fields
    }

    await catalogApi.updateProduct(productId, request)

    if (!commonData.isDraft) {
      await catalogApi.publishProduct(productId)
    }

    router.push({ name: 'product-detail', params: { id: productId } })
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Failed to update product'
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <div class="container mx-auto p-6">
    <LoadingState v-if="isLoading" message="Loading product..." />

    <ErrorState v-else-if="error" :message="error" @retry="loadProduct" />

    <div v-else>
      <div class="mb-6">
        <Button variant="ghost" @click="goBack" class="mb-4">
          <ArrowLeft class="mr-2 h-4 w-4" />
          Back to Product
        </Button>

        <h1 class="text-3xl font-bold">Edit Product</h1>
        <p class="text-muted-foreground" v-if="selectedCategory">Editing {{ categoryLabels[selectedCategory] }}</p>
      </div>

      <!-- Product Form -->

      <Card>
        <CardHeader>
          <CardTitle>Product Information</CardTitle>
        </CardHeader>
        <CardContent>
          <ProductFormShell v-if="selectedCategory" :category="selectedCategory" :is-submitting="isSubmitting"
            :initial-name="productName" :initial-manufacturer="productManufacturer" :initial-price="productPrice"
            @submit="handleSubmit">
            <component :is="currentFormComponent" v-if="currentFormComponent" ref="categoryFormRef"
              v-model="categoryFormData" />
          </ProductFormShell>

          <div v-if="error"
            class="mt-4 p-3 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-md">
            <p class="text-sm text-red-800 dark:text-red-200">{{ error }}</p>
          </div>
        </CardContent>
      </Card>

      <!-- 3D Preview (when spatial data exists) -->
      <Card v-if="hasSpatialData" class="mt-6">
        <CardHeader>
          <CardTitle>3D Preview</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="h-[500px]">
            <ProductViewer3D :dimensions="(categoryFormData as any).dimensions" :slots="(categoryFormData as any).slots"
              :chambers="(categoryFormData as any).chambers" title="Product 3D Preview" />
          </div>
        </CardContent>
      </Card>
    </div>
  </div>
</template>
