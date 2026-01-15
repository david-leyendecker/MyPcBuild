<template>
  <v-container fluid class="pa-0">
    <!-- Cooler Type and Height - Side by side -->
    <v-row>
      <v-col cols="12" md="6">
        <v-select 
          v-model="localProduct.coolerType"
          :items="coolerTypeOptions"
          label="Cooler Type"
          :readonly="!editable"
          :variant="editable ? 'filled' : 'outlined'"
        ></v-select>
      </v-col>
      <v-col cols="12" md="6">
        <LengthInput 
          v-model="localProduct.height"
          label="Height"
          :editable="editable"
        />
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <!-- TDP -->
        <PowerInput 
          v-model="localProduct.tdp"
          label="TDP Rating"
          :editable="editable"
        />
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <!-- Compatible Sockets -->
        <v-select 
          v-model="localProduct.sockets"
          :items="socketOptions"
          label="Compatible CPU Sockets"
          :readonly="!editable"
          :variant="editable ? 'filled' : 'outlined'"
          multiple
          chips
        ></v-select>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <!-- Dimensions -->
        <div class="mb-2">
          <label class="text-subtitle-2 font-weight-semibold mb-2 d-block">Dimensions</label>
          <DimensionsInput 
            v-model="localProduct.dimensions"
            :editable="editable"
          />
        </div>
      </v-col>
    </v-row>
  </v-container>
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
