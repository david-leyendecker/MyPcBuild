<template>
  <div class="chambers-editor">
    <div class="d-flex justify-space-between align-center mb-2">
      <p class="text-body-2 text-medium-emphasis ma-0">Define chambers (compartments) for this case</p>
      <v-btn 
        prepend-icon="mdi-plus"
        size="small"
        @click="addChamber"
      >
        Add Chamber
      </v-btn>
    </div>

    <div v-if="chambers.length === 0" class="text-center py-3" style="background-color: rgba(var(--v-theme-surface), 0.5); border-radius: 4px;">
      <p class="text-body-2 text-medium-emphasis ma-0">No chambers defined. Click "Add Chamber" to create one.</p>
    </div>

    <div v-else class="d-flex flex-column ga-2">
      <v-card 
        v-for="(chamber, index) in chambers"
        :key="index"
        variant="outlined"
      >
        <v-card-text class="pa-2">
          <div class="d-flex justify-space-between align-start mb-2">
            <h5 class="text-subtitle-2 ma-0">Chamber {{ index + 1 }}</h5>
            <v-btn 
              icon="mdi-delete"
              color="error"
              variant="text"
              size="small"
              @click="removeChamber(index)"
            ></v-btn>
          </div>

          <v-row dense>
            <v-col cols="12">
              <label class="text-caption font-weight-semibold d-block mb-1">Chamber Name</label>
              <v-text-field 
                v-model="chamber.name"
                placeholder="e.g., Main Compartment"
                variant="outlined"
                density="compact"
                hide-details
              ></v-text-field>
            </v-col>
            <v-col cols="4">
              <label class="text-caption font-weight-semibold d-block mb-1">Length (mm)</label>
              <v-text-field 
                v-model.number="chamber.length"
                type="number"
                placeholder="mm"
                variant="outlined"
                density="compact"
                hide-details
              ></v-text-field>
            </v-col>
            <v-col cols="4">
              <label class="text-caption font-weight-semibold d-block mb-1">Width (mm)</label>
              <v-text-field 
                v-model.number="chamber.width"
                type="number"
                placeholder="mm"
                variant="outlined"
                density="compact"
                hide-details
              ></v-text-field>
            </v-col>
            <v-col cols="4">
              <label class="text-caption font-weight-semibold d-block mb-1">Height (mm)</label>
              <v-text-field 
                v-model.number="chamber.height"
                type="number"
                placeholder="mm"
                variant="outlined"
                density="compact"
                hide-details
              ></v-text-field>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';

interface ChamberData {
  name: string;
  length: number;
  width: number;
  height: number;
}

interface Props {
  modelValue?: string;
}

const props = defineProps<Props>();
const emit = defineEmits<{
  'update:modelValue': [value: string]
}>();

const chambers = ref<ChamberData[]>([]);

// Parse chamber data from JSON string
function parseChambersValue(value?: string): ChamberData[] {
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

chambers.value = parseChambersValue(props.modelValue);

function addChamber() {
  chambers.value.push({
    name: '',
    length: 0,
    width: 0,
    height: 0
  });
}

function removeChamber(index: number) {
  chambers.value.splice(index, 1);
}

// Watch for changes and emit as JSON
watch(chambers, (newChambers) => {
  emit('update:modelValue', JSON.stringify(newChambers));
}, { deep: true });

// Watch for external changes
watch(() => props.modelValue, (newValue) => {
  chambers.value = parseChambersValue(newValue);
});
</script>
