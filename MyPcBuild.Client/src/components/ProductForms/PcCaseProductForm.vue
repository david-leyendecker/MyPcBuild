<template>
  <n-flex vertical :size="12">
    <!-- Form Factor and Color - Side by side -->
    <n-flex :size="12">
      <div style="flex: 1; min-width: 150px;">
        <label style="display: block; margin-bottom: 4px; font-size: 14px;">Form Factor</label>
        <n-input 
          v-model:value="localProduct.formFactor"
          :disabled="!editable"
          placeholder="e.g., Mid Tower, Full Tower"
        />
      </div>
      <div style="flex: 1; min-width: 150px;">
        <label style="display: block; margin-bottom: 4px; font-size: 14px;">Color</label>
        <n-input 
          v-model:value="localProduct.color"
          :disabled="!editable"
          placeholder="e.g., Black, White"
        />
      </div>
    </n-flex>

    <!-- Side Panel Window -->
    <n-flex :size="12">
      <div style="flex: 1; min-width: 150px;">
        <label style="display: block; margin-bottom: 4px; font-size: 14px;">Side Panel Window</label>
        <n-input 
          v-model:value="localProduct.sidePanelWindow"
          :disabled="!editable"
          placeholder="e.g., Tempered Glass, Acrylic, None"
        />
      </div>
    </n-flex>

    <!-- Dimensions -->
    <n-flex :size="12">
      <div style="flex: 1; min-width: 150px;">
        <label style="display: block; margin-bottom: 4px; font-size: 14px;">Dimensions</label>
        <DimensionsInput 
          v-model="localProduct.dimensions"
          :editable="editable"
        />
      </div>
    </n-flex>

    <!-- Chambers -->
    <n-flex :size="12">
      <div style="flex: 1; min-width: 150px;">
        <ChambersInput 
          v-model="localProduct.chambers"
          :editable="editable"
        />
      </div>
    </n-flex>
  </n-flex>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { NFlex, NInput } from 'naive-ui';
import type { PcCaseProductRequest, PcCaseProductResponse } from '@/types/products';
import DimensionsInput from '@/components/ValueObjects/DimensionsInput.vue';
import ChambersInput from '@/components/ValueObjects/ChambersInput.vue';

interface Props {
  modelValue: Partial<PcCaseProductRequest> | Partial<PcCaseProductResponse>;
  editable?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  editable: true
});

const emit = defineEmits<{
  'update:modelValue': [value: Partial<PcCaseProductRequest>]
}>();

const localProduct = ref<Partial<PcCaseProductRequest>>({
  formFactor: props.modelValue.formFactor ?? 'Mid Tower',
  color: props.modelValue.color ?? 'Black',
  sidePanelWindow: props.modelValue.sidePanelWindow ?? 'Tempered Glass',
  dimensions: props.modelValue.dimensions ?? { length: 450, width: 210, height: 450 },
  chambers: props.modelValue.chambers ?? []
});

watch(
  () => props.modelValue,
  (newValue) => {
    Object.assign(localProduct.value, {
      formFactor: newValue.formFactor ?? 'Mid Tower',
      color: newValue.color ?? 'Black',
      sidePanelWindow: newValue.sidePanelWindow ?? 'Tempered Glass',
      dimensions: newValue.dimensions ?? { length: 450, width: 210, height: 450 },
      chambers: newValue.chambers ?? []
    });
  },
  { deep: true }
);

watch(
  localProduct,
  (newValue) => {
    emit('update:modelValue', newValue);
  },
  { deep: true }
);
</script>
