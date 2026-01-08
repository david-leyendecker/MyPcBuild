<template>
  <div class="dimensions-editor">
    <v-row dense>
      <v-col cols="4">
        <v-text-field 
          v-model.number="dimensions.length"
          label="Length"
          type="number"
          suffix="mm"
        ></v-text-field>
      </v-col>
      <v-col cols="4">
        <v-text-field 
          v-model.number="dimensions.width"
          label="Width"
          type="number"
          suffix="mm"
        ></v-text-field>
      </v-col>
      <v-col cols="4">
        <v-text-field 
          v-model.number="dimensions.height"
          label="Height"
          type="number"
          suffix="mm"
        ></v-text-field>
      </v-col>
    </v-row>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';

interface Props {
  modelValue?: string;
}

const props = defineProps<Props>();
const emit = defineEmits<{
  'update:modelValue': [value: string]
}>();

const dimensions = ref({
  length: 0,
  width: 0,
  height: 0
});

// Initialize from modelValue
function parseDimensionsValue(value?: string) {
  if (value) {
    const parts = value.split(',');
    if (parts.length === 3) {
      return {
        length: parseFloat(parts[0] ?? '0') || 0,
        width: parseFloat(parts[1] ?? '0') || 0,
        height: parseFloat(parts[2] ?? '0') || 0
      };
    }
  }
  return { length: 0, width: 0, height: 0 };
}

dimensions.value = parseDimensionsValue(props.modelValue);

// Watch for changes and emit in format: "length,width,height"
watch(dimensions, (newDimensions) => {
  const value = `${newDimensions.length},${newDimensions.width},${newDimensions.height}`;
  emit('update:modelValue', value);
}, { deep: true });

// Watch for external changes
watch(() => props.modelValue, (newValue) => {
  dimensions.value = parseDimensionsValue(newValue);
});
</script>
