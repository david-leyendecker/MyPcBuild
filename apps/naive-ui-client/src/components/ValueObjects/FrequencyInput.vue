<template>
  <n-input-number 
    :value="displayValue"
    @update:value="handleInput"
    :placeholder="label"
    :readonly="!editable"
    :step="0.1"
    style="width: 100%"
  >
    <template #suffix>GHz</template>
  </n-input-number>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { NInputNumber } from 'naive-ui';
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

function handleInput(value: number | null) {
  emit('update:modelValue', { valueInGHz: value || 0 });
}
</script>
