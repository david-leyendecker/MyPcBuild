<template>
  <div class="fade-in">
    <div class="mb-4 d-flex justify-space-between align-center">
      <h2 class="text-h4 text-primary">Create New Product</h2>
      <v-btn 
        prepend-icon="mdi-arrow-left"
        variant="text"
        @click="$router.push('/catalog')"
      >
        Back to Catalog
      </v-btn>
    </div>

    <v-card>
      <v-card-text>
        <div class="d-flex flex-column ga-4">
          <!-- Step 1: Creation Mode Selection -->
          <div v-if="currentStep === 1">
            <h3 class="text-h5 mb-3">How would you like to create this product?</h3>
            
            <v-row>
              <v-col cols="12" md="6">
                <v-card 
                  variant="outlined"
                  hover
                  @click="selectCreationMode('manual')"
                  class="cursor-pointer pa-4"
                  :class="creationMode === 'manual' ? 'border-primary' : ''"
                >
                  <v-icon size="48" color="primary" class="mb-2">mdi-pencil</v-icon>
                  <h4 class="text-h6 mb-2">Manual Entry</h4>
                  <p class="text-body-2">Enter all product details manually</p>
                </v-card>
              </v-col>
              
              <v-col cols="12" md="6">
                <v-card 
                  variant="outlined"
                  hover
                  @click="selectCreationMode('ai')"
                  class="cursor-pointer pa-4"
                  :class="creationMode === 'ai' ? 'border-primary' : ''"
                >
                  <v-icon size="48" color="primary" class="mb-2">mdi-robot</v-icon>
                  <h4 class="text-h6 mb-2">AI-Assisted</h4>
                  <p class="text-body-2">Generate product details from a description using AI</p>
                </v-card>
              </v-col>
            </v-row>

            <div class="d-flex justify-end mt-4">
              <v-btn 
                append-icon="mdi-arrow-right"
                color="primary"
                :disabled="!creationMode"
                @click="nextStep"
              >
                Continue
              </v-btn>
            </div>
          </div>

          <!-- Step 2: AI Generation or Basic Information -->
          <div v-else-if="currentStep === 2">
            <!-- AI Mode -->
            <div v-if="creationMode === 'ai'">
              <h3 class="text-h5 mb-3">Generate Product with AI</h3>
              
              <div class="d-flex flex-column ga-3">
                <v-select 
                  v-model="formData.category"
                  :items="categories"
                  label="Category *"
                ></v-select>

                <v-textarea
                  v-model="aiDescription"
                  label="Product Description *"
                  placeholder="e.g., High-performance AMD Ryzen processor with 16 cores, 32 threads, 5.7 GHz boost clock"
                  rows="4"
                  auto-grow
                ></v-textarea>
              </div>

              <v-alert v-if="error" type="error" class="mt-3">
                {{ error }}
              </v-alert>

              <div class="d-flex justify-space-between mt-4">
                <v-btn 
                  prepend-icon="mdi-arrow-left"
                  variant="text"
                  @click="currentStep = 1"
                >
                  Back
                </v-btn>
                <v-btn 
                  prepend-icon="mdi-robot"
                  color="primary"
                  :loading="isGenerating"
                  :disabled="!formData.category || !aiDescription"
                  @click="generateWithAi"
                >
                  Generate Product
                </v-btn>
              </div>
            </div>

            <!-- Manual Mode -->
            <div v-else>
              <h3 class="text-h5 mb-3">Basic Information</h3>
              
              <div class="d-flex flex-column ga-3">
                <v-select 
                  v-model="formData.category"
                  :items="categories"
                  label="Category *"
                ></v-select>

                <v-text-field 
                  v-model="formData.name"
                  label="Product Name *"
                  placeholder="e.g., AMD Ryzen 9 7950X"
                ></v-text-field>

                <v-text-field 
                  v-model="formData.manufacturer"
                  label="Manufacturer *"
                  placeholder="e.g., AMD"
                ></v-text-field>

                <v-text-field 
                  v-model.number="formData.price"
                  label="Price *"
                  type="number"
                  prefix="$"
                ></v-text-field>
              </div>

              <div class="d-flex justify-space-between mt-4">
                <v-btn 
                  prepend-icon="mdi-arrow-left"
                  variant="text"
                  @click="currentStep = 1"
                >
                  Back
                </v-btn>
                <v-btn 
                  append-icon="mdi-arrow-right"
                  color="primary"
                  :disabled="!canProceedToStep3"
                  @click="nextStep"
                >
                  Next: Product Details
                </v-btn>
              </div>
            </div>
          </div>

          <!-- Step 3: Category-Specific Fields or AI Review -->
          <div v-else-if="currentStep === 3">
            <div v-if="creationMode === 'ai' && generatedProduct">
              <h3 class="text-h5 mb-3">Review AI-Generated Product</h3>

              <v-alert type="info" class="mb-3">
                This product has been generated by AI. Review the details and make any necessary edits before creating it as a draft.
              </v-alert>

              <div class="d-flex flex-column ga-3 mb-4">
                <v-text-field 
                  v-model="formData.name"
                  label="Product Name *"
                ></v-text-field>

                <v-text-field 
                  v-model="formData.manufacturer"
                  label="Manufacturer *"
                ></v-text-field>

                <v-text-field 
                  v-model.number="formData.price"
                  label="Price *"
                  type="number"
                  prefix="$"
                ></v-text-field>
              </div>

              <!-- Use ProductFormSelector -->
              <ProductFormSelector 
                v-model="productFormData"
                :category="formData.category"
                :editable="true"
              />

              <v-alert v-if="error" type="error" class="mt-3">
                {{ error }}
              </v-alert>

              <div class="d-flex justify-space-between mt-4">
                <v-btn 
                  prepend-icon="mdi-arrow-left"
                  variant="text"
                  @click="currentStep = 2"
                >
                  Back
                </v-btn>
                <v-btn 
                  prepend-icon="mdi-check"
                  color="primary"
                  :loading="isCreating"
                  @click="createProduct"
                >
                  Create as Draft
                </v-btn>
              </div>
            </div>

            <div v-else>
              <h3 class="text-h5 mb-3">{{ formData.category }} Details</h3>

              <!-- Use ProductFormSelector -->
              <ProductFormSelector 
                v-model="productFormData"
                :category="formData.category"
                :editable="true"
              />

              <v-alert v-if="error" type="error" class="mt-3">
                {{ error }}
              </v-alert>

              <div class="d-flex justify-space-between mt-4">
                <v-btn 
                  prepend-icon="mdi-arrow-left"
                  variant="text"
                  @click="currentStep = 2"
                >
                  Back
                </v-btn>
                <v-btn 
                  prepend-icon="mdi-check"
                  color="primary"
                  :loading="isCreating"
                  @click="createProduct"
                >
                  Create Product
                </v-btn>
              </div>
            </div>
          </div>
        </div>
      </v-card-text>
    </v-card>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import { catalogApi, ProductCategory, categoryLabels, type GenerateProductResponse } from '@/api/catalog';
import { createTypedProduct } from '@/api/catalogTyped';
import ProductFormSelector from '@/components/ProductFormSelector.vue';
import { fieldsToTypedProduct } from '@/utils/productFieldConverters';
import type { ProductRequest } from '@/types/products';

const router = useRouter();

const categories = computed(() => 
  Object.values(ProductCategory).map(value => ({
    title: categoryLabels[value],
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

.border-primary {
  border-color: rgb(var(--v-theme-primary)) !important;
  border-width: 2px !important;
}
</style>
