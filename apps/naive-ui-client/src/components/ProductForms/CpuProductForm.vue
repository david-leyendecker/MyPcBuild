<template>
  <n-form>
    <n-grid :cols="2" :x-gap="12">
      <!-- CPU Socket -->
      <n-form-item-gi label="CPU Socket" :span="2">
        <n-select v-model:value="localProduct.socket" :options="cpuSocketOptions" placeholder="Select socket"
          :disabled="!editable" />
      </n-form-item-gi>

      <!-- Cores and Threads -->
      <n-form-item-gi label="Cores">
        <n-input-number v-model:value="localProduct.cores" placeholder="Number of cores" :disabled="!editable" style="width: 100%" />
      </n-form-item-gi>
      <n-form-item-gi label="Threads">
        <n-input-number v-model:value="localProduct.threads" placeholder="Number of threads" :disabled="!editable" style="width: 100%" />
      </n-form-item-gi>

      <!-- Base Clock and Boost Clock -->
      <n-form-item-gi label="Base Clock">
        <FrequencyInput v-model="localProduct.baseClock" :editable="editable" />
      </n-form-item-gi>
      <n-form-item-gi label="Boost Clock">
        <FrequencyInput v-model="localProduct.boostClock" :editable="editable" />
      </n-form-item-gi>


      <!-- TDP -->
      <n-form-item-gi label="TDP (Thermal Design Power)">
        <PowerInput v-model="localProduct.tdp" :editable="editable" />
      </n-form-item-gi>

      <!-- Integrated Graphics -->
      <n-form-item-gi label="Integrated Graphics">
        <n-checkbox v-model:checked="localProduct.integratedGraphics" :disabled="!editable">
          Has Integrated Graphics
        </n-checkbox>
      </n-form-item-gi>
    </n-grid>
  </n-form>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { NForm, NFormItemGi, NGrid, NSelect, NInputNumber, NCheckbox } from 'naive-ui';
import type { CpuProductRequest, CpuProductResponse } from '@/types/products';
import { cpuSocketOptions } from '@/constants/enumOptions';
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
