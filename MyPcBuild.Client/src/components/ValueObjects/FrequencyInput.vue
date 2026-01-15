<template>
  <v-text-field 
    :model-value="displayValue"
    @update:model-value="handleInput"
    :label="label"
    :readonly="!editable"
    type="number"
    step="0.1"
    suffix="GHz"
    :variant="editable ? 'filled' : 'outlined'"
    density="comfortable"
  ></v-text-field>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import type { Frequency } from '@/types/products';

interface Props {
  modelValue: Frequency | null | undefined;
  label?: string;
  editable?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  label: 'Frequency',
  editable: true
});

const emit = defineEmits<{
  'update:modelValue': [value: Frequency]
}>();

const displayValue = computed(() => {
  return props.modelValue?.valueInGHz ?? 0;
});

function handleInput(value: string | number) {
  const numValue = typeof value === 'string' ? parseFloat(value) : value;
  emit('update:modelValue', { valueInGHz: numValue || 0 });
}
</script>
