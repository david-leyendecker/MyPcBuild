<template>
  <v-text-field 
    :model-value="displayValue"
    @update:model-value="handleInput"
    :label="label"
    :readonly="!editable"
    type="number"
    suffix="W"
    :variant="editable ? 'filled' : 'outlined'"
    density="comfortable"
  ></v-text-field>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import type { Power } from '@/types/products';

interface Props {
  modelValue: Power | null | undefined;
  label?: string;
  editable?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  label: 'Power',
  editable: true
});

const emit = defineEmits<{
  'update:modelValue': [value: Power]
}>();

const displayValue = computed(() => {
  return props.modelValue?.valueInWatts ?? 0;
});

function handleInput(value: string | number) {
  const numValue = typeof value === 'string' ? parseInt(value) : value;
  emit('update:modelValue', { valueInWatts: numValue || 0 });
}
</script>
