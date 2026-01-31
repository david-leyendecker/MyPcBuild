<template>
  <n-form>
    <n-grid :cols="2">
      <!-- Socket and Chipset -->
      <n-form-item-gi label="CPU Socket">
        <n-select 
          v-model:value="localProduct.socket"
          :options="socketOptions"
          :disabled="!editable"
        />
      </n-form-item-gi>
      
      <n-form-item-gi label="Chipset">
        <n-input 
          v-model:value="localProduct.chipset"
          :disabled="!editable"
          placeholder="e.g., Z790, X670E"
        />
      </n-form-item-gi>

      <!-- Form Factor and Memory Type -->
      <n-form-item-gi label="Form Factor">
        <n-select 
          v-model:value="localProduct.formFactor"
          :options="formFactorOptions"
          :disabled="!editable"
        />
      </n-form-item-gi>
      
      <n-form-item-gi label="Memory Type">
        <n-select 
          v-model:value="localProduct.memoryType"
          :options="memoryTypeOptions"
          :disabled="!editable"
        />
      </n-form-item-gi>

      <!-- Max Memory -->
      <n-form-item-gi label="Maximum Memory Capacity">
        <StorageCapacityInput 
          v-model="localProduct.maxMemory"
          :editable="editable"
        />
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
import { NForm, NFormItemGi, NGrid, NInput, NSelect } from 'naive-ui';
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
