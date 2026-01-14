<template>
  <v-text-field 
    :model-value="displayValue"
    @update:model-value="handleInput"
    :label="label"
    :readonly="!editable"
    type="number"
    step="0.1"
    suffix="V"
    :variant="editable ? 'filled' : 'outlined'"
    density="comfortable"
  ></v-text-field>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import type { Voltage } from '@/types/products';

interface Props {
  modelValue: Voltage | null | undefined;
  label?: string;
  editable?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  label: 'Voltage',
  editable: true
});

const emit = defineEmits<{
  'update:modelValue': [value: Voltage]
}>();

const displayValue = computed(() => {
  return props.modelValue?.valueInVolts ?? 0;
});

function handleInput(value: string | number) {
  const numValue = typeof value === 'string' ? parseFloat(value) : value;
  emit('update:modelValue', { valueInVolts: numValue || 0 });
}
</script>
