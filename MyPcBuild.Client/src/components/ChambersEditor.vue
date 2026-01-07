<template>
  <div class="chambers-editor">
    <div class="flex justify-content-between align-items-center mb-2">
      <p class="text-sm text-500 m-0">Define chambers (compartments) for this case</p>
      <Button 
        label="Add Chamber"
        icon="pi pi-plus"
        @click="addChamber"
        size="small"
      />
    </div>

    <div v-if="chambers.length === 0" class="text-center py-3 surface-ground border-round">
      <p class="text-sm text-500 m-0">No chambers defined. Click "Add Chamber" to create one.</p>
    </div>

    <div v-else class="flex flex-column gap-2">
      <Card 
        v-for="(chamber, index) in chambers"
        :key="index"
        class="p-2"
      >
        <template #content>
          <div class="flex justify-content-between align-items-start mb-2">
            <h5 class="m-0">Chamber {{ index + 1 }}</h5>
            <Button 
              icon="pi pi-trash"
              @click="removeChamber(index)"
              severity="danger"
              text
              size="small"
            />
          </div>

          <div class="grid">
            <div class="col-12">
              <label class="text-xs font-semibold">Chamber Name</label>
              <InputText 
                v-model="chamber.name"
                placeholder="e.g., Main Compartment"
                class="w-full"
                size="small"
              />
            </div>
            <div class="col-4">
              <label class="text-xs font-semibold">Length (mm)</label>
              <InputNumber 
                v-model="chamber.length"
                placeholder="mm"
                class="w-full"
                :min="0"
              />
            </div>
            <div class="col-4">
              <label class="text-xs font-semibold">Width (mm)</label>
              <InputNumber 
                v-model="chamber.width"
                placeholder="mm"
                class="w-full"
                :min="0"
              />
            </div>
            <div class="col-4">
              <label class="text-xs font-semibold">Height (mm)</label>
              <InputNumber 
                v-model="chamber.height"
                placeholder="mm"
                class="w-full"
                :min="0"
              />
            </div>
          </div>
        </template>
      </Card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import Button from 'primevue/button';
import Card from 'primevue/card';
import InputText from 'primevue/inputtext';
import InputNumber from 'primevue/inputnumber';

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

<style scoped>
.chambers-editor label {
  display: block;
  margin-bottom: 0.25rem;
}
</style>
