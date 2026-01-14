<template>
  <div class="cpu-product-form d-flex flex-column ga-3">
    <!-- CPU Socket -->
    <v-select 
      v-model="localProduct.socket"
      :items="socketOptions"
      label="CPU Socket"
      :readonly="!editable"
      :variant="editable ? 'filled' : 'outlined'"
      density="comfortable"
    ></v-select>

    <!-- Cores and Threads - Side by side -->
    <v-row dense>
      <v-col cols="6">
        <v-text-field 
          v-model.number="localProduct.cores"
          label="Cores"
          :readonly="!editable"
          type="number"
          :variant="editable ? 'filled' : 'outlined'"
          density="comfortable"
        ></v-text-field>
      </v-col>
      <v-col cols="6">
        <v-text-field 
          v-model.number="localProduct.threads"
          label="Threads"
          :readonly="!editable"
          type="number"
          :variant="editable ? 'filled' : 'outlined'"
          density="comfortable"
        ></v-text-field>
      </v-col>
    </v-row>

    <!-- Base Clock and Boost Clock - Side by side -->
    <v-row dense>
      <v-col cols="6">
        <FrequencyInput 
          v-model="localProduct.baseClock"
          label="Base Clock"
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

    <!-- TDP -->
    <PowerInput 
      v-model="localProduct.tdp"
      label="TDP (Thermal Design Power)"
      :editable="editable"
    />

    <!-- Integrated Graphics -->
    <v-checkbox 
      v-model="localProduct.integratedGraphics"
      label="Integrated Graphics"
      :readonly="!editable"
      :disabled="!editable"
      density="comfortable"
    ></v-checkbox>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import type { CpuProductRequest, CpuProductResponse, CpuSocket } from '@/types/products';
import FrequencyInput from '@/components/ValueObjects/FrequencyInput.vue';
import PowerInput from '@/components/ValueObjects/PowerInput.vue';

interface Props {
  modelValue: Partial<CpuProductRequest> | Partial<CpuProductResponse>;
  editable?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  editable: true
});

const emit = defineEmits<{
  'update:modelValue': [value: Partial<CpuProductRequest>]
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

const localProduct = ref<Partial<CpuProductRequest>>({
  socket: props.modelValue.socket,
  cores: props.modelValue.cores ?? 8,
  threads: props.modelValue.threads ?? 16,
  baseClock: props.modelValue.baseClock ?? { valueInGHz: 3.5 },
  boostClock: props.modelValue.boostClock ?? { valueInGHz: 5.0 },
  tdp: props.modelValue.tdp ?? { valueInWatts: 105 },
  integratedGraphics: props.modelValue.integratedGraphics ?? false
});

watch(
  () => props.modelValue,
  (newValue) => {
    Object.assign(localProduct.value, {
      socket: newValue.socket,
      cores: newValue.cores ?? 8,
      threads: newValue.threads ?? 16,
      baseClock: newValue.baseClock ?? { valueInGHz: 3.5 },
      boostClock: newValue.boostClock ?? { valueInGHz: 5.0 },
      tdp: newValue.tdp ?? { valueInWatts: 105 },
      integratedGraphics: newValue.integratedGraphics ?? false
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
