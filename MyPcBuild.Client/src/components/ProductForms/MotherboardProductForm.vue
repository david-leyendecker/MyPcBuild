<template>
  <v-container fluid class="pa-0">
    <!-- Socket and Chipset - Side by side -->
    <v-row>
      <v-col cols="12" md="6">
        <v-select 
          v-model="localProduct.socket"
          :items="socketOptions"
          label="CPU Socket"
          :readonly="!editable"
          :variant="editable ? 'filled' : 'outlined'"
        ></v-select>
      </v-col>
      <v-col cols="12" md="6">
        <v-text-field 
          v-model="localProduct.chipset"
          label="Chipset"
          :readonly="!editable"
          :variant="editable ? 'filled' : 'outlined'"
          placeholder="e.g., Z790, X670E"
        ></v-text-field>
      </v-col>
    </v-row>

    <!-- Form Factor and Memory Type - Side by side -->
    <v-row>
      <v-col cols="12" md="6">
        <v-select 
          v-model="localProduct.formFactor"
          :items="formFactorOptions"
          label="Form Factor"
          :readonly="!editable"
          :variant="editable ? 'filled' : 'outlined'"
        ></v-select>
      </v-col>
      <v-col cols="12" md="6">
        <v-select 
          v-model="localProduct.memoryType"
          :items="memoryTypeOptions"
          label="Memory Type"
          :readonly="!editable"
          :variant="editable ? 'filled' : 'outlined'"
        ></v-select>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <!-- Max Memory -->
        <StorageCapacityInput 
          v-model="localProduct.maxMemory"
          label="Maximum Memory Capacity"
          :editable="editable"
        />
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

    <v-row>
      <v-col cols="12">
        <!-- Slots -->
        <SlotsInput 
          v-model="localProduct.slots"
          :editable="editable"
        />
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
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
  { title: 'LGA1700', value: 'LGA1700' as CpuSocket },
  { title: 'LGA1200', value: 'LGA1200' as CpuSocket },
  { title: 'LGA1151', value: 'LGA1151' as CpuSocket },
  { title: 'LGA2066', value: 'LGA2066' as CpuSocket },
  { title: 'AM5', value: 'AM5' as CpuSocket },
  { title: 'AM4', value: 'AM4' as CpuSocket },
  { title: 'sTRX4', value: 'sTRX4' as CpuSocket },
  { title: 'TR4', value: 'TR4' as CpuSocket }
];

const formFactorOptions = [
  { title: 'ATX', value: 'ATX' as FormFactor },
  { title: 'Micro ATX', value: 'MicroATX' as FormFactor },
  { title: 'Mini ITX', value: 'MiniITX' as FormFactor },
  { title: 'E-ATX', value: 'EATX' as FormFactor }
];

const memoryTypeOptions = [
  { title: 'DDR3', value: 'DDR3' as MemoryType },
  { title: 'DDR4', value: 'DDR4' as MemoryType },
  { title: 'DDR5', value: 'DDR5' as MemoryType }
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
