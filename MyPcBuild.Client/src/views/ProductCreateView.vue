<template>
  <div class="fade-in">
    <n-flex :gap="16" style="margin-bottom: 16px; justify-content: space-between; align-items: center;">
      <h2 class="text-h4">Create New Product</h2>
      <n-button 
        text
        @click="$router.push('/catalog')"
      >
        ← Back to Catalog
      </n-button>
    </n-flex>

    <n-card>
      <div style="display: flex; flex-direction: column; gap: 16px;">
          <!-- Step 1: Creation Mode Selection -->
          <div v-if="currentStep === 1">
            <h3 class="text-h5" style="margin-bottom: 12px;">How would you like to create this product?</h3>
            
            <n-flex :gap="16" style="margin-bottom: 16px;">
              <n-card 
                :bordered="true"
                style="flex: 1; cursor: pointer; padding: 16px; border: 2px solid transparent;"
                :style="creationMode === 'manual' ? { borderColor: 'var(--n-primary-color)' } : {}"
                @click="selectCreationMode('manual')"
              >
                <div style="text-align: center;">
                  <div style="font-size: 48px; margin-bottom: 8px;">✏️</div>
                  <h4 class="text-h6" style="margin-bottom: 8px;">Manual Entry</h4>
                  <p class="text-body-2">Enter all product details manually</p>
                </div>
              </n-card>
              
              <n-card 
                :bordered="true"
                style="flex: 1; cursor: pointer; padding: 16px; border: 2px solid transparent;"
                :style="creationMode === 'ai' ? { borderColor: 'var(--n-primary-color)' } : {}"
                @click="selectCreationMode('ai')"
              >
                <div style="text-align: center;">
                  <div style="font-size: 48px; margin-bottom: 8px;">🤖</div>
                  <h4 class="text-h6" style="margin-bottom: 8px;">AI-Assisted</h4>
                  <p class="text-body-2">Generate product details from a description using AI</p>
                </div>
              </n-card>
            </n-flex>

            <n-flex justify="flex-end" style="margin-top: 16px;">
              <n-button 
                type="primary"
                :disabled="!creationMode"
                @click="nextStep"
              >
                Continue →
              </n-button>
            </n-flex>
          </div>

          <!-- Step 2: AI Generation or Basic Information -->
          <div v-else-if="currentStep === 2">
            <!-- AI Mode -->
            <div v-if="creationMode === 'ai'">
              <h3 class="text-h5" style="margin-bottom: 12px;">Generate Product with AI</h3>
              
              <n-flex vertical :gap="12">
                <n-select 
                  v-model:value="formData.category"
                  :options="categories"
                  label="Category *"
                ></n-select>

                <n-input
                  v-model:value="aiDescription"
                  type="textarea"
                  placeholder="e.g., High-performance AMD Ryzen processor with 16 cores, 32 threads, 5.7 GHz boost clock"
                  :rows="4"
                ></n-input>
              </n-flex>

              <n-alert v-if="error" type="error" style="margin-top: 12px;">
                {{ error }}
              </n-alert>

              <n-flex justify="space-between" style="margin-top: 16px;">
                <n-button 
                  text
                  @click="currentStep = 1"
                >
                  ← Back
                </n-button>
                <n-button 
                  type="primary"
                  :loading="isGenerating"
                  :disabled="!formData.category || !aiDescription"
                  @click="generateWithAi"
                >
                  🤖 Generate Product
                </n-button>
              </n-flex>
            </div>

            <!-- Manual Mode -->
            <div v-else>
              <h3 class="text-h5" style="margin-bottom: 12px;">Basic Information</h3>
              
              <n-flex vertical :gap="12">
                <n-select 
                  v-model:value="formData.category"
                  :options="categories"
                  label="Category *"
                ></n-select>

                <n-input 
                  v-model:value="formData.name"
                  placeholder="e.g., AMD Ryzen 9 7950X"
                >
                  <template #prefix>Product Name *</template>
                </n-input>

                <n-input 
                  v-model:value="formData.manufacturer"
                  placeholder="e.g., AMD"
                >
                  <template #prefix>Manufacturer *</template>
                </n-input>

                <n-input-number 
                  v-model:value="formData.price"
                >
                  <template #prefix>Price * ($)</template>
                </n-input-number>
              </n-flex>

              <n-flex justify="space-between" style="margin-top: 16px;">
                <n-button 
                  text
                  @click="currentStep = 1"
                >
                  ← Back
                </n-button>
                <n-button 
                  type="primary"
                  :disabled="!canProceedToStep3"
                  @click="nextStep"
                >
                  Next: Product Details →
                </n-button>
              </n-flex>
            </div>
          </div>

          <!-- Step 3: Category-Specific Fields or AI Review -->
          <div v-else-if="currentStep === 3">
            <div v-if="creationMode === 'ai' && generatedProduct">
              <h3 class="text-h5" style="margin-bottom: 12px;">Review AI-Generated Product</h3>

              <n-alert type="info" style="margin-bottom: 12px;">
                This product has been generated by AI. Review the details and make any necessary edits before creating it as a draft.
              </n-alert>

              <n-flex vertical :gap="12">
                <n-input 
                  v-model:value="formData.name"
                >
                  <template #prefix>Product Name *</template>
                </n-input>

                <n-input 
                  v-model:value="formData.manufacturer"
                >
                  <template #prefix>Manufacturer *</template>
                </n-input>

                <n-input-number 
                  v-model:value="formData.price"
                >
                  <template #prefix>Price * ($)</template>
                </n-input-number>

                <!-- Use ProductFormSelector -->
                <ProductFormSelector 
                  v-model="productFormData"
                  :category="formData.category"
                  :editable="true"
                />

                <!-- 3D Preview for products with spatial data -->
                <div v-if="hasSpatialData" style="margin-top: 24px;">
                  <n-divider style="margin-bottom: 16px;"></n-divider>
                  <n-flex justify="space-between" align="center" style="margin-bottom: 12px;">
                    <h4 class="text-h6">3D Preview</h4>
                    <n-button
                      text
                      @click="open3DInPopout"
                    >
                      ↗ Open in Popout
                    </n-button>
                  </n-flex>
                  <p class="text-body-2" style="margin-bottom: 12px;">
                    Interactive visualization of slots and chambers
                  </p>
                  <ProductViewer3D 
                    :dimensions="(productFormData as any).dimensions"
                    :slots="(productFormData as any).slots"
                    :chambers="(productFormData as any).chambers"
                  />
                </div>

                <n-alert v-if="error" type="error" style="margin-top: 12px;">
                  {{ error }}
                </n-alert>

                <n-flex justify="space-between" style="margin-top: 16px;">
                  <n-button 
                    text
                    @click="currentStep = 2"
                  >
                    ← Back
                  </n-button>
                  <n-button 
                    type="primary"
                    :loading="isCreating"
                    @click="createProduct"
                  >
                    ✓ Create as Draft
                  </n-button>
                </n-flex>
              </n-flex>
            </div>

            <div v-else>
              <h3 class="text-h5" style="margin-bottom: 12px;">{{ formData.category }} Details</h3>

              <n-flex vertical :gap="12">
                <!-- Use ProductFormSelector -->
                <ProductFormSelector 
                  v-model="productFormData"
                  :category="formData.category"
                  :editable="true"
                />

                <!-- 3D Preview for products with spatial data -->
                <div v-if="hasSpatialData" style="margin-top: 24px;">
                  <n-divider style="margin-bottom: 16px;"></n-divider>
                  <n-flex justify="space-between" align="center" style="margin-bottom: 12px;">
                    <h4 class="text-h6">3D Preview</h4>
                    <n-button
                      text
                      @click="open3DInPopout"
                    >
                      ↗ Open in Popout
                    </n-button>
                  </n-flex>
                  <p class="text-body-2" style="margin-bottom: 12px;">
                    Interactive visualization of slots and chambers
                  </p>
                  <ProductViewer3D 
                    :dimensions="(productFormData as any).dimensions"
                    :slots="(productFormData as any).slots"
                    :chambers="(productFormData as any).chambers"
                  />
                </div>

                <n-alert v-if="error" type="error" style="margin-top: 12px;">
                  {{ error }}
                </n-alert>

                <n-flex justify="space-between" style="margin-top: 16px;">
                  <n-button 
                    text
                    @click="currentStep = 2"
                  >
                    ← Back
                  </n-button>
                  <n-button 
                    type="primary"
                    :loading="isCreating"
                    @click="createProduct"
                  >
                    ✓ Create Product
                  </n-button>
                </n-flex>
              </n-flex>
            </div>
          </div>
        </div>
      </n-card>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import { NCard, NButton, NInput, NInputNumber, NSelect, NAlert, NFlex, NDivider } from 'naive-ui';
import { catalogApi, ProductCategory, categoryLabels, type GenerateProductResponse } from '@/api/catalog';
import { createTypedProduct } from '@/api/catalogTyped';
import { use3DPopout } from '@/composables/use3DPopout';
import ProductFormSelector from '@/components/ProductFormSelector.vue';
import ProductViewer3D from '@/components/ProductViewer3D.vue';
import { fieldsToTypedProduct } from '@/utils/productFieldConverters';
import type { ProductRequest } from '@/types/products';

const router = useRouter();
const { openPopout } = use3DPopout();

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

function open3DInPopout() {
  const data = productFormData.value as any;
  openPopout({
    component: ProductViewer3D,
    props: {
      dimensions: data.dimensions,
      slots: data.slots,
      chambers: data.chambers,
    },
    title: `3D Preview - ${formData.value.name || 'New Product'}`,
  });
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
