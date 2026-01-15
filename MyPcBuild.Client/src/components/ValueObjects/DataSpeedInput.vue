<template>
  <v-text-field 
    :model-value="displayValue"
    @update:model-value="handleInput"
    :label="label"
    :readonly="!editable"
    type="number"
    suffix="MB/s"
    :variant="editable ? 'filled' : 'outlined'"
    density="comfortable"
  ></v-text-field>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import type { DataSpeed } from '@/types/products';

interface Props {
  modelValue: DataSpeed | null | undefined;
  label?: string;
  editable?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  label: 'Speed',
  editable: true
});

const emit = defineEmits<{
  'update:modelValue': [value: DataSpeed]
}>();

const displayValue = computed(() => {
  return props.modelValue?.valueInMBps ?? 0;
});

function handleInput(value: string | number) {
  const numValue = typeof value === 'string' ? parseInt(value) : value;
  emit('update:modelValue', { valueInMBps: numValue || 0 });
}
</script>
