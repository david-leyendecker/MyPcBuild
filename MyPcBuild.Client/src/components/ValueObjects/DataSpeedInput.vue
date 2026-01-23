<template>
  <n-input-number 
    :value="displayValue"
    @update:value="handleInput"
    :placeholder="label"
    :readonly="!editable"
  >
    <template #suffix>MB/s</template>
  </n-input-number>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { NInputNumber } from 'naive-ui';
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

function handleInput(value: number | null) {
  emit('update:modelValue', { valueInMBps: value || 0 });
}
</script>
