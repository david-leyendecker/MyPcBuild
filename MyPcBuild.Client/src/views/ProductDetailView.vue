<template>
  <div class="fade-in">
    <div class="mb-4 d-flex justify-space-between align-center">
      <h2 class="text-h4 text-primary">
        {{ isEditMode ? 'Edit Product' : (product?.isDraft ? 'Review Draft Product' : 'Product Details') }}
      </h2>
      <div class="d-flex ga-2">
        <v-btn 
          v-if="!isEditMode && !product?.isDraft"
          prepend-icon="mdi-pencil"
          color="primary"
          variant="tonal"
          @click="enterEditMode"
        >
          Edit Product
        </v-btn>
        <v-btn 
          prepend-icon="mdi-arrow-left"
          variant="text"
          @click="$router.push('/catalog')"
        >
          Back to Catalog
        </v-btn>
      </div>
    </div>

    <v-alert v-if="product?.isDraft" type="warning" class="mb-4">
      <v-icon start icon="mdi-alert-circle"></v-icon>
      This product is a draft and cannot be added to builds until published.
    </v-alert>

    <v-alert v-if="isEditMode" type="info" class="mb-4">
      <v-icon start icon="mdi-pencil"></v-icon>
      You are editing this product. Make your changes and click "Save Changes" to update.
    </v-alert>

    <v-card v-if="isLoading" class="pa-8">
      <div class="d-flex justify-center">
        <v-progress-circular indeterminate color="primary"></v-progress-circular>
      </div>
    </v-card>

    <v-card v-else-if="error">
      <v-card-text>
        <v-alert type="error">
          {{ error }}
        </v-alert>
      </v-card-text>
    </v-card>

    <v-card v-else-if="product">
      <v-card-text>
        <div class="d-flex flex-column ga-4">
          <h3 class="text-h5 mb-3">Basic Information</h3>
          
          <div class="d-flex flex-column ga-3">
            <v-text-field 
              v-model="formData.name"
              label="Product Name *"
              :readonly="!isEditMode"
            ></v-text-field>

            <v-text-field 
              v-model="formData.manufacturer"
              label="Manufacturer *"
              :readonly="!isEditMode"
            ></v-text-field>

            <v-text-field 
              v-model.number="formData.price"
              label="Price *"
              type="number"
              prefix="$"
              :readonly="!isEditMode"
            ></v-text-field>

            <v-text-field 
              v-model="formData.category"
              label="Category"
              readonly
            ></v-text-field>
          </div>

          <v-divider class="my-4"></v-divider>

          <h3 class="text-h5 mb-3">{{ formData.category }} Details</h3>

          <div v-if="isLoadingFields" class="d-flex justify-center py-4">
            <v-progress-circular indeterminate color="primary"></v-progress-circular>
          </div>

          <div v-else-if="fieldDefinitions.length > 0">
            <!-- Display fields as read-only or editable based on mode -->
            <div v-if="!isEditMode" class="d-flex flex-column ga-3">
              <v-text-field
                v-for="field in fieldDefinitions"
                :key="field.name"
                :label="field.name"
                :model-value="formData.fields[field.name] || ''"
                :suffix="field.unit || undefined"
                readonly
              ></v-text-field>
            </div>
            
            <!-- Use DynamicFieldRenderer for edit mode -->
            <DynamicFieldRenderer 
              v-else
              v-model="formData.fields"
              :field-definitions="fieldDefinitions"
            />
          </div>

          <v-alert v-if="publishError || updateError" type="error" class="mt-3">
            {{ publishError || updateError }}
          </v-alert>

          <v-alert v-if="publishSuccess || updateSuccess" type="success" class="mt-3">
            {{ publishSuccess ? 'Product successfully published!' : 'Product successfully updated!' }}
          </v-alert>

          <div class="d-flex justify-space-between mt-4">
            <v-btn 
              prepend-icon="mdi-arrow-left"
              variant="text"
              @click="isEditMode ? cancelEdit() : $router.push('/catalog')"
            >
              {{ isEditMode ? 'Cancel' : 'Back to Catalog' }}
            </v-btn>
            
            <div class="d-flex ga-2">
              <v-btn 
                v-if="isEditMode"
                prepend-icon="mdi-content-save"
                color="primary"
                :loading="isUpdating"
                @click="saveProduct"
              >
                Save Changes
              </v-btn>
              
              <v-btn 
                v-if="product?.isDraft && !isEditMode"
                prepend-icon="mdi-check-circle"
                color="success"
                :loading="isPublishing"
                @click="publishProduct"
              >
                Publish Product
              </v-btn>
            </div>
          </div>
        </div>
      </v-card-text>
    </v-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { catalogApi, type FieldDefinition, type Product } from '@/api/catalog';
import DynamicFieldRenderer from '@/components/DynamicFieldRenderer.vue';

const route = useRoute();
const router = useRouter();

const product = ref<Product | null>(null);
const isLoading = ref(true);
const isLoadingFields = ref(false);
const isPublishing = ref(false);
const isUpdating = ref(false);
const isEditMode = ref(false);
const error = ref<string | null>(null);
const publishError = ref<string | null>(null);
const publishSuccess = ref(false);
const updateError = ref<string | null>(null);
const updateSuccess = ref(false);
const fieldDefinitions = ref<FieldDefinition[]>([]);

const formData = ref({
  category: '',
  name: '',
  manufacturer: '',
  price: 0,
  fields: {} as Record<string, string>
});

const originalFormData = ref({
  category: '',
  name: '',
  manufacturer: '',
  price: 0,
  fields: {} as Record<string, string>
});

onMounted(async () => {
  await loadProduct();
});

async function loadProduct() {
  const productId = route.params.id as string;
  
  if (!productId) {
    error.value = 'Product ID is required';
    isLoading.value = false;
    return;
  }

  try {
    product.value = await catalogApi.getProduct(productId);
    
    // Populate form data from product
    formData.value.name = product.value.name;
    formData.value.price = product.value.price;
    formData.value.category = product.value.category;
    formData.value.manufacturer = product.value.manufacturer;
    
    // Extract manufacturer and other fields from specifications
    if (product.value.specifications) {
      // Populate other fields
      formData.value.fields = Object.entries(product.value.specifications)
        .reduce((acc, [key, value]) => {
          acc[key] = String(value);
          return acc;
        }, {} as Record<string, string>);
    }

    // Load field definitions for the category
    await loadFieldDefinitions();
    
    // Store original values for cancel operation
    originalFormData.value = JSON.parse(JSON.stringify(formData.value));
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Failed to load product';
  } finally {
    isLoading.value = false;
  }
}

async function loadFieldDefinitions() {
  if (!formData.value.category) {
    return;
  }

  isLoadingFields.value = true;
  try {
    fieldDefinitions.value = await catalogApi.getFieldDefinitions(formData.value.category as any);
  } catch (err) {
    console.error('Failed to load field definitions:', err);
  } finally {
    isLoadingFields.value = false;
  }
}

async function publishProduct() {
  if (!product.value) {
    return;
  }

  isPublishing.value = true;
  publishError.value = null;
  publishSuccess.value = false;
  
  try {
    await catalogApi.publishProduct(product.value.id);
    publishSuccess.value = true;
    
    // Reload product to get updated status
    await loadProduct();
    
    // Redirect to catalog after a short delay
    setTimeout(() => {
      router.push('/catalog');
    }, 1500);
  } catch (err) {
    publishError.value = err instanceof Error ? err.message : 'Failed to publish product';
  } finally {
    isPublishing.value = false;
  }
}

function enterEditMode() {
  isEditMode.value = true;
  updateSuccess.value = false;
  updateError.value = null;
}

function cancelEdit() {
  // Restore original values
  formData.value = JSON.parse(JSON.stringify(originalFormData.value));
  isEditMode.value = false;
  updateSuccess.value = false;
  updateError.value = null;
}

async function saveProduct() {
  if (!product.value) {
    return;
  }

  isUpdating.value = true;
  updateError.value = null;
  updateSuccess.value = false;
  
  try {
    await catalogApi.updateProduct(product.value.id, {
      category: formData.value.category as any,
      name: formData.value.name,
      price: formData.value.price,
      manufacturer: formData.value.manufacturer,
      fields: formData.value.fields
    });
    
    updateSuccess.value = true;
    
    // Reload product to get updated data
    await loadProduct();
    
    // Exit edit mode after a short delay
    setTimeout(() => {
      isEditMode.value = false;
      updateSuccess.value = false;
    }, 1500);
  } catch (err) {
    updateError.value = err instanceof Error ? err.message : 'Failed to update product';
  } finally {
    isUpdating.value = false;
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
