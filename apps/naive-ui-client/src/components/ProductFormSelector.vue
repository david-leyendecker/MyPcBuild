<template>
  <component 
    :is="formComponent" 
    v-if="formComponent"
    v-model="localValue"
    :editable="editable"
  />
  <n-empty v-else :description="`No form available for category: ${category}`" />
</template>

<script setup lang="ts">
import { ref, computed, watch, type Component } from 'vue';
import { NEmpty } from 'naive-ui';
import { ProductCategory } from '@/api/catalog';
import type { ProductRequest, ProductResponse } from '@/types/products';
import CpuProductForm from './ProductForms/CpuProductForm.vue';
import GpuProductForm from './ProductForms/GpuProductForm.vue';
import MotherboardProductForm from './ProductForms/MotherboardProductForm.vue';
import RamProductForm from './ProductForms/RamProductForm.vue';
import StorageProductForm from './ProductForms/StorageProductForm.vue';
import PsuProductForm from './ProductForms/PsuProductForm.vue';
import CoolerProductForm from './ProductForms/CoolerProductForm.vue';
import PcCaseProductForm from './ProductForms/PcCaseProductForm.vue';

interface Props {
  modelValue: Partial<ProductRequest> | Partial<ProductResponse>;
  category: string;
  editable?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  editable: true
});

const emit = defineEmits<{
  'update:modelValue': [value: Partial<ProductRequest>]
}>();

const localValue = ref<Partial<ProductRequest>>(props.modelValue);

// Map category to component
const formComponent = computed<Component | null>(() => {
  const categoryLower = props.category.toLowerCase();
  
  switch (categoryLower) {
    case ProductCategory.CPU:
      return CpuProductForm;
    case ProductCategory.GPU:
      return GpuProductForm;
    case ProductCategory.Motherboard:
      return MotherboardProductForm;
    case ProductCategory.RAM:
      return RamProductForm;
    case ProductCategory.Storage:
      return StorageProductForm;
    case ProductCategory.PowerSupply:
      return PsuProductForm;
    case ProductCategory.Cooler:
      return CoolerProductForm;
    case ProductCategory.Case:
      return PcCaseProductForm;
    default:
      return null;
  }
});

watch(
  () => props.modelValue,
  (newValue) => {
    localValue.value = newValue;
  },
  { deep: true }
);

watch(
  localValue,
  (newValue) => {
    emit('update:modelValue', newValue);
  },
  { deep: true }
);
</script>
