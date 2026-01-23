<template>
  <div class="dimensions-input">
    <n-flex :size="12">
      <n-input-number 
        :value="dimensions.length"
        @update:value="updateLength"
        placeholder="Length"
        :readonly="!editable"
        style="flex: 1; min-width: 100px;"
      >
        <template #suffix>mm</template>
      </n-input-number>
      <n-input-number 
        :value="dimensions.width"
        @update:value="updateWidth"
        placeholder="Width"
        :readonly="!editable"
        style="flex: 1; min-width: 100px;"
      >
        <template #suffix>mm</template>
      </n-input-number>
      <n-input-number 
        :value="dimensions.height"
        @update:value="updateHeight"
        placeholder="Height"
        :readonly="!editable"
        style="flex: 1; min-width: 100px;"
      >
        <template #suffix>mm</template>
      </n-input-number>
    </n-flex>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { NInputNumber, NFlex } from 'naive-ui';
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
