<template>
  <n-flex vertical :size="12">
    <p style="font-size: 14px; opacity: 0.7;">Add a component to your build with optional slot placement</p>

    <!-- Product Selection -->
    <div v-if="!selectedProductId">
      <n-flex vertical :size="12">
        <n-input 
          v-model:value="searchQuery"
          placeholder="Search components..."
          @keyup.enter="handleSearch"
        />
        <n-flex :size="8" wrap>
          <n-button 
            v-for="category in categories"
            :key="category"
            :type="selectedCategory === category ? 'primary' : 'default'"
            size="small"
            @click="selectCategory(category)"
          >
            {{ categoryDisplayMap[category] }}
          </n-button>
        </n-flex>
      </n-flex>

      <n-flex v-if="isLoading" justify="center" style="padding: 16px 0;">
        <n-spin size="medium" />
      </n-flex>

      <n-empty v-else-if="filteredProducts.length === 0" description="No components found">
        <template #extra>
          <n-button @click="searchQuery = ''; selectedCategory = null">
            Clear Filters
          </n-button>
        </template>
      </n-empty>

      <div 
        v-else 
        style="max-height: 400px; overflow-y: auto; margin-top: 12px; border: 1px solid var(--n-border-color); border-radius: 4px;"
      >
        <div 
          v-for="product in filteredProducts"
          :key="product.id"
          class="product-item"
          style="padding: 12px; border-bottom: 1px solid var(--n-border-color); cursor: pointer;"
          @click="selectProduct(product.id)"
        >
          <n-flex justify="space-between" align="center">
            <div>
              <h4 style="font-size: 16px; margin-bottom: 4px;">{{ product.name }}</h4>
              <p style="color: var(--n-color-success); font-weight: 600; font-size: 14px; margin: 0;">${{ product.price.toFixed(2) }}</p>
            </div>
            <span>→</span>
          </n-flex>
        </div>
      </div>

      <n-flex justify="flex-end" :size="8" style="padding-top: 12px; border-top: 1px solid var(--n-border-color);">
        <n-button 
          text
          @click="$emit('close')"
        >
          <template #icon>
            <n-icon :component="Icons.Close" />
          </template>
          Cancel
        </n-button>
      </n-flex>
    </div>

    <!-- Slot Selection (optional) -->
    <div v-else>
      <div style="margin-bottom: 12px;">
        <h4 style="font-size: 16px; margin-bottom: 8px;">Selected: {{ selectedProduct?.name }}</h4>
        <p style="font-size: 14px; opacity: 0.7;">
          Choose a slot for placement (optional) or add without slot assignment
        </p>
      </div>

      <n-card v-if="loadingSlots" style="padding: 16px;">
        <n-spin size="medium" />
      </n-card>

      <div v-else>
        <div v-if="availableSlots.length > 0" style="margin-bottom: 16px;">
          <h5 style="font-size: 14px; margin-bottom: 8px; font-weight: 600;">Available Slots</h5>
          <div 
            style="max-height: 300px; overflow-y: auto; border: 1px solid var(--n-border-color); border-radius: 4px;"
          >
            <div 
              v-for="slot in availableSlots"
              :key="slot.id"
              class="slot-item"
              :class="{ 'slot-selected': selectedSlotId === slot.id, 'slot-occupied': slot.isOccupied }"
              style="padding: 12px; border-bottom: 1px solid var(--n-border-color); cursor: pointer;"
              @click="!slot.isOccupied && selectSlot(slot.id)"
            >
              <n-flex justify="space-between" align="center">
                <div>
                  <h5 style="font-size: 14px; margin-bottom: 4px; font-weight: 600;">{{ slot.name }}</h5>
                  <p style="font-size: 12px; opacity: 0.7; margin-bottom: 4px;">
                    Parent: {{ slot.parentProductName }}
                  </p>
                  <p style="font-size: 12px; margin: 0;">
                    <n-tag size="small" :type="slot.isOccupied ? 'error' : 'success'">
                      {{ slot.isOccupied ? 'Occupied' : 'Available' }}
                    </n-tag>
                  </p>
                </div>
                <span v-if="selectedSlotId === slot.id">
                  <n-icon :component="Icons.Check" />
                </span>
              </n-flex>
            </div>
          </div>
        </div>

        <div v-if="selectedSlotId">
          <h5 style="font-size: 14px; margin-bottom: 8px; font-weight: 600;">Position (optional)</h5>
          <n-flex :size="8">
            <n-input-number
              v-model:value="position.x"
              placeholder="X (mm)"
              style="flex: 1;"
            />
            <n-input-number
              v-model:value="position.y"
              placeholder="Y (mm)"
              style="flex: 1;"
            />
            <n-input-number
              v-model:value="position.z"
              placeholder="Z (mm)"
              style="flex: 1;"
            />
          </n-flex>
        </div>
      </div>

      <n-flex justify="space-between" :size="8" style="padding-top: 12px; border-top: 1px solid var(--n-border-color);">
        <n-button 
          text
          @click="selectedProductId = null"
        >
          <template #icon>
            <n-icon :component="Icons.ArrowBack" />
          </template>
          Back
        </n-button>
        <n-flex :size="8">
          <n-button 
            text
            @click="$emit('close')"
          >
            <template #icon>
              <n-icon :component="Icons.Close" />
            </template>
            Cancel
          </n-button>
          <n-button 
            v-if="availableSlots.length > 0"
            @click="confirmAddWithoutSlot"
          >
            <template #icon>
              <n-icon :component="Icons.Add" />
            </template>
            Add Without Slot
          </n-button>
          <n-button 
            v-if="selectedSlotId"
            type="primary"
            @click="confirmAddToSlot"
          >
            <template #icon>
              <n-icon :component="Icons.Add" />
            </template>
            Add to Slot
          </n-button>
          <n-button 
            v-else-if="availableSlots.length === 0"
            type="primary"
            @click="confirmAddWithoutSlot"
          >
            <template #icon>
              <n-icon :component="Icons.Add" />
            </template>
            Add Component
          </n-button>
        </n-flex>
      </n-flex>
    </div>
  </n-flex>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import { NFlex, NButton, NInput, NSpin, NCard, NTag, NInputNumber, NEmpty, NIcon } from 'naive-ui';
import { useCatalogStore } from '@/stores/catalogStore';
import { ProductCategory, categoryLabels, getCategoryFromBackend } from '@/api/catalog';
import { buildsApi, type AvailableSlot } from '@/api/builds';
import { Icons } from '@/utils/icons';

interface Props {
  buildId: string;
}

const props = defineProps<Props>();

const emit = defineEmits<{
  'part-selected': [productId: string];
  'part-selected-with-slot': [productId: string, slotId: string, position: { x: number; y: number; z: number }, rotation?: { x: number; y: number; z: number } | null];
  'close': [];
}>();

const catalogStore = useCatalogStore();
const categories = computed(() => Object.values(ProductCategory));
const categoryDisplayMap = computed(() => categoryLabels);

const searchQuery = ref('');
const selectedCategory = ref<string | null>(null);
const isLoading = ref(false);
const selectedProductId = ref<string | null>(null);
const selectedSlotId = ref<string | null>(null);
const loadingSlots = ref(false);
const availableSlots = ref<AvailableSlot[]>([]);
const position = ref({ x: 0, y: 0, z: 0 });

const filteredProducts = computed(() => {
  return catalogStore.products.filter(p => {
    const matchesSearch = p.name.toLowerCase().includes(searchQuery.value.toLowerCase());
    const productCategoryEnum = getCategoryFromBackend(p.categoryName);
    const matchesCategory = !selectedCategory.value || productCategoryEnum === selectedCategory.value;
    return matchesSearch && matchesCategory;
  });
});

const selectedProduct = computed(() => {
  return catalogStore.products.find(p => p.id === selectedProductId.value);
});

onMounted(() => {
  catalogStore.loadProducts();
});

watch(selectedProductId, async (newProductId) => {
  if (newProductId) {
    await loadAvailableSlots();
  }
});

async function loadAvailableSlots() {
  loadingSlots.value = true;
  try {
    availableSlots.value = await buildsApi.getAvailableSlots(props.buildId);
    
    // Filter slots that match the selected product category if needed
    const product = selectedProduct.value;
    if (product) {
      availableSlots.value = availableSlots.value.filter(
        slot => slot.allowedCategory.toLowerCase() === product.categoryName.toLowerCase() || 
                slot.allowedCategory === 'Any'
      );
    }
  } catch (error) {
    console.error('Failed to load available slots:', error);
    availableSlots.value = [];
  } finally {
    loadingSlots.value = false;
  }
}

function handleSearch() {
  catalogStore.setSearch(searchQuery.value);
}

function selectCategory(category: string) {
  selectedCategory.value = selectedCategory.value === category ? null : category;
  catalogStore.setCategory(selectedCategory.value);
}

function selectProduct(productId: string) {
  selectedProductId.value = productId;
}

function selectSlot(slotId: string) {
  selectedSlotId.value = slotId;
  
  // Pre-fill position with slot's absolute position
  const slot = availableSlots.value.find(s => s.id === slotId);
  if (slot) {
    position.value = {
      x: slot.absolutePosition.x,
      y: slot.absolutePosition.y,
      z: slot.absolutePosition.z
    };
  }
}

function confirmAddWithoutSlot() {
  if (selectedProductId.value) {
    emit('part-selected', selectedProductId.value);
  }
}

function confirmAddToSlot() {
  if (selectedProductId.value && selectedSlotId.value) {
    const selectedSlot = availableSlots.value.find(s => s.id === selectedSlotId.value);
    emit('part-selected-with-slot', selectedProductId.value, selectedSlotId.value, position.value, selectedSlot?.rotation);
  }
}
</script>

<style scoped>
.product-item {
  transition: background-color 0.2s ease;
}

.product-item:hover {
  background-color: var(--n-color-hover);
}

.slot-item {
  transition: background-color 0.2s ease, border-color 0.2s ease;
}

.slot-item:not(.slot-occupied):hover {
  background-color: var(--n-color-hover);
}

.slot-selected {
  background-color: var(--n-color-primary-hover);
  border-left: 3px solid var(--n-color-primary);
}

.slot-occupied {
  opacity: 0.5;
  cursor: not-allowed !important;
}
</style>
