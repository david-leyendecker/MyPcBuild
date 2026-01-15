<template>
  <v-text-field 
    :model-value="displayValue"
    @update:model-value="handleInput"
    :label="label"
    :readonly="!editable"
    type="number"
    suffix="mm"
    :variant="editable ? 'filled' : 'outlined'"
    density="comfortable"
  ></v-text-field>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import type { Length } from '@/types/products';

interface Props {
  modelValue: Length | null | undefined;
  label?: string;
  editable?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  label: 'Length',
  editable: true
});

const emit = defineEmits<{
  'update:modelValue': [value: Length]
}>();

const displayValue = computed(() => {
  return props.modelValue?.valueInMm ?? 0;
});

function handleInput(value: string | number) {
  const numValue = typeof value === 'string' ? parseInt(value) : value;
  emit('update:modelValue', { valueInMm: numValue || 0 });
}
</script>
