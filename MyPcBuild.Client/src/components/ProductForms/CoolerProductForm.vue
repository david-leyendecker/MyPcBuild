<template>
  <div class="cooler-product-form d-flex flex-column ga-3">
    <!-- Cooler Type and Height - Side by side -->
    <v-row dense>
      <v-col cols="6">
        <v-select 
          v-model="localProduct.coolerType"
          :items="coolerTypeOptions"
          label="Cooler Type"
          :readonly="!editable"
          :variant="editable ? 'filled' : 'outlined'"
          density="comfortable"
        ></v-select>
      </v-col>
      <v-col cols="6">
        <LengthInput 
          v-model="localProduct.height"
          label="Height"
          :editable="editable"
        />
      </v-col>
    </v-row>

    <!-- TDP -->
    <PowerInput 
      v-model="localProduct.tdp"
      label="TDP Rating"
      :editable="editable"
    />

    <!-- Compatible Sockets -->
    <v-select 
      v-model="localProduct.sockets"
      :items="socketOptions"
      label="Compatible CPU Sockets"
      :readonly="!editable"
      :variant="editable ? 'filled' : 'outlined'"
      density="comfortable"
      multiple
      chips
    ></v-select>

    <!-- Dimensions -->
    <div class="mb-2">
      <label class="text-subtitle-2 font-weight-semibold mb-2 d-block">Dimensions</label>
      <DimensionsInput 
        v-model="localProduct.dimensions"
        :editable="editable"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
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
  { title: 'Air', value: 'Air' as CoolerType },
  { title: 'AIO (All-in-One)', value: 'AIO' as CoolerType },
  { title: 'Custom Loop', value: 'CustomLoop' as CoolerType }
];

const socketOptions = [
  { title: 'LGA1700', value: 'LGA1700' as CpuSocket },
  { title: 'LGA1200', value: 'LGA1200' as CpuSocket },
  { title: 'LGA1151', value: 'LGA1151' as CpuSocket },
  { title: 'LGA2066', value: 'LGA2066' as CpuSocket },
  { title: 'AM5', value: 'AM5' as CpuSocket },
  { title: 'AM4', value: 'AM4' as CpuSocket },
  { title: 'sTRX4', value: 'sTRX4' as CpuSocket },
  { title: 'TR4', value: 'TR4' as CpuSocket }
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
    localProduct.value = {
      coolerType: newValue.coolerType,
      height: newValue.height ?? { valueInMm: 155 },
      tdp: newValue.tdp ?? { valueInWatts: 220 },
      sockets: newValue.sockets ?? [],
      dimensions: newValue.dimensions ?? { length: 120, width: 120, height: 155 }
    };
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
