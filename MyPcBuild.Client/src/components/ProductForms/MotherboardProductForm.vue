<template>
  <n-flex vertical :size="12">
    <!-- Socket and Chipset - Side by side -->
    <n-flex :size="12">
      <div style="flex: 1; min-width: 150px;">
        <label style="display: block; margin-bottom: 4px; font-size: 14px;">CPU Socket</label>
        <n-select 
          v-model:value="localProduct.socket"
          :options="socketOptions"
          :disabled="!editable"
        />
      </div>
      <div style="flex: 1; min-width: 150px;">
        <label style="display: block; margin-bottom: 4px; font-size: 14px;">Chipset</label>
        <n-input 
          v-model:value="localProduct.chipset"
          :disabled="!editable"
          placeholder="e.g., Z790, X670E"
        />
      </div>
    </n-flex>

    <!-- Form Factor and Memory Type - Side by side -->
    <n-flex :size="12">
      <div style="flex: 1; min-width: 150px;">
        <label style="display: block; margin-bottom: 4px; font-size: 14px;">Form Factor</label>
        <n-select 
          v-model:value="localProduct.formFactor"
          :options="formFactorOptions"
          :disabled="!editable"
        />
      </div>
      <div style="flex: 1; min-width: 150px;">
        <label style="display: block; margin-bottom: 4px; font-size: 14px;">Memory Type</label>
        <n-select 
          v-model:value="localProduct.memoryType"
          :options="memoryTypeOptions"
          :disabled="!editable"
        />
      </div>
    </n-flex>

    <!-- Max Memory -->
    <n-flex :size="12">
      <div style="flex: 1; min-width: 150px;">
        <StorageCapacityInput 
          v-model="localProduct.maxMemory"
          label="Maximum Memory Capacity"
          :editable="editable"
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

    <!-- Slots -->
    <n-flex :size="12">
      <div style="flex: 1; min-width: 150px;">
        <SlotsInput 
          v-model="localProduct.slots"
          :editable="editable"
        />
      </div>
    </n-flex>
  </n-flex>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { NFlex, NInput, NSelect } from 'naive-ui';
import type { MotherboardProductRequest, MotherboardProductResponse, CpuSocket, FormFactor, MemoryType } from '@/types/products';
import StorageCapacityInput from '@/components/ValueObjects/StorageCapacityInput.vue';
import DimensionsInput from '@/components/ValueObjects/DimensionsInput.vue';
import SlotsInput from '@/components/ValueObjects/SlotsInput.vue';

interface Props {
  modelValue: Partial<MotherboardProductRequest> | Partial<MotherboardProductResponse>;
  editable?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  editable: true
});

const emit = defineEmits<{
  'update:modelValue': [value: Partial<MotherboardProductRequest>]
}>();

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

const formFactorOptions = [
  { label: 'ATX', value: 'ATX' as FormFactor },
  { label: 'Micro ATX', value: 'MicroATX' as FormFactor },
  { label: 'Mini ITX', value: 'MiniITX' as FormFactor },
  { label: 'E-ATX', value: 'EATX' as FormFactor }
];

const memoryTypeOptions = [
  { label: 'DDR3', value: 'DDR3' as MemoryType },
  { label: 'DDR4', value: 'DDR4' as MemoryType },
  { label: 'DDR5', value: 'DDR5' as MemoryType }
];

const localProduct = ref<Partial<MotherboardProductRequest>>({
  socket: props.modelValue.socket,
  chipset: props.modelValue.chipset ?? '',
  formFactor: props.modelValue.formFactor,
  memoryType: props.modelValue.memoryType,
  maxMemory: props.modelValue.maxMemory ?? { valueInGB: 128 },
  dimensions: props.modelValue.dimensions ?? { length: 305, width: 244, height: 50 },
  slots: props.modelValue.slots ?? []
});

watch(
  () => props.modelValue,
  (newValue) => {
    Object.assign(localProduct.value, {
      socket: newValue.socket,
      chipset: newValue.chipset ?? '',
      formFactor: newValue.formFactor,
      memoryType: newValue.memoryType,
      maxMemory: newValue.maxMemory ?? { valueInGB: 128 },
      dimensions: newValue.dimensions ?? { length: 305, width: 244, height: 50 },
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
