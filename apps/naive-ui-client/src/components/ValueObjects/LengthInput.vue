<template>
  <n-input-number 
    :value="displayValue"
    @update:value="handleInput"
    :placeholder="label"
    :readonly="!editable"
    style="width: 100%"
  >
    <template #suffix>mm</template>
  </n-input-number>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { NInputNumber } from 'naive-ui';
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

function handleInput(value: number | null) {
  emit('update:modelValue', { valueInMm: value || 0 });
}
</script>
