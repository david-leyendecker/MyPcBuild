<template>
  <ProductFormContainer>
    <!-- Chipset Manufacturer and Series - Side by side -->
    <v-row dense>
      <v-col cols="6">
        <v-text-field 
          v-model="localProduct.chipsetManufacturer"
          label="Chipset Manufacturer"
          :readonly="!editable"
          :variant="editable ? 'filled' : 'outlined'"
          density="comfortable"
          placeholder="e.g., NVIDIA, AMD"
        ></v-text-field>
      </v-col>
      <v-col cols="6">
        <v-text-field 
          v-model="localProduct.series"
          label="Series"
          :readonly="!editable"
          :variant="editable ? 'filled' : 'outlined'"
          density="comfortable"
          placeholder="e.g., RTX 4090, RX 7900 XTX"
        ></v-text-field>
      </v-col>
    </v-row>

    <!-- VRAM and Memory Type - Side by side -->
    <v-row dense>
      <v-col cols="6">
        <StorageCapacityInput 
          v-model="localProduct.vram"
          label="VRAM"
          :editable="editable"
        />
      </v-col>
      <v-col cols="6">
        <v-select 
          v-model="localProduct.memoryType"
          :items="memoryTypeOptions"
          label="Memory Type"
          :readonly="!editable"
          :variant="editable ? 'filled' : 'outlined'"
          density="comfortable"
        ></v-select>
      </v-col>
    </v-row>

    <!-- Core Clock and Boost Clock - Side by side -->
    <v-row dense>
      <v-col cols="6">
        <FrequencyInput 
          v-model="localProduct.coreClock"
          label="Core Clock"
          :editable="editable"
        />
      </v-col>
      <v-col cols="6">
        <FrequencyInput 
          v-model="localProduct.boostClock"
          label="Boost Clock"
          :editable="editable"
        />
      </v-col>
    </v-row>

    <!-- TDP and Length - Side by side -->
    <v-row dense>
      <v-col cols="6">
        <PowerInput 
          v-model="localProduct.tdp"
          label="TDP"
          :editable="editable"
        />
      </v-col>
      <v-col cols="6">
        <LengthInput 
          v-model="localProduct.length"
          label="Card Length"
          :editable="editable"
        />
      </v-col>
    </v-row>

    <!-- Power Connectors -->
    <v-select 
      v-model="localProduct.powerConnectors"
      :items="powerConnectorOptions"
      label="Power Connectors"
      :readonly="!editable"
      :variant="editable ? 'filled' : 'outlined'"
      density="comfortable"
    ></v-select>

    <!-- Ray Tracing -->
    <v-checkbox 
      v-model="localProduct.rayTracing"
      label="Ray Tracing Support"
      :readonly="!editable"
      :disabled="!editable"
      density="comfortable"
    ></v-checkbox>

    <!-- Dimensions -->
    <div class="mb-2">
      <label class="text-subtitle-2 font-weight-semibold mb-2 d-block">Dimensions</label>
      <DimensionsInput 
        v-model="localProduct.dimensions"
        :editable="editable"
      />
    </div>
  </ProductFormContainer>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import type { GpuProductRequest, GpuProductResponse, MemoryType, GpuPowerConnector } from '@/types/products';
import ProductFormContainer from '@/components/ProductFormContainer.vue';
import FrequencyInput from '@/components/ValueObjects/FrequencyInput.vue';
import PowerInput from '@/components/ValueObjects/PowerInput.vue';
import LengthInput from '@/components/ValueObjects/LengthInput.vue';
import StorageCapacityInput from '@/components/ValueObjects/StorageCapacityInput.vue';
import DimensionsInput from '@/components/ValueObjects/DimensionsInput.vue';

interface Props {
  modelValue: Partial<GpuProductRequest> | Partial<GpuProductResponse>;
  editable?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  editable: true
});

const emit = defineEmits<{
  'update:modelValue': [value: Partial<GpuProductRequest>]
}>();

const memoryTypeOptions = [
  { title: 'GDDR5', value: 'GDDR5' as MemoryType },
  { title: 'GDDR5X', value: 'GDDR5X' as MemoryType },
  { title: 'GDDR6', value: 'GDDR6' as MemoryType },
  { title: 'GDDR6X', value: 'GDDR6X' as MemoryType },
  { title: 'HBM2', value: 'HBM2' as MemoryType },
  { title: 'HBM2E', value: 'HBM2E' as MemoryType },
  { title: 'HBM3', value: 'HBM3' as MemoryType }
];

const powerConnectorOptions = [
  { title: 'Dual 8-Pin', value: 'Dual8Pin' as GpuPowerConnector },
  { title: 'Triple 8-Pin', value: 'Triple8Pin' as GpuPowerConnector },
  { title: '1x 16-Pin', value: 'One16Pin' as GpuPowerConnector }
];

const localProduct = ref<Partial<GpuProductRequest>>({
  chipsetManufacturer: props.modelValue.chipsetManufacturer ?? '',
  series: props.modelValue.series ?? '',
  vram: props.modelValue.vram ?? { valueInGB: 8 },
  memoryType: props.modelValue.memoryType,
  coreClock: props.modelValue.coreClock ?? { valueInGHz: 2.0 },
  boostClock: props.modelValue.boostClock ?? { valueInGHz: 2.5 },
  tdp: props.modelValue.tdp ?? { valueInWatts: 300 },
  length: props.modelValue.length ?? { valueInMm: 300 },
  powerConnectors: props.modelValue.powerConnectors,
  rayTracing: props.modelValue.rayTracing ?? false,
  dimensions: props.modelValue.dimensions ?? { length: 300, width: 130, height: 50 }
});

watch(
  () => props.modelValue,
  (newValue) => {
    Object.assign(localProduct.value, {
      chipsetManufacturer: newValue.chipsetManufacturer ?? '',
      series: newValue.series ?? '',
      vram: newValue.vram ?? { valueInGB: 8 },
      memoryType: newValue.memoryType,
      coreClock: newValue.coreClock ?? { valueInGHz: 2.0 },
      boostClock: newValue.boostClock ?? { valueInGHz: 2.5 },
      tdp: newValue.tdp ?? { valueInWatts: 300 },
      length: newValue.length ?? { valueInMm: 300 },
      powerConnectors: newValue.powerConnectors,
      rayTracing: newValue.rayTracing ?? false,
      dimensions: newValue.dimensions ?? { length: 300, width: 130, height: 50 }
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
