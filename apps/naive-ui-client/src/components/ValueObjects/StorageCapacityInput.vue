<template>
  <n-input-number 
    :value="displayValue"
    @update:value="handleInput"
    :placeholder="label"
    :readonly="!editable"
    style="width: 100%"
  >
    <template #suffix>GB</template>
  </n-input-number>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { NInputNumber } from 'naive-ui';
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

function handleInput(value: number | null) {
  emit('update:modelValue', { valueInGB: value || 0 });
}
</script>
