<template>
  <div class="d-flex flex-column ga-3">
    <div 
      v-for="field in fieldDefinitions"
      :key="field.name"
    >
      <label class="text-subtitle-2 font-weight-semibold mb-2 d-block">
        {{ formatFieldName(field.name) }}
        <span v-if="field.required" class="text-error">*</span>
        <span v-if="field.unit" class="text-medium-emphasis font-weight-regular">({{ field.unit }})</span>
      </label>

      <!-- Text input -->
      <v-text-field 
        v-if="field.type?.toLowerCase() === 'text'"
        v-model="localValues[field.name]"
        :placeholder="`Enter ${formatFieldName(field.name).toLowerCase()}`"
      ></v-text-field>

      <!-- Number input -->
      <v-text-field 
        v-else-if="field.type?.toLowerCase() === 'number'"
        v-model.number="localValues[field.name]"
        type="number"
        :placeholder="`Enter ${formatFieldName(field.name).toLowerCase()}`"
      ></v-text-field>

      <!-- Boolean checkbox -->
      <v-checkbox 
        v-else-if="field.type?.toLowerCase() === 'boolean'"
        v-model="localValues[field.name]"
      ></v-checkbox>

      <!-- Select dropdown -->
      <v-select 
        v-else-if="field.type?.toLowerCase() === 'select' && field.options"
        v-model="localValues[field.name]"
        :items="field.options"
        :placeholder="`Select ${formatFieldName(field.name).toLowerCase()}`"
      ></v-select>

      <!-- Multi-select -->
      <v-select 
        v-else-if="field.type?.toLowerCase() === 'multi-select' && field.options"
        v-model="localValues[field.name]"
        :items="field.options"
        :placeholder="`Select ${formatFieldName(field.name).toLowerCase()}`"
        multiple
        chips
      ></v-select>

      <!-- Dimensions editor -->
      <DimensionsEditor 
        v-else-if="field.type?.toLowerCase() === 'dimensions'"
        v-model="localValues[field.name]"
      />

      <!-- Slots editor -->
      <SlotsEditor 
        v-else-if="field.type?.toLowerCase() === 'slots'"
        v-model="localValues[field.name]"
      />

      <!-- Chambers editor -->
      <ChambersEditor 
        v-else-if="field.type?.toLowerCase() === 'chambers'"
        v-model="localValues[field.name]"
      />

      <!-- Fallback for unknown types -->
      <v-text-field 
        v-else
        v-model="localValues[field.name]"
        :placeholder="`Enter ${formatFieldName(field.name).toLowerCase()}`"
      ></v-text-field>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import type { FieldDefinition } from '@/api/catalog';
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

const localValues = ref<Record<string, any>>(convertToLocalValues(props.modelValue, props.fieldDefinitions));

function convertToLocalValues(values: Record<string, string>, fields: FieldDefinition[]): Record<string, any> {
  const converted: Record<string, any> = {};
  
  fields.forEach((field) => {
    const value = values[field.name];
    const fieldType = field.type?.toLowerCase();
    
    if (fieldType === 'boolean') {
      // Convert string to boolean
      converted[field.name] = value === 'true' || value === 'True';
    } else if (fieldType === 'number') {
      converted[field.name] = value ? Number(value) : null;
    } else if (fieldType === 'multi-select') {
      // Convert comma-separated string to array
      converted[field.name] = value ? value.split(',') : [];
    } else {
      converted[field.name] = value || '';
    }
  });
  
  return converted;
}

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
  localValues.value = convertToLocalValues(newValue, props.fieldDefinitions);
}, { deep: true });

function formatFieldName(name: string): string {
  // Convert camelCase to Title Case with spaces
  return name
    .replace(/([A-Z])/g, ' $1')
    .replace(/^./, (str) => str.toUpperCase())
    .trim();
}
</script>
