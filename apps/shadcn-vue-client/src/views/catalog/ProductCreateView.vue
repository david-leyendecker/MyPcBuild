<script setup lang="ts">
import { ref, computed, markRaw } from 'vue'
import { useRouter } from 'vue-router'
import type { ProductCategory, CategoryFormData, CategoryFormComponentRef } from '@/types/product'
import { categoryLabels, catalogApi } from '@/api/catalog'
import type { CreateProductRequest } from '@/api/catalog'
import { useProductSpatialData } from '@/composables/useProductSpatialData'
import CategoryIcon from '@/components/shared/CategoryIcon.vue'
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

const step = ref<1 | 2>(1)
const selectedCategory = ref<ProductCategory | null>(null)
const categoryFormData = ref<CategoryFormData>({})
const isSubmitting = ref(false)
const error = ref<string | null>(null)

const categoryFormRef = ref<CategoryFormComponentRef | null>(null)

const categories: ProductCategory[] = ['cpu', 'gpu', 'motherboard', 'ram', 'storage', 'powersupply', 'cooler', 'case']

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

const { hasSpatialData, dimensions, slots, chambers } = useProductSpatialData(categoryFormData)

function selectCategory(category: ProductCategory) {
  selectedCategory.value = category
  step.value = 2
}

function goBack() {
  if (step.value === 2) {
    step.value = 1
    selectedCategory.value = null
    categoryFormData.value = {}
    error.value = null
  } else {
    router.push({ name: 'catalog' })
  }
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

    const response = await catalogApi.createProduct(request)
    
    if (!commonData.isDraft) {
      await catalogApi.publishProduct(response.id)
    }

    router.push({ name: 'catalog' })
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Failed to create product'
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <div class="container mx-auto p-6">
    <div class="mb-6">
      <Button variant="ghost" @click="goBack" class="mb-4">
        <ArrowLeft class="mr-2 h-4 w-4" />
        {{ step === 1 ? 'Back to Catalog' : 'Back to Category Selection' }}
      </Button>

      <h1 class="text-3xl font-bold">Create New Product</h1>
      <p class="text-muted-foreground">{{ step === 1 ? 'Select a product category' : `Create a new ${categoryLabels[selectedCategory!]}` }}</p>
    </div>

    <!-- Step 1: Category Selection -->
    <div v-if="step === 1" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
      <Card
        v-for="category in categories"
        :key="category"
        class="cursor-pointer transition-all hover:shadow-lg hover:border-primary"
        @click="selectCategory(category)"
      >
        <CardHeader>
          <div class="flex items-center justify-center mb-4">
            <CategoryIcon :category="category" class="h-12 w-12 text-primary" />
          </div>
          <CardTitle class="text-center">{{ categoryLabels[category] }}</CardTitle>
        </CardHeader>
      </Card>
    </div>

    <!-- Step 2: Product Form -->
    <div v-else-if="step === 2 && selectedCategory" class="max-w-4xl">
      <Card>
        <CardHeader>
          <CardTitle>Product Information</CardTitle>
        </CardHeader>
        <CardContent>
          <ProductFormShell
            :category="selectedCategory"
            :is-submitting="isSubmitting"
            @submit="handleSubmit"
          >
            <component
              :is="currentFormComponent"
              v-if="currentFormComponent"
              ref="categoryFormRef"
              v-model="categoryFormData"
            />
          </ProductFormShell>

          <div v-if="error" class="mt-4 p-3 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-md">
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
            <ProductViewer3D
              :dimensions="dimensions"
              :slots="slots"
              :chambers="chambers"
              title="Product 3D Preview"
            />
          </div>
        </CardContent>
      </Card>
    </div>
  </div>
</template>
