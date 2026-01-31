<template>
  <n-input-number 
    :value="displayValue"
    @update:value="handleInput"
    :placeholder="label"
    :readonly="!editable"
    style="width: 100%"
  >
    <template #suffix>W</template>
  </n-input-number>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { NInputNumber } from 'naive-ui';
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

function handleInput(value: number | null) {
  emit('update:modelValue', { valueInWatts: value || 0 });
}
</script>
