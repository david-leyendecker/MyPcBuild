<template>
  <n-grid :cols="3" :x-gap="12">
    <n-form-item-gi label="Length">
      <n-input-number :value="dimensions.length" @update:value="updateLength" placeholder="Length" :disabled="!editable" style="width: 100%">
        <template #suffix>mm</template>
      </n-input-number>
    </n-form-item-gi>
    <n-form-item-gi label="Width">
      <n-input-number :value="dimensions.width" @update:value="updateWidth" placeholder="Width" :disabled="!editable" style="width: 100%">
        <template #suffix>mm</template>
      </n-input-number>
    </n-form-item-gi>
    <n-form-item-gi label="Height">
      <n-input-number :value="dimensions.height" @update:value="updateHeight" placeholder="Height" :disabled="!editable" style="width: 100%">
        <template #suffix>mm</template>
      </n-input-number>
    </n-form-item-gi>
  </n-grid>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { NInputNumber, NFormItemGi, NGrid } from 'naive-ui';
import type { Dimensions } from '@/types/products';

interface Props {
  modelValue: Dimensions | null | undefined;
  editable?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  editable: true
});

const emit = defineEmits<{
  'update:modelValue': [value: Dimensions]
}>();

const dimensions = computed(() => {
  return props.modelValue ?? { length: 0, width: 0, height: 0 };
});

function updateLength(value: number | null) {
  emit('update:modelValue', {
    ...dimensions.value,
    length: value || 0
  });
}

function updateWidth(value: number | null) {
  emit('update:modelValue', {
    ...dimensions.value,
    width: value || 0
  });
}

function updateHeight(value: number | null) {
  emit('update:modelValue', {
    ...dimensions.value,
    height: value || 0
  });
}
</script>
