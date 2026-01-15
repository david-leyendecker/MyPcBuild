<template>
  <v-container fluid class="pa-0">
    <!-- Type and Configuration - Side by side -->
    <v-row>
      <v-col cols="12" md="6">
        <v-select 
          v-model="localProduct.type"
          :items="memoryTypeOptions"
          label="Memory Type"
          :readonly="!editable"
          :variant="editable ? 'filled' : 'outlined'"
        ></v-select>
      </v-col>
      <v-col cols="12" md="6">
        <v-text-field 
          v-model="localProduct.configuration"
          label="Configuration"
          :readonly="!editable"
          :variant="editable ? 'filled' : 'outlined'"
          placeholder="e.g., 2x16GB"
        ></v-text-field>
      </v-col>
    </v-row>

    <!-- Capacity and Speed - Side by side -->
    <v-row>
      <v-col cols="12" md="6">
        <StorageCapacityInput 
          v-model="localProduct.capacity"
          label="Total Capacity"
          :editable="editable"
        />
      </v-col>
      <v-col cols="12" md="6">
        <FrequencyInput 
          v-model="localProduct.speed"
          label="Speed (MHz)"
          :editable="editable"
        />
      </v-col>
    </v-row>

    <!-- CAS Latency and Voltage - Side by side -->
    <v-row>
      <v-col cols="12" md="6">
        <v-text-field 
          v-model="localProduct.casLatency"
          label="CAS Latency"
          :readonly="!editable"
          :variant="editable ? 'filled' : 'outlined'"
          placeholder="e.g., CL16"
        ></v-text-field>
      </v-col>
      <v-col cols="12" md="6">
        <VoltageInput 
          v-model="localProduct.voltage"
          label="Voltage"
          :editable="editable"
        />
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
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
  { title: 'DDR3', value: 'DDR3' as MemoryType },
  { title: 'DDR4', value: 'DDR4' as MemoryType },
  { title: 'DDR5', value: 'DDR5' as MemoryType }
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
