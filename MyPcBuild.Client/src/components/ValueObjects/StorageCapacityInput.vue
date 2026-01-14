<template>
  <v-text-field 
    :model-value="displayValue"
    @update:model-value="handleInput"
    :label="label"
    :readonly="!editable"
    type="number"
    suffix="GB"
    :variant="editable ? 'filled' : 'outlined'"
    density="comfortable"
  ></v-text-field>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import type { StorageCapacity } from '@/types/products';

interface Props {
  modelValue: StorageCapacity | null | undefined;
  label?: string;
  editable?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  label: 'Capacity',
  editable: true
});

const emit = defineEmits<{
  'update:modelValue': [value: StorageCapacity]
}>();

const displayValue = computed(() => {
  return props.modelValue?.valueInGB ?? 0;
});

function handleInput(value: string | number) {
  const numValue = typeof value === 'string' ? parseInt(value) : value;
  emit('update:modelValue', { valueInGB: numValue || 0 });
}
</script>
