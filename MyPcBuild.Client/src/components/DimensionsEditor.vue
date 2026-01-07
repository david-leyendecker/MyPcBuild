<template>
  <div class="dimensions-editor">
    <div class="grid">
      <div class="col-4">
        <label for="length" class="text-sm">Length</label>
        <InputNumber 
          id="length"
          v-model="dimensions.length"
          placeholder="mm"
          class="w-full"
          :min="0"
        />
      </div>
      <div class="col-4">
        <label for="width" class="text-sm">Width</label>
        <InputNumber 
          id="width"
          v-model="dimensions.width"
          placeholder="mm"
          class="w-full"
          :min="0"
        />
      </div>
      <div class="col-4">
        <label for="height" class="text-sm">Height</label>
        <InputNumber 
          id="height"
          v-model="dimensions.height"
          placeholder="mm"
          class="w-full"
          :min="0"
        />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import InputNumber from 'primevue/inputnumber';

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

// Parse initial value
if (props.modelValue) {
  const parts = props.modelValue.split(',');
  if (parts.length === 3) {
    dimensions.value = {
      length: parseFloat(parts[0] ?? '0') || 0,
      width: parseFloat(parts[1] ?? '0') || 0,
      height: parseFloat(parts[2] ?? '0') || 0
    };
  }
}

// Watch for changes and emit in format: "length,width,height"
watch(dimensions, (newDimensions) => {
  const value = `${newDimensions.length},${newDimensions.width},${newDimensions.height}`;
  emit('update:modelValue', value);
}, { deep: true });

// Watch for external changes
watch(() => props.modelValue, (newValue) => {
  if (newValue) {
    const parts = newValue.split(',');
    if (parts.length === 3) {
      dimensions.value = {
        length: parseFloat(parts[0] ?? '0') || 0,
        width: parseFloat(parts[1] ?? '0') || 0,
        height: parseFloat(parts[2] ?? '0') || 0
      };
    }
  }
});
</script>

<style scoped>
.dimensions-editor label {
  display: block;
  margin-bottom: 0.25rem;
  font-weight: 500;
}
</style>
