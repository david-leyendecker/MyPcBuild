<template>
  <n-flex vertical :size="16">
    <!-- CPU Socket -->
    <n-select 
      v-model:value="localProduct.socket"
      :options="socketOptions"
      placeholder="CPU Socket"
      :disabled="!editable"
    />

    <!-- Cores and Threads - Side by side -->
    <n-flex :size="12">
      <n-input-number 
        v-model:value="localProduct.cores"
        placeholder="Cores"
        :readonly="!editable"
        style="flex: 1; min-width: 150px;"
      />
      <n-input-number 
        v-model:value="localProduct.threads"
        placeholder="Threads"
        :readonly="!editable"
        style="flex: 1; min-width: 150px;"
      />
    </n-flex>

    <!-- Base Clock and Boost Clock - Side by side -->
    <n-flex :size="12">
      <div style="flex: 1; min-width: 150px;">
        <label style="display: block; margin-bottom: 4px; font-size: 14px;">Base Clock</label>
        <FrequencyInput 
          v-model="localProduct.baseClock"
          label="Base Clock"
          :editable="editable"
        />
      </div>
      <div style="flex: 1; min-width: 150px;">
        <label style="display: block; margin-bottom: 4px; font-size: 14px;">Boost Clock</label>
        <FrequencyInput 
          v-model="localProduct.boostClock"
          label="Boost Clock"
          :editable="editable"
        />
      </div>
    </n-flex>

    <!-- TDP -->
    <div>
      <label style="display: block; margin-bottom: 4px; font-size: 14px;">TDP (Thermal Design Power)</label>
      <PowerInput 
        v-model="localProduct.tdp"
        label="TDP (Thermal Design Power)"
        :editable="editable"
      />
    </div>

    <!-- Integrated Graphics -->
    <n-checkbox 
      v-model:checked="localProduct.integratedGraphics"
      :disabled="!editable"
    >
      Integrated Graphics
    </n-checkbox>
  </n-flex>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { NFlex, NSelect, NInputNumber, NCheckbox } from 'naive-ui';
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
  { label: 'LGA1700', value: 'LGA1700' as CpuSocket },
  { label: 'LGA1200', value: 'LGA1200' as CpuSocket },
  { label: 'LGA1151', value: 'LGA1151' as CpuSocket },
  { label: 'LGA2066', value: 'LGA2066' as CpuSocket },
  { label: 'AM5', value: 'AM5' as CpuSocket },
  { label: 'AM4', value: 'AM4' as CpuSocket },
  { label: 'sTRX4', value: 'sTRX4' as CpuSocket },
  { label: 'TR4', value: 'TR4' as CpuSocket }
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
