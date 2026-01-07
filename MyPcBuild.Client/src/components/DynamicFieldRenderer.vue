<template>
  <div class="flex flex-column gap-3">
    <div 
      v-for="field in fieldDefinitions"
      :key="field.name"
      class="field"
    >
      <label :for="field.name" class="font-semibold">
        {{ formatFieldName(field.name) }}
        <span v-if="field.required" class="text-red-500">*</span>
        <span v-if="field.unit" class="text-500 font-normal">({{ field.unit }})</span>
      </label>

      <!-- Text input -->
      <InputText 
        v-if="field.type === 'text'"
        :id="field.name"
        v-model="localValues[field.name]"
        :placeholder="`Enter ${formatFieldName(field.name).toLowerCase()}`"
        class="w-full"
      />

      <!-- Number input -->
      <InputNumber 
        v-else-if="field.type === 'number'"
        :id="field.name"
        v-model="localValues[field.name]"
        :placeholder="`Enter ${formatFieldName(field.name).toLowerCase()}`"
        class="w-full"
        :min="0"
      />

      <!-- Boolean checkbox -->
      <Checkbox 
        v-else-if="field.type === 'boolean'"
        :id="field.name"
        v-model="localValues[field.name]"
        :binary="true"
      />

      <!-- Select dropdown -->
      <Select 
        v-else-if="field.type === 'select' && field.options"
        :id="field.name"
        v-model="localValues[field.name]"
        :options="field.options"
        :placeholder="`Select ${formatFieldName(field.name).toLowerCase()}`"
        class="w-full"
      />

      <!-- Multi-select -->
      <MultiSelect 
        v-else-if="field.type === 'multi-select' && field.options"
        :id="field.name"
        v-model="localValues[field.name]"
        :options="field.options"
        :placeholder="`Select ${formatFieldName(field.name).toLowerCase()}`"
        class="w-full"
      />

      <!-- Dimensions editor -->
      <DimensionsEditor 
        v-else-if="field.type === 'dimensions'"
        :id="field.name"
        v-model="localValues[field.name]"
      />

      <!-- Slots editor -->
      <SlotsEditor 
        v-else-if="field.type === 'slots'"
        :id="field.name"
        v-model="localValues[field.name]"
      />

      <!-- Chambers editor -->
      <ChambersEditor 
        v-else-if="field.type === 'chambers'"
        :id="field.name"
        v-model="localValues[field.name]"
      />

      <!-- Fallback for unknown types -->
      <InputText 
        v-else
        :id="field.name"
        v-model="localValues[field.name]"
        :placeholder="`Enter ${formatFieldName(field.name).toLowerCase()}`"
        class="w-full"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import type { FieldDefinition } from '@/api/catalog';
import InputText from 'primevue/inputtext';
import InputNumber from 'primevue/inputnumber';
import Checkbox from 'primevue/checkbox';
import Select from 'primevue/select';
import MultiSelect from 'primevue/multiselect';
import DimensionsEditor from './DimensionsEditor.vue';
import SlotsEditor from './SlotsEditor.vue';
import ChambersEditor from './ChambersEditor.vue';

interface Props {
  fieldDefinitions: FieldDefinition[];
  modelValue: Record<string, string>;
}

const props = defineProps<Props>();
const emit = defineEmits<{
  'update:modelValue': [value: Record<string, string>]
}>();

const localValues = ref<Record<string, any>>({ ...props.modelValue });

// Watch for changes in local values and emit updates
watch(localValues, (newValues) => {
  const stringValues: Record<string, string> = {};
  
  Object.entries(newValues).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== '') {
      if (Array.isArray(value)) {
        // For multi-select, join with commas
        stringValues[key] = value.join(',');
      } else if (typeof value === 'boolean') {
        stringValues[key] = value.toString();
      } else if (typeof value === 'number') {
        stringValues[key] = value.toString();
      } else {
        stringValues[key] = String(value);
      }
    }
  });
  
  emit('update:modelValue', stringValues);
}, { deep: true });

// Watch for external changes to modelValue
watch(() => props.modelValue, (newValue) => {
  localValues.value = { ...newValue };
}, { deep: true });

function formatFieldName(name: string): string {
  // Convert camelCase to Title Case with spaces
  return name
    .replace(/([A-Z])/g, ' $1')
    .replace(/^./, (str) => str.toUpperCase())
    .trim();
}
</script>

<style scoped>
.field {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}
</style>
