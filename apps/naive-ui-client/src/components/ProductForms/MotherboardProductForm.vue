<template>
  <n-form>
    <n-grid :cols="2" :x-gap="12">
      <!-- Socket and Chipset -->
      <n-form-item-gi label="CPU Socket">
        <n-select 
          v-model:value="localProduct.socket"
          :options="cpuSocketOptions"
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
          :options="ramMemoryTypeOptions"
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
import type { MotherboardProductRequest, MotherboardProductResponse } from '@/types/products';
import { cpuSocketOptions, formFactorOptions, ramMemoryTypeOptions } from '@/constants/enumOptions';
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
