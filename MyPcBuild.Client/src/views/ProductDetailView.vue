<template>
  <div class="fade-in">
    <div class="mb-4 d-flex justify-space-between align-center">
      <h2 class="text-h4 text-primary">
        {{ product?.isDraft ? 'Review Draft Product' : 'Product Details' }}
      </h2>
      <v-btn 
        prepend-icon="mdi-arrow-left"
        variant="text"
        @click="$router.push('/catalog')"
      >
        Back to Catalog
      </v-btn>
    </div>

    <v-alert v-if="product?.isDraft" type="warning" class="mb-4">
      <v-icon start icon="mdi-alert-circle"></v-icon>
      This product is a draft and cannot be added to builds until published.
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
              readonly
            ></v-text-field>

            <v-text-field 
              v-model="formData.manufacturer"
              label="Manufacturer *"
              readonly
            ></v-text-field>

            <v-text-field 
              v-model.number="formData.price"
              label="Price *"
              type="number"
              prefix="$"
              readonly
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
            <!-- Display fields as read-only for now -->
            <div class="d-flex flex-column ga-3">
              <v-text-field
                v-for="field in fieldDefinitions"
                :key="field.name"
                :label="field.name"
                :model-value="formData.fields[field.name] || ''"
                :suffix="field.unit || undefined"
                readonly
              ></v-text-field>
            </div>
          </div>

          <v-alert v-if="publishError" type="error" class="mt-3">
            {{ publishError }}
          </v-alert>

          <v-alert v-if="publishSuccess" type="success" class="mt-3">
            Product successfully published!
          </v-alert>

          <div class="d-flex justify-space-between mt-4">
            <v-btn 
              prepend-icon="mdi-arrow-left"
              variant="text"
              @click="$router.push('/catalog')"
            >
              Back to Catalog
            </v-btn>
            
            <v-btn 
              v-if="product.isDraft"
              prepend-icon="mdi-check-circle"
              color="success"
              :loading="isPublishing"
              @click="publishProduct"
            >
              Publish Product
            </v-btn>
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

const route = useRoute();
const router = useRouter();

const product = ref<Product | null>(null);
const isLoading = ref(true);
const isLoadingFields = ref(false);
const isPublishing = ref(false);
const error = ref<string | null>(null);
const publishError = ref<string | null>(null);
const publishSuccess = ref(false);
const fieldDefinitions = ref<FieldDefinition[]>([]);

const formData = ref({
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
    fieldDefinitions.value = await catalogApi.getFieldDefinitions(formData.value.category);
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
