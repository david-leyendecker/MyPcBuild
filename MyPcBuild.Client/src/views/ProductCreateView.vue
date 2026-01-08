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
          <!-- Step 1: Basic Information -->
          <div v-if="currentStep === 1">
            <h3 class="text-h5 mb-3">Basic Information</h3>
            
            <div class="d-flex flex-column ga-3">
              <v-select 
                v-model="formData.category"
                :items="categories"
                label="Category *"
                @update:model-value="onCategoryChange"
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

            <div class="d-flex justify-end mt-4">
              <v-btn 
                append-icon="mdi-arrow-right"
                color="primary"
                :disabled="!canProceedToStep2"
                @click="nextStep"
              >
                Next: Product Details
              </v-btn>
            </div>
          </div>

          <!-- Step 2: Category-Specific Fields -->
          <div v-else-if="currentStep === 2">
            <h3 class="text-h5 mb-3">{{ formData.category }} Details</h3>

            <div v-if="isLoadingFields" class="d-flex justify-center py-4">
              <v-progress-circular indeterminate color="primary"></v-progress-circular>
            </div>

            <div v-else-if="fieldDefinitions.length > 0">
              <!-- Use dynamic field renderer -->
              <DynamicFieldRenderer 
                v-model="formData.fields"
                :field-definitions="fieldDefinitions"
              />
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
      </v-card-text>
    </v-card>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import { catalogApi, type FieldDefinition } from '@/api/catalog';
import DynamicFieldRenderer from '@/components/DynamicFieldRenderer.vue';

const router = useRouter();

const categories = ref(['CPU', 'Motherboard', 'GPU', 'RAM', 'Storage', 'PSU', 'PCCase', 'Cooler']);
const currentStep = ref(1);
const isLoadingFields = ref(false);
const isCreating = ref(false);
const error = ref<string | null>(null);
const fieldDefinitions = ref<FieldDefinition[]>([]);

const formData = ref({
  category: '',
  name: '',
  manufacturer: '',
  price: 0,
  fields: {} as Record<string, string>
});

const canProceedToStep2 = computed(() => {
  return formData.value.category && 
         formData.value.name && 
         formData.value.manufacturer && 
         formData.value.price > 0;
});

async function onCategoryChange() {
  if (formData.value.category) {
    isLoadingFields.value = true;
    error.value = null;
    try {
      fieldDefinitions.value = await catalogApi.getFieldDefinitions(formData.value.category);
      // Initialize fields with empty values
      formData.value.fields = {};
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load field definitions';
    } finally {
      isLoadingFields.value = false;
    }
  }
}

function nextStep() {
  if (canProceedToStep2.value) {
    currentStep.value = 2;
  }
}

async function createProduct() {
  isCreating.value = true;
  error.value = null;
  
  try {
    await catalogApi.createProduct({
      category: formData.value.category,
      name: formData.value.name,
      price: formData.value.price,
      manufacturer: formData.value.manufacturer,
      fields: formData.value.fields
    });

    // Success - redirect to catalog
    router.push('/catalog');
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
</style>
