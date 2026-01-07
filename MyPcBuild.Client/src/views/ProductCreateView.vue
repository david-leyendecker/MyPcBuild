<template>
  <div class="fadein animation-duration-300">
    <div class="mb-4 flex justify-content-between align-items-center">
      <h2 class="mt-0 mb-0 text-primary">Create New Product</h2>
      <Button 
        icon="pi pi-arrow-left"
        label="Back to Catalog"
        @click="$router.push('/catalog')"
        severity="secondary"
      />
    </div>

    <Card>
      <template #content>
        <div class="flex flex-column gap-4">
          <!-- Step 1: Basic Information -->
          <div v-if="currentStep === 1">
            <h3 class="mt-0 mb-3">Basic Information</h3>
            
            <div class="flex flex-column gap-3">
              <div class="field">
                <label for="category" class="font-semibold">Category *</label>
                <Select 
                  id="category"
                  v-model="formData.category"
                  :options="categories"
                  placeholder="Select a category"
                  class="w-full"
                  @change="onCategoryChange"
                />
              </div>

              <div class="field">
                <label for="name" class="font-semibold">Product Name *</label>
                <InputText 
                  id="name"
                  v-model="formData.name"
                  placeholder="e.g., AMD Ryzen 9 7950X"
                  class="w-full"
                />
              </div>

              <div class="field">
                <label for="manufacturer" class="font-semibold">Manufacturer *</label>
                <InputText 
                  id="manufacturer"
                  v-model="formData.manufacturer"
                  placeholder="e.g., AMD"
                  class="w-full"
                />
              </div>

              <div class="field">
                <label for="price" class="font-semibold">Price *</label>
                <InputNumber 
                  id="price"
                  v-model="formData.price"
                  mode="currency"
                  currency="USD"
                  locale="en-US"
                  class="w-full"
                />
              </div>
            </div>

            <div class="flex justify-content-end mt-4">
              <Button 
                label="Next: Product Details"
                icon="pi pi-arrow-right"
                icon-pos="right"
                @click="nextStep"
                :disabled="!canProceedToStep2"
              />
            </div>
          </div>

          <!-- Step 2: Category-Specific Fields -->
          <div v-else-if="currentStep === 2">
            <h3 class="mt-0 mb-3">{{ formData.category }} Details</h3>

            <div v-if="isLoadingFields" class="flex justify-content-center py-4">
              <ProgressSpinner />
            </div>

            <div v-else-if="fieldDefinitions.length > 0">
              <!-- Use dynamic field renderer -->
              <DynamicFieldRenderer 
                v-model="formData.fields"
                :field-definitions="fieldDefinitions"
              />
            </div>

            <div v-if="error" class="mt-3">
              <Message severity="error" :text="error" />
            </div>

            <div class="flex justify-content-between mt-4">
              <Button 
                label="Back"
                icon="pi pi-arrow-left"
                @click="currentStep = 1"
                severity="secondary"
              />
              <Button 
                label="Create Product"
                icon="pi pi-check"
                @click="createProduct"
                :loading="isCreating"
              />
            </div>
          </div>
        </div>
      </template>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import { catalogApi, type FieldDefinition } from '@/api/catalog';
import Button from 'primevue/button';
import Card from 'primevue/card';
import InputText from 'primevue/inputtext';
import InputNumber from 'primevue/inputnumber';
import Select from 'primevue/select';
import Message from 'primevue/message';
import ProgressSpinner from 'primevue/progressspinner';
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
.field {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}
</style>
