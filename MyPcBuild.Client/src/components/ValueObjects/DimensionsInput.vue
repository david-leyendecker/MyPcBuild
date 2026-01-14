<template>
  <div class="dimensions-input">
    <v-row dense>
      <v-col cols="4">
        <v-text-field 
          :model-value="dimensions.length"
          @update:model-value="updateLength"
          label="Length"
          :readonly="!editable"
          type="number"
          suffix="mm"
          :variant="editable ? 'filled' : 'outlined'"
          density="comfortable"
        ></v-text-field>
      </v-col>
      <v-col cols="4">
        <v-text-field 
          :model-value="dimensions.width"
          @update:model-value="updateWidth"
          label="Width"
          :readonly="!editable"
          type="number"
          suffix="mm"
          :variant="editable ? 'filled' : 'outlined'"
          density="comfortable"
        ></v-text-field>
      </v-col>
      <v-col cols="4">
        <v-text-field 
          :model-value="dimensions.height"
          @update:model-value="updateHeight"
          label="Height"
          :readonly="!editable"
          type="number"
          suffix="mm"
          :variant="editable ? 'filled' : 'outlined'"
          density="comfortable"
        ></v-text-field>
      </v-col>
    </v-row>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
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

function updateLength(value: string | number) {
  const numValue = typeof value === 'string' ? parseFloat(value) : value;
  emit('update:modelValue', {
    ...dimensions.value,
    length: numValue || 0
  });
}

function updateWidth(value: string | number) {
  const numValue = typeof value === 'string' ? parseFloat(value) : value;
  emit('update:modelValue', {
    ...dimensions.value,
    width: numValue || 0
  });
}

function updateHeight(value: string | number) {
  const numValue = typeof value === 'string' ? parseFloat(value) : value;
  emit('update:modelValue', {
    ...dimensions.value,
    height: numValue || 0
  });
}
</script>
