<template>
  <n-form>
    <n-grid :cols="2" :x-gap="12">
      <!-- Type and Configuration -->
      <n-form-item-gi label="Memory Type">
        <n-select 
          v-model:value="localProduct.type"
          :options="ramMemoryTypeOptions"
          :disabled="!editable"
        />
      </n-form-item-gi>
      
      <n-form-item-gi label="Configuration">
        <n-input 
          v-model:value="localProduct.configuration"
          :disabled="!editable"
          placeholder="e.g., 2x16GB"
        />
      </n-form-item-gi>

      <!-- Capacity and Speed -->
      <n-form-item-gi label="Total Capacity">
        <StorageCapacityInput 
          v-model="localProduct.capacity"
          :editable="editable"
        />
      </n-form-item-gi>
      
      <n-form-item-gi label="Speed (MHz)">
        <FrequencyInput 
          v-model="localProduct.speed"
          :editable="editable"
        />
      </n-form-item-gi>

      <!-- CAS Latency and Voltage -->
      <n-form-item-gi label="CAS Latency">
        <n-input 
          v-model:value="localProduct.casLatency"
          :disabled="!editable"
          placeholder="e.g., CL16"
        />
      </n-form-item-gi>
      
      <n-form-item-gi label="Voltage">
        <VoltageInput 
          v-model="localProduct.voltage"
          :editable="editable"
        />
      </n-form-item-gi>
    </n-grid>
  </n-form>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { NForm, NFormItemGi, NGrid, NInput, NSelect } from 'naive-ui';
import type { RamProductRequest, RamProductResponse } from '@/types/products';
import { ramMemoryTypeOptions } from '@/constants/enumOptions';
import StorageCapacityInput from '@/components/ValueObjects/StorageCapacityInput.vue';
import FrequencyInput from '@/components/ValueObjects/FrequencyInput.vue';
import VoltageInput from '@/components/ValueObjects/VoltageInput.vue';

interface Props {
  modelValue: Partial<RamProductRequest> | Partial<RamProductResponse>;
  editable?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  editable: true
});

const emit = defineEmits<{
  'update:modelValue': [value: Partial<RamProductRequest>]
}>();

const localProduct = ref<Partial<RamProductRequest>>({
  type: props.modelValue.type,
  capacity: props.modelValue.capacity ?? { valueInGB: 32 },
  configuration: props.modelValue.configuration ?? '2x16GB',
  speed: props.modelValue.speed ?? { valueInGHz: 3.6 },
  casLatency: props.modelValue.casLatency ?? 'CL16',
  voltage: props.modelValue.voltage ?? { valueInVolts: 1.35 }
});

watch(
  () => props.modelValue,
  (newValue) => {
    Object.assign(localProduct.value, {
      type: newValue.type,
      capacity: newValue.capacity ?? { valueInGB: 32 },
      configuration: newValue.configuration ?? '2x16GB',
      speed: newValue.speed ?? { valueInGHz: 3.6 },
      casLatency: newValue.casLatency ?? 'CL16',
      voltage: newValue.voltage ?? { valueInVolts: 1.35 }
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
