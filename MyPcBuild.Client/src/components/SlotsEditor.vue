<template>
  <div class="slots-editor">
    <div class="d-flex justify-space-between align-center mb-2">
      <p class="text-body-2 text-medium-emphasis ma-0">Define installation slots for this product</p>
      <v-btn 
        prepend-icon="mdi-plus"
        size="small"
        @click="addSlot"
      >
        Add Slot
      </v-btn>
    </div>

    <div v-if="slots.length === 0" class="text-center py-3" style="background-color: rgba(var(--v-theme-surface), 0.5); border-radius: 4px;">
      <p class="text-body-2 text-medium-emphasis ma-0">No slots defined. Click "Add Slot" to create one.</p>
    </div>

    <div v-else class="d-flex flex-column ga-2">
      <v-card 
        v-for="(slot, index) in slots"
        :key="index"
        variant="outlined"
      >
        <v-card-text class="pa-2">
          <div class="d-flex justify-space-between align-start">
            <div class="flex-grow-1">
              <v-row dense>
                <v-col cols="6">
                  <label class="text-caption font-weight-semibold d-block mb-1">Slot Name</label>
                  <v-text-field 
                    v-model="slot.name"
                    placeholder="e.g., PCIe x16"
                  ></v-text-field>
                </v-col>
                <v-col cols="6">
                  <label class="text-caption font-weight-semibold d-block mb-1">Allowed Category</label>
                  <v-select 
                    v-model="slot.allowedCategory"
                    :items="categories"
                    placeholder="Select category"
                  ></v-select>
                </v-col>
              </v-row>
              
              <v-row dense class="mt-2">
                <v-col cols="12">
                  <label class="text-caption font-weight-semibold d-block mb-1">Location (mm)</label>
                </v-col>
                <v-col cols="4">
                  <v-text-field 
                    v-model.number="slot.location.x"
                    label="X"
                    type="number"
                    suffix="mm"
                    density="compact"
                  ></v-text-field>
                </v-col>
                <v-col cols="4">
                  <v-text-field 
                    v-model.number="slot.location.y"
                    label="Y"
                    type="number"
                    suffix="mm"
                    density="compact"
                  ></v-text-field>
                </v-col>
                <v-col cols="4">
                  <v-text-field 
                    v-model.number="slot.location.z"
                    label="Z"
                    type="number"
                    suffix="mm"
                    density="compact"
                  ></v-text-field>
                </v-col>
              </v-row>
            </div>
            <v-btn 
              icon="mdi-delete"
              color="error"
              variant="text"
              size="small"
              @click="removeSlot(index)"
            ></v-btn>
          </div>
        </v-card-text>
      </v-card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue';
import { ProductCategory, categoryLabels } from '@/api/catalog';

interface SlotData {
  name: string;
  allowedCategory: string;
  location: {
    x: number;
    y: number;
    z: number;
  };
}

interface Props {
  modelValue?: string;
}

const props = defineProps<Props>();
const emit = defineEmits<{
  'update:modelValue': [value: string]
}>();

const categories = computed(() => 
  Object.values(ProductCategory).map(value => ({
    title: categoryLabels[value],
    value
  }))
);
const slots = ref<SlotData[]>([]);

// Parse slot data from JSON string
function parseSlotsValue(value?: string): SlotData[] {
  if (value) {
    try {
      const parsed = JSON.parse(value);
      if (Array.isArray(parsed)) {
        return parsed;
      }
    } catch {
      // Invalid JSON, return empty array
    }
  }
  return [];
}

slots.value = parseSlotsValue(props.modelValue);

function addSlot() {
  slots.value.push({
    name: '',
    allowedCategory: '',
    location: {
      x: 0,
      y: 0,
      z: 0
    }
  });
}

function removeSlot(index: number) {
  slots.value.splice(index, 1);
}

// Watch for changes and emit as JSON
watch(slots, (newSlots) => {
  emit('update:modelValue', JSON.stringify(newSlots));
}, { deep: true });

// Watch for external changes
watch(() => props.modelValue, (newValue) => {
  slots.value = parseSlotsValue(newValue);
});
</script>
