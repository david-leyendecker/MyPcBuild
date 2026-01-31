<template>
  <n-form>
    <n-grid :cols="2" :x-gap="12">
      <!-- Chipset Manufacturer and Series -->
      <n-form-item-gi label="Chipset Manufacturer">
        <n-input 
          v-model:value="localProduct.chipsetManufacturer"
          :disabled="!editable"
          placeholder="e.g., NVIDIA, AMD"
        />
      </n-form-item-gi>
      
      <n-form-item-gi label="Series">
        <n-input 
          v-model:value="localProduct.series"
          :disabled="!editable"
          placeholder="e.g., RTX 4090, RX 7900 XTX"
        />
      </n-form-item-gi>

      <!-- VRAM and Memory Type -->
      <n-form-item-gi label="VRAM">
        <StorageCapacityInput 
          v-model="localProduct.vram"
          :editable="editable"
        />
      </n-form-item-gi>
      
      <n-form-item-gi label="Memory Type">
        <n-select 
          v-model:value="localProduct.memoryType"
          :options="memoryTypeOptions"
          :disabled="!editable"
        />
      </n-form-item-gi>

      <!-- Core Clock and Boost Clock -->
      <n-form-item-gi label="Core Clock">
        <FrequencyInput 
          v-model="localProduct.coreClock"
          :editable="editable"
        />
      </n-form-item-gi>
      
      <n-form-item-gi label="Boost Clock">
        <FrequencyInput 
          v-model="localProduct.boostClock"
          :editable="editable"
        />
      </n-form-item-gi>

      <!-- TDP and Card Length -->
      <n-form-item-gi label="TDP">
        <PowerInput 
          v-model="localProduct.tdp"
          :editable="editable"
        />
      </n-form-item-gi>
      
      <n-form-item-gi label="Card Length">
        <LengthInput 
          v-model="localProduct.length"
          :editable="editable"
        />
      </n-form-item-gi>

      <!-- Power Connectors and Ray Tracing -->
      <n-form-item-gi label="Power Connectors">
        <n-select 
          v-model:value="localProduct.powerConnectors"
          :options="powerConnectorOptions"
          :disabled="!editable"
        />
      </n-form-item-gi>
      
      <n-form-item-gi>
        <n-checkbox 
          v-model:checked="localProduct.rayTracing"
          :disabled="!editable"
        >
          Ray Tracing Support
        </n-checkbox>
      </n-form-item-gi>

      <!-- Dimensions -->
      <n-form-item-gi label="Dimensions" :span="2">
        <DimensionsInput 
          v-model="localProduct.dimensions"
          :editable="editable"
        />
      </n-form-item-gi>

      <!-- Slots -->
      <n-form-item-gi :span="2">
        <SlotsInput 
          v-model="localProduct.slots"
          :editable="editable"
        />
      </n-form-item-gi>
    </n-grid>
  </n-form>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { NForm, NFormItemGi, NGrid, NInput, NSelect, NCheckbox } from 'naive-ui';
import type { GpuProductRequest, GpuProductResponse, MemoryType, GpuPowerConnector } from '@/types/products';
import FrequencyInput from '@/components/ValueObjects/FrequencyInput.vue';
import PowerInput from '@/components/ValueObjects/PowerInput.vue';
import LengthInput from '@/components/ValueObjects/LengthInput.vue';
import StorageCapacityInput from '@/components/ValueObjects/StorageCapacityInput.vue';
import DimensionsInput from '@/components/ValueObjects/DimensionsInput.vue';
import SlotsInput from '@/components/ValueObjects/SlotsInput.vue';

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
  { label: 'GDDR5', value: 'GDDR5' as MemoryType },
  { label: 'GDDR5X', value: 'GDDR5X' as MemoryType },
  { label: 'GDDR6', value: 'GDDR6' as MemoryType },
  { label: 'GDDR6X', value: 'GDDR6X' as MemoryType },
  { label: 'HBM2', value: 'HBM2' as MemoryType },
  { label: 'HBM2E', value: 'HBM2E' as MemoryType },
  { label: 'HBM3', value: 'HBM3' as MemoryType }
];

const powerConnectorOptions = [
  { label: 'Dual 8-Pin', value: 'Dual8Pin' as GpuPowerConnector },
  { label: 'Triple 8-Pin', value: 'Triple8Pin' as GpuPowerConnector },
  { label: '1x 16-Pin', value: 'One16Pin' as GpuPowerConnector }
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
  dimensions: props.modelValue.dimensions ?? { length: 300, width: 130, height: 50 },
  slots: props.modelValue.slots ?? []
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
      dimensions: newValue.dimensions ?? { length: 300, width: 130, height: 50 },
      slots: newValue.slots ?? []
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
