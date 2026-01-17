<template>
  <div class="d-flex flex-column ga-3">
    <p class="text-body-2 text-medium-emphasis">Add a component to your build with optional slot placement</p>

    <!-- Product Selection -->
    <div v-if="!selectedProductId">
      <div class="d-flex flex-column ga-3">
        <v-text-field 
          v-model="searchQuery"
          placeholder="Search components..."
          @keyup.enter="handleSearch"
        ></v-text-field>
        <div class="d-flex flex-wrap ga-2">
          <v-btn 
            v-for="category in categories"
            :key="category"
            :variant="selectedCategory === category ? 'elevated' : 'outlined'"
            size="small"
            @click="selectCategory(category)"
          >
            {{ categoryDisplayMap[category] }}
          </v-btn>
        </div>
      </div>

      <div v-if="isLoading" class="d-flex justify-center py-4">
        <v-progress-circular indeterminate color="primary"></v-progress-circular>
      </div>

      <div v-else-if="filteredProducts.length === 0" class="text-center py-4">
        <p class="text-medium-emphasis">No components found</p>
      </div>

      <div 
        v-else 
        class="overflow-y-auto mt-3"
        style="max-height: 400px; border: 1px solid rgba(var(--v-border-color), var(--v-border-opacity)); border-radius: 4px;"
      >
        <div 
          v-for="product in filteredProducts"
          :key="product.id"
          class="pa-3 product-item"
          style="border-bottom: 1px solid rgba(var(--v-border-color), var(--v-border-opacity)); cursor: pointer;"
          @click="selectProduct(product.id)"
        >
          <div class="d-flex justify-space-between align-center">
            <div>
              <h4 class="text-subtitle-1 mb-1">{{ product.name }}</h4>
              <p class="text-success font-weight-semibold text-body-2">${{ product.price.toFixed(2) }}</p>
            </div>
            <v-icon>mdi-arrow-right</v-icon>
          </div>
        </div>
      </div>

      <div class="d-flex justify-end ga-2 pt-3" style="border-top: 1px solid rgba(var(--v-border-color), var(--v-border-opacity));">
        <v-btn 
          prepend-icon="mdi-close"
          variant="text"
          @click="$emit('close')"
        >
          Cancel
        </v-btn>
      </div>
    </div>

    <!-- Slot Selection (optional) -->
    <div v-else>
      <div class="mb-3">
        <h4 class="text-subtitle-1 mb-2">Selected: {{ selectedProduct?.name }}</h4>
        <p class="text-body-2 text-medium-emphasis">
          Choose a slot for placement (optional) or add without slot assignment
        </p>
      </div>

      <v-card v-if="loadingSlots" class="pa-4">
        <v-progress-circular indeterminate color="primary"></v-progress-circular>
      </v-card>

      <div v-else>
        <div v-if="availableSlots.length > 0" class="mb-4">
          <h5 class="text-subtitle-2 mb-2">Available Slots</h5>
          <div 
            class="overflow-y-auto"
            style="max-height: 300px; border: 1px solid rgba(var(--v-border-color), var(--v-border-opacity)); border-radius: 4px;"
          >
            <div 
              v-for="slot in availableSlots"
              :key="slot.id"
              class="pa-3 slot-item"
              :class="{ 'slot-selected': selectedSlotId === slot.id, 'slot-occupied': slot.isOccupied }"
              style="border-bottom: 1px solid rgba(var(--v-border-color), var(--v-border-opacity)); cursor: pointer;"
              @click="!slot.isOccupied && selectSlot(slot.id)"
            >
              <div class="d-flex justify-space-between align-center">
                <div>
                  <h5 class="text-subtitle-2 mb-1">{{ slot.name }}</h5>
                  <p class="text-caption text-medium-emphasis">
                    Parent: {{ slot.parentProductName }}
                  </p>
                  <p class="text-caption">
                    <v-chip size="x-small" :color="slot.isOccupied ? 'error' : 'success'">
                      {{ slot.isOccupied ? 'Occupied' : 'Available' }}
                    </v-chip>
                  </p>
                </div>
                <v-icon v-if="selectedSlotId === slot.id">mdi-check-circle</v-icon>
              </div>
            </div>
          </div>
        </div>

        <div v-if="selectedSlotId">
          <h5 class="text-subtitle-2 mb-2">Position (optional)</h5>
          <v-row>
            <v-col cols="4">
              <v-text-field
                v-model.number="position.x"
                label="X (mm)"
                type="number"
                density="compact"
              ></v-text-field>
            </v-col>
            <v-col cols="4">
              <v-text-field
                v-model.number="position.y"
                label="Y (mm)"
                type="number"
                density="compact"
              ></v-text-field>
            </v-col>
            <v-col cols="4">
              <v-text-field
                v-model.number="position.z"
                label="Z (mm)"
                type="number"
                density="compact"
              ></v-text-field>
            </v-col>
          </v-row>
        </div>
      </div>

      <div class="d-flex justify-space-between ga-2 pt-3" style="border-top: 1px solid rgba(var(--v-border-color), var(--v-border-opacity));">
        <v-btn 
          prepend-icon="mdi-arrow-left"
          variant="text"
          @click="selectedProductId = null"
        >
          Back
        </v-btn>
        <div class="d-flex ga-2">
          <v-btn 
            prepend-icon="mdi-close"
            variant="text"
            @click="$emit('close')"
          >
            Cancel
          </v-btn>
          <v-btn 
            v-if="availableSlots.length > 0"
            prepend-icon="mdi-plus"
            variant="outlined"
            @click="confirmAddWithoutSlot"
          >
            Add Without Slot
          </v-btn>
          <v-btn 
            v-if="selectedSlotId"
            prepend-icon="mdi-plus"
            color="primary"
            @click="confirmAddToSlot"
          >
            Add to Slot
          </v-btn>
          <v-btn 
            v-else-if="availableSlots.length === 0"
            prepend-icon="mdi-plus"
            color="primary"
            @click="confirmAddWithoutSlot"
          >
            Add Component
          </v-btn>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import { useCatalogStore } from '@/stores/catalogStore';
import { ProductCategory, categoryLabels, getCategoryFromBackend } from '@/api/catalog';
import { buildsApi, type AvailableSlot } from '@/api/builds';

interface Props {
  buildId: string;
}

const props = defineProps<Props>();

const emit = defineEmits<{
  'part-selected': [productId: string];
  'part-selected-with-slot': [productId: string, slotId: string, position: { x: number; y: number; z: number }];
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
    emit('part-selected-with-slot', selectedProductId.value, selectedSlotId.value, position.value);
  }
}
</script>

<style scoped>
.product-item {
  transition: background-color 0.2s ease;
}

.product-item:hover {
  background-color: rgba(var(--v-theme-on-surface), 0.05);
}

.slot-item {
  transition: background-color 0.2s ease, border-color 0.2s ease;
}

.slot-item:not(.slot-occupied):hover {
  background-color: rgba(var(--v-theme-on-surface), 0.05);
}

.slot-selected {
  background-color: rgba(var(--v-theme-primary), 0.1);
  border-left: 3px solid rgb(var(--v-theme-primary));
}

.slot-occupied {
  opacity: 0.5;
  cursor: not-allowed !important;
}
</style>
