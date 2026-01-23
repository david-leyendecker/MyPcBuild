<template>
  <n-input-number 
    :value="displayValue"
    @update:value="handleInput"
    :placeholder="label"
    :readonly="!editable"
    :step="0.1"
  >
    <template #suffix>V</template>
  </n-input-number>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { NInputNumber } from 'naive-ui';
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

function handleInput(value: number | null) {
  emit('update:modelValue', { valueInVolts: value || 0 });
}
</script>
