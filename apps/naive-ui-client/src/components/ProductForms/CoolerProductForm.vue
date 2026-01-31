<template>
  <n-form>
    <n-grid :cols="2">
      <!-- Cooler Type and Height -->
      <n-form-item-gi label="Cooler Type">
        <n-select 
          v-model:value="localProduct.coolerType"
          :options="coolerTypeOptions"
          :disabled="!editable"
        />
      </n-form-item-gi>
      
      <n-form-item-gi label="Height">
        <LengthInput 
          v-model="localProduct.height"
          :editable="editable"
        />
      </n-form-item-gi>

      <!-- TDP Rating -->
      <n-form-item-gi label="TDP Rating">
        <PowerInput 
          v-model="localProduct.tdp"
          :editable="editable"
        />
      </n-form-item-gi>

      <!-- Compatible Sockets -->
      <n-form-item-gi label="Compatible CPU Sockets">
        <n-select 
          v-model:value="localProduct.sockets"
          :options="socketOptions"
          :disabled="!editable"
          multiple
        />
      </n-form-item-gi>

      <!-- Dimensions -->
      <n-form-item-gi label="Dimensions" :span="2">
        <DimensionsInput 
          v-model="localProduct.dimensions"
          :editable="editable"
        />
      </n-form-item-gi>
    </n-grid>
  </n-form>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { NForm, NFormItemGi, NGrid, NSelect } from 'naive-ui';
import type { CoolerProductRequest, CoolerProductResponse, CoolerType, CpuSocket } from '@/types/products';
import PowerInput from '@/components/ValueObjects/PowerInput.vue';
import LengthInput from '@/components/ValueObjects/LengthInput.vue';
import DimensionsInput from '@/components/ValueObjects/DimensionsInput.vue';

interface Props {
  modelValue: Partial<CoolerProductRequest> | Partial<CoolerProductResponse>;
  editable?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  editable: true
});

const emit = defineEmits<{
  'update:modelValue': [value: Partial<CoolerProductRequest>]
}>();

const coolerTypeOptions = [
  { label: 'Air', value: 'Air' as CoolerType },
  { label: 'AIO (All-in-One)', value: 'AIO' as CoolerType },
  { label: 'Custom Loop', value: 'CustomLoop' as CoolerType }
];

const socketOptions = [
  { label: 'LGA1700', value: 'LGA1700' as CpuSocket },
  { label: 'LGA1200', value: 'LGA1200' as CpuSocket },
  { label: 'LGA1151', value: 'LGA1151' as CpuSocket },
  { label: 'LGA2066', value: 'LGA2066' as CpuSocket },
  { label: 'AM5', value: 'AM5' as CpuSocket },
  { label: 'AM4', value: 'AM4' as CpuSocket },
  { label: 'sTRX4', value: 'sTRX4' as CpuSocket },
  { label: 'TR4', value: 'TR4' as CpuSocket }
];

const localProduct = ref<Partial<CoolerProductRequest>>({
  coolerType: props.modelValue.coolerType,
  height: props.modelValue.height ?? { valueInMm: 155 },
  tdp: props.modelValue.tdp ?? { valueInWatts: 220 },
  sockets: props.modelValue.sockets ?? [],
  dimensions: props.modelValue.dimensions ?? { length: 120, width: 120, height: 155 }
});

watch(
  () => props.modelValue,
  (newValue) => {
    Object.assign(localProduct.value, {
      coolerType: newValue.coolerType,
      height: newValue.height ?? { valueInMm: 150 },
      tdp: newValue.tdp ?? { valueInWatts: 220 },
      sockets: newValue.sockets ?? [],
      dimensions: newValue.dimensions ?? { length: 100, width: 100, height: 150 }
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
