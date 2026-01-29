<template>
  <div class="fade-in">
    <n-flex justify="space-between" align="center">
      <n-h2>Create New Product</n-h2>
      <n-button text @click="$router.push('/catalog')">
        ← Back to Catalog
      </n-button>
    </n-flex>

    <n-card>
      <!-- Step Indicator -->
      <n-steps :current="currentStep" style="margin-bottom: 24px;">
        <n-step title="Creation Mode" description="Choose how to create" />
        <n-step :title="creationMode === 'ai' ? 'AI Generation' : 'Basic Info'"
          :description="creationMode === 'ai' ? 'Generate with AI' : 'Enter details'" />
        <n-step :title="creationMode === 'ai' ? 'Review' : 'Product Details'"
          :description="creationMode === 'ai' ? 'Review and edit' : 'Category-specific'" />
      </n-steps>

      <n-flex vertical>
        <!-- Step 1: Creation Mode Selection -->
        <n-flex vertical v-if="currentStep === 1">
          <n-h3 style="margin: 0 0 12px 0;">How would you like to create this product?</n-h3>

          <n-grid :cols="'1 s:1 m:2'">
            <n-form-item-gi>
              <n-card :bordered="true" style="cursor: pointer; padding: 16px; border: 2px solid transparent;"
                :style="creationMode === 'manual' ? { borderColor: 'var(--n-primary-color)' } : {}"
                @click="selectCreationMode('manual')">
                <div style="text-align: center;">
                  <div style="font-size: 48px; margin-bottom: 8px;">
                    <n-icon :component="Icons.Pencil" :size="48" />
                  </div>
                  <h4 class="text-h6" style="margin-bottom: 8px;">Manual Entry</h4>
                  <p class="text-body-2">Enter all product details manually</p>
                </div>
              </n-card>
            </n-form-item-gi>

            <n-form-item-gi>
              <n-card :bordered="true" style="cursor: pointer; padding: 16px; border: 2px solid transparent;"
                :style="creationMode === 'ai' ? { borderColor: 'var(--n-primary-color)' } : {}"
                @click="selectCreationMode('ai')">
                <div style="text-align: center;">
                  <div style="font-size: 48px; margin-bottom: 8px;">
                    <n-icon :component="Icons.Bulb" :size="48" />
                  </div>
                  <h4 class="text-h6" style="margin-bottom: 8px;">AI-Assisted</h4>
                  <p class="text-body-2">Generate product details from a description using AI</p>
                </div>
              </n-card>
            </n-form-item-gi>
          </n-grid>

          <n-flex justify="end" align="center">
            <n-button type="primary" :disabled="!creationMode" @click="nextStep" icon-placement="right">
              <template #icon>
                <n-icon :component="Icons.ArrowForward" />
              </template>
              Continue
            </n-button>
          </n-flex>
        </n-flex>

        <!-- Step 2: AI Generation or Basic Information -->
        <n-flex vertical v-else-if="currentStep === 2">
          <!-- AI Mode -->
          <n-flex vertical v-if="creationMode === 'ai'">
            <n-h3 style="margin: 0 0 12px 0;">Generate Product with AI</n-h3>

            <n-form :model="formData" label-placement="top">
              <n-grid :cols="'1 s:1 m:2'" :x-gap="16" :y-gap="12">
                <n-form-item-gi label="Category *" path="category" :span="2">
                  <n-select v-model:value="formData.category" :options="categories"></n-select>
                </n-form-item-gi>
                <n-form-item-gi label="AI Description *" path="description" :span="2">
                  <n-input v-model:value="aiDescription" type="textarea"
                    placeholder="e.g., High-performance AMD Ryzen processor with 16 cores, 32 threads, 5.7 GHz boost clock"
                    :rows="4"></n-input>
                </n-form-item-gi>
              </n-grid>
            </n-form>

            <n-alert v-if="error" type="error" style="margin-top: 12px;">
              {{ error }}
            </n-alert>

            <n-flex justify="space-between" align="center">
              <n-button text @click="currentStep = 1">
                <template #icon>
                  <n-icon :component="Icons.ArrowBack" />
                </template>
                Back
              </n-button>
              <n-button type="primary" :loading="isGenerating" :disabled="!formData.category || !aiDescription"
                @click="generateWithAi">
                <template #icon>
                  <n-icon :component="Icons.Bulb" />
                </template>
                Generate Product
              </n-button>
            </n-flex>
          </n-flex>

          <!-- Manual Mode -->
          <n-flex vertical v-else>
            <n-h3 style="margin: 0 0 12px 0;">Basic Information</n-h3>

            <n-form :model="formData" label-placement="top">
              <n-grid :cols="'1 s:1 m:2'" :x-gap="16" :y-gap="12">
                <n-form-item-gi label="Category *" path="category" :span="2">
                  <n-select v-model:value="formData.category" :options="categories"></n-select>
                </n-form-item-gi>
                <n-form-item-gi label="Product Name *" path="name" :span="2">
                  <n-input v-model:value="formData.name" placeholder="e.g., AMD Ryzen 9 7950X" />
                </n-form-item-gi>
                <n-form-item-gi label="Manufacturer *" path="manufacturer">
                  <n-input v-model:value="formData.manufacturer" placeholder="e.g., AMD" />
                </n-form-item-gi>
                <n-form-item-gi label="Price * ($)" path="price">
                  <n-input-number v-model:value="formData.price" />
                </n-form-item-gi>
              </n-grid>
            </n-form>

            <n-flex justify="space-between" align="center">
              <n-button text @click="currentStep = 1">
                <template #icon>
                  <n-icon :component="Icons.ArrowBack" />
                </template>
                Back
              </n-button>
              <n-button type="primary" :disabled="!canProceedToStep3" @click="nextStep">
                <template #icon>
                  <n-icon :component="Icons.ArrowForward" />
                </template>
                Next: Product Details
              </n-button>
            </n-flex>
          </n-flex>
        </n-flex>

        <!-- Step 3: Category-Specific Fields or AI Review -->
        <n-flex vertical v-else-if="currentStep === 3">
          <n-flex vertical v-if="creationMode === 'ai' && generatedProduct">
            <n-h3 style="margin: 0 0 12px 0;">Review AI-Generated Product</n-h3>

            <n-alert type="info" style="margin-bottom: 12px;">
              This product has been generated by AI. Review the details and make any necessary edits before creating it
              as a
              draft.
            </n-alert>

            <n-flex vertical>
              <n-form :model="formData" label-placement="top">
                <n-grid :cols="'1 s:1 m:2'" :x-gap="16" :y-gap="12">
                  <n-form-item-gi label="Product Name *" path="name" :span="2">
                    <n-input v-model:value="formData.name" />
                  </n-form-item-gi>
                  <n-form-item-gi label="Manufacturer *" path="manufacturer">
                    <n-input v-model:value="formData.manufacturer" />
                  </n-form-item-gi>
                  <n-form-item-gi label="Price * ($)" path="price">
                    <n-input-number v-model:value="formData.price" />
                  </n-form-item-gi>
                </n-grid>
              </n-form>

              <ProductFormSelector v-model="productFormData" :category="formData.category" :editable="true" />

              <div v-if="hasSpatialData" style="margin-top: 8px;">
                <n-divider style="margin-bottom: 16px;"></n-divider>

                <ProductViewer3D :dimensions="(productFormData as any).dimensions"
                  :slots="(productFormData as any).slots" :chambers="(productFormData as any).chambers" />
              </div>

              <n-alert v-if="error" type="error" style="margin-top: 12px;">
                {{ error }}
              </n-alert>

              <n-flex justify="space-between" align="center">
                <n-button text @click="currentStep = 2">
                  <template #icon>
                    <n-icon :component="Icons.ArrowBack" />
                  </template>
                  Back
                </n-button>
                <n-button type="primary" :loading="isCreating" @click="createProduct">
                  <template #icon>
                    <n-icon :component="Icons.Check" />
                  </template>
                  Create as Draft
                </n-button>
              </n-flex>
            </n-flex>
          </n-flex>

          <n-flex vertical v-else>
            <n-h3 style="margin: 0 0 12px 0;">{{ formData.category }} Details</n-h3>

            <n-flex vertical>
              <ProductFormSelector v-model="productFormData" :category="formData.category" :editable="true" />

              <div v-if="hasSpatialData" style="margin-top: 8px;">
                <n-divider style="margin-bottom: 16px;"></n-divider>

                <ProductViewer3D :dimensions="(productFormData as any).dimensions"
                  :slots="(productFormData as any).slots" :chambers="(productFormData as any).chambers" />
              </div>

              <n-alert v-if="error" type="error" style="margin-top: 12px;">
                {{ error }}
              </n-alert>

              <n-flex justify="space-between" align="center">
                <n-button text @click="currentStep = 2">
                  <template #icon>
                    <n-icon :component="Icons.ArrowBack" />
                  </template>
                  Back
                </n-button>
                <n-button type="primary" :loading="isCreating" @click="createProduct">
                  <template #icon>
                    <n-icon :component="Icons.Check" />
                  </template>
                  Create Product
                </n-button>
              </n-flex>
            </n-flex>
          </n-flex>
        </n-flex>
      </n-flex>
    </n-card>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import { NCard, NButton, NInput, NInputNumber, NSelect, NAlert, NDivider, NSteps, NStep, NIcon, NForm, NFormItemGi, NGrid, NFlex, NH2, NH3, NText } from 'naive-ui';
import { catalogApi, ProductCategory, categoryLabels, type GenerateProductResponse } from '@/api/catalog';
import { createTypedProduct } from '@/api/catalogTyped';
import ProductFormSelector from '@/components/ProductFormSelector.vue';
import ProductViewer3D from '@/components/ProductViewer3D.vue';
import { fieldsToTypedProduct } from '@/utils/productFieldConverters';
import type { ProductRequest } from '@/types/products';
import { Icons } from '@/utils/icons';

const router = useRouter();

const categories = computed(() =>
  Object.values(ProductCategory).map(value => ({
    label: categoryLabels[value],
    value
  }))
);
const currentStep = ref(1);
const creationMode = ref<'manual' | 'ai' | null>(null);
const isCreating = ref(false);
const isGenerating = ref(false);
const error = ref<string | null>(null);
const aiDescription = ref('');
const generatedProduct = ref<GenerateProductResponse | null>(null);

const formData = ref({
  category: '',
  name: '',
  manufacturer: '',
  price: 0
});

const productFormData = ref<Partial<ProductRequest>>({});

const hasSpatialData = computed(() => {
  const data = productFormData.value as any;
  const hasSlots = data.slots && data.slots.length > 0;
  const hasChambers = data.chambers && data.chambers.length > 0;
  return hasSlots || hasChambers;
});

const canProceedToStep3 = computed(() => {
  return formData.value.category &&
    formData.value.name &&
    formData.value.manufacturer &&
    formData.value.price > 0;
});

function selectCreationMode(mode: 'manual' | 'ai') {
  creationMode.value = mode;
}

async function generateWithAi() {
  if (!formData.value.category || !aiDescription.value) {
    return;
  }

  isGenerating.value = true;
  error.value = null;

  try {
    generatedProduct.value = await catalogApi.generateProductWithAi({
      category: formData.value.category as any,
      description: aiDescription.value
    });

    // Extract the product data and populate form
    const product = generatedProduct.value.product;
    formData.value.name = product.name;
    formData.value.price = product.price;

    // Get manufacturer from specifications or use a default
    const manufacturer = product.specifications && 'Manufacturer' in product.specifications
      ? String(product.specifications.Manufacturer)
      : '';
    formData.value.manufacturer = manufacturer;

    // Convert specifications to typed product form data
    if (product.specifications) {
      const fields = Object.entries(product.specifications)
        .filter(([key]) => key !== 'Manufacturer')
        .reduce((acc, [key, value]) => {
          acc[key] = String(value);
          return acc;
        }, {} as Record<string, string>);

      productFormData.value = fieldsToTypedProduct(fields, formData.value.category);
    }

    // Move to review step
    currentStep.value = 3;
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Failed to generate product with AI';
  } finally {
    isGenerating.value = false;
  }
}

function nextStep() {
  if (currentStep.value === 1 && creationMode.value) {
    currentStep.value = 2;
  } else if (currentStep.value === 2 && canProceedToStep3.value) {
    currentStep.value = 3;
  }
}

async function createProduct() {
  isCreating.value = true;
  error.value = null;

  try {
    // Build the complete typed product request
    const productRequest: any = {
      ...productFormData.value,
      category: formData.value.category,
      name: formData.value.name,
      price: formData.value.price,
      manufacturer: formData.value.manufacturer
    };

    const response = await createTypedProduct(productRequest);

    // If AI-generated (draft), redirect to detail view for review
    // Otherwise, redirect to catalog
    if (creationMode.value === 'ai' && generatedProduct.value) {
      router.push(`/catalog/product/${response.id}`);
    } else {
      router.push('/catalog');
    }
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Failed to create product';
  } finally {
    isCreating.value = false;
  }
}

// Popout opening handled within ProductViewer3D component
</script>

<style scoped>
.fade-in {
  animation: fadeIn 0.3s ease-in;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }

  to {
    opacity: 1;
  }
}

.text-h4 {
  font-size: 2.125rem;
  font-weight: 500;
}

.text-h5 {
  font-size: 1.5rem;
  font-weight: 500;
}

.text-h6 {
  font-size: 1.25rem;
  font-weight: 500;
}

.text-body-2 {
  font-size: 0.875rem;
}
</style>
