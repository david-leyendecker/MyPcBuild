<template>
  <n-flex vertical :size="12">
    <!-- Type and Configuration - Side by side -->
    <n-flex :size="12">
      <div style="flex: 1; min-width: 150px;">
        <label style="display: block; margin-bottom: 4px; font-size: 14px;">Memory Type</label>
        <n-select 
          v-model:value="localProduct.type"
          :options="memoryTypeOptions"
          :disabled="!editable"
        />
      </div>
      <div style="flex: 1; min-width: 150px;">
        <label style="display: block; margin-bottom: 4px; font-size: 14px;">Configuration</label>
        <n-input 
          v-model:value="localProduct.configuration"
          :disabled="!editable"
          placeholder="e.g., 2x16GB"
        />
      </div>
    </n-flex>

    <!-- Capacity and Speed - Side by side -->
    <n-flex :size="12">
      <div style="flex: 1; min-width: 150px;">
        <StorageCapacityInput 
          v-model="localProduct.capacity"
          label="Total Capacity"
          :editable="editable"
        />
      </div>
      <div style="flex: 1; min-width: 150px;">
        <FrequencyInput 
          v-model="localProduct.speed"
          label="Speed (MHz)"
          :editable="editable"
        />
      </div>
    </n-flex>

    <!-- CAS Latency and Voltage - Side by side -->
    <n-flex :size="12">
      <div style="flex: 1; min-width: 150px;">
        <label style="display: block; margin-bottom: 4px; font-size: 14px;">CAS Latency</label>
        <n-input 
          v-model:value="localProduct.casLatency"
          :disabled="!editable"
          placeholder="e.g., CL16"
        />
      </div>
      <div style="flex: 1; min-width: 150px;">
        <VoltageInput 
          v-model="localProduct.voltage"
          label="Voltage"
          :editable="editable"
        />
      </div>
    </n-flex>
  </n-flex>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { NFlex, NInput, NSelect } from 'naive-ui';
import type { RamProductRequest, RamProductResponse, MemoryType } from '@/types/products';
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

const memoryTypeOptions = [
  { label: 'DDR3', value: 'DDR3' as MemoryType },
  { label: 'DDR4', value: 'DDR4' as MemoryType },
  { label: 'DDR5', value: 'DDR5' as MemoryType }
];

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
