<template>
  <div class="fade-in">
    <n-flex vertical :gap="16">
      <n-flex justify="space-between" align="center" style="flex-direction: row;">
        <h2 class="text-h4">
          {{ isEditMode ? 'Edit Product' : (product?.isDraft ? 'Review Draft Product' : 'Product Details') }}
        </h2>
        <n-flex :gap="8">
          <n-button 
            v-if="!isEditMode && !product?.isDraft"
            type="primary"
            @click="enterEditMode"
          >
            ✏️ Edit Product
          </n-button>
          <n-button 
            text
            @click="$router.push('/catalog')"
          >
            ← Back to Catalog
          </n-button>
        </n-flex>
      </n-flex>

      <n-alert v-if="product?.isDraft" type="warning">
        ⚠️ This product is a draft and cannot be added to builds until published.
      </n-alert>

      <n-alert v-if="isEditMode" type="info">
        ✏️ You are editing this product. Make your changes and click "Save Changes" to update.
      </n-alert>

      <n-card v-if="isLoading" style="padding: 32px;">
        <div style="display: flex; justify-content: center;">
          <n-spin></n-spin>
        </div>
      </n-card>

      <n-card v-else-if="error">
        <n-alert type="error">
          {{ error }}
        </n-alert>
      </n-card>

      <n-card v-else-if="product">
        <n-flex vertical :gap="12">
          <div>
            <h3 class="text-h5" style="margin-bottom: 12px;">Basic Information</h3>
            
            <n-flex vertical :gap="12">
              <n-input 
                v-model:value="formData.name"
                :disabled="!isEditMode"
              >
                <template #prefix>Product Name *</template>
              </n-input>

              <n-input 
                v-model:value="formData.manufacturer"
                :disabled="!isEditMode"
              >
                <template #prefix>Manufacturer *</template>
              </n-input>

              <n-input-number 
                v-model:value="formData.price"
                :disabled="!isEditMode"
              >
                <template #prefix>Price * ($)</template>
              </n-input-number>

              <n-input 
                v-model:value="formData.category"
                disabled
              >
                <template #prefix>Category</template>
              </n-input>
            </n-flex>
          </div>

          <n-divider></n-divider>

          <div>
            <h3 class="text-h5" style="margin-bottom: 12px;">{{ formData.category }} Details</h3>

            <!-- Use ProductFormSelector -->
            <ProductFormSelector 
              v-model="productFormData"
              :category="formData.category"
              :editable="isEditMode"
            />
          </div>

          <!-- 3D Preview for products with spatial data -->
          <div v-if="hasSpatialData">
            <n-divider></n-divider>
            <n-flex justify="space-between" align="center" style="margin-bottom: 12px;">
              <h3 class="text-h5">3D Preview</h3>
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

          <n-alert v-if="publishError || updateError" type="error">
            {{ publishError || updateError }}
          </n-alert>

          <n-alert v-if="publishSuccess || updateSuccess" type="success">
            {{ publishSuccess ? 'Product successfully published!' : 'Product successfully updated!' }}
          </n-alert>

          <n-flex justify="space-between" style="margin-top: 16px;">
            <n-button 
              text
              @click="isEditMode ? cancelEdit() : $router.push('/catalog')"
            >
              {{ isEditMode ? '← Cancel' : '← Back to Catalog' }}
            </n-button>
            
            <n-flex :gap="8">
              <n-button 
                v-if="isEditMode"
                type="primary"
                :loading="isUpdating"
                @click="saveProduct"
              >
                💾 Save Changes
              </n-button>
              
              <n-button 
                v-if="product?.isDraft && !isEditMode"
                type="success"
                :loading="isPublishing"
                @click="publishProduct"
              >
                ✓ Publish Product
              </n-button>
            </n-flex>
          </n-flex>
        </n-flex>
      </n-card>
    </n-flex>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { NCard, NButton, NInput, NInputNumber, NAlert, NFlex, NDivider, NSpin } from 'naive-ui';
import { catalogApi } from '@/api/catalog';
import { getTypedProduct, updateTypedProduct } from '@/api/catalogTyped';
import { use3DPopout } from '@/composables/use3DPopout';
import ProductFormSelector from '@/components/ProductFormSelector.vue';
import ProductViewer3D from '@/components/ProductViewer3D.vue';
import type { ProductRequest, ProductResponse } from '@/types/products';

const route = useRoute();
const router = useRouter();
const { openPopout } = use3DPopout();

const product = ref<ProductResponse | null>(null);
const isLoading = ref(true);
const isPublishing = ref(false);
const isUpdating = ref(false);
const isEditMode = ref(false);
const error = ref<string | null>(null);
const publishError = ref<string | null>(null);
const publishSuccess = ref(false);
const updateError = ref<string | null>(null);
const updateSuccess = ref(false);

const formData = ref({
  category: '',
  name: '',
  manufacturer: '',
  price: 0
});

const originalFormData = ref({
  category: '',
  name: '',
  manufacturer: '',
  price: 0
});

const productFormData = ref<Partial<ProductRequest>>({});
const originalProductFormData = ref<Partial<ProductRequest>>({});

const hasSpatialData = computed(() => {
  const data = productFormData.value as any;
  const hasSlots = data.slots && data.slots.length > 0;
  const hasChambers = data.chambers && data.chambers.length > 0;
  return hasSlots || hasChambers;
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
    product.value = await getTypedProduct(productId);
    
    // Populate form data from product
    formData.value.name = product.value.name;
    formData.value.price = product.value.price;
    formData.value.category = product.value.category;
    formData.value.manufacturer = product.value.manufacturer;
    
    // Extract category-specific data (remove base ProductBase fields)
    const { id, isDraft, publishedAt, category, name, price, manufacturer, ...categoryData } = product.value;
    productFormData.value = categoryData as Partial<ProductRequest>;
    
    // Store original values for cancel operation
    originalFormData.value = JSON.parse(JSON.stringify(formData.value));
    originalProductFormData.value = JSON.parse(JSON.stringify(productFormData.value));
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Failed to load product';
  } finally {
    isLoading.value = false;
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
  productFormData.value = JSON.parse(JSON.stringify(originalProductFormData.value));
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
    // Build the complete typed product request
    const productRequest: any = {
      ...productFormData.value,
      category: formData.value.category,
      name: formData.value.name,
      price: formData.value.price,
      manufacturer: formData.value.manufacturer
    };

    await updateTypedProduct(product.value.id, productRequest);
    
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

function open3DInPopout() {
  const data = productFormData.value as any;
  openPopout({
    component: ProductViewer3D,
    props: {
      dimensions: data.dimensions,
      slots: data.slots,
      chambers: data.chambers,
    },
    title: `3D Preview - ${formData.value.name}`,
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

.text-body-2 {
  font-size: 0.875rem;
}
</style>
