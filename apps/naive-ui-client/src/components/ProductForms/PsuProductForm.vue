<template>
  <n-form>
    <n-grid :cols="2" :x-gap="12">
      <!-- Wattage and Efficiency -->
      <n-form-item-gi label="Wattage">
        <PowerInput 
          v-model="localProduct.wattage"
          :editable="editable"
        />
      </n-form-item-gi>
      
      <n-form-item-gi label="Efficiency Rating">
        <n-select 
          v-model:value="localProduct.efficiency"
          :options="psuEfficiencyOptions"
          :disabled="!editable"
        />
      </n-form-item-gi>

      <!-- Modular and Form Factor -->
      <n-form-item-gi label="Modularity">
        <n-select 
          v-model:value="localProduct.modular"
          :options="psuModularityOptions"
          :disabled="!editable"
        />
      </n-form-item-gi>
      
      <n-form-item-gi label="Form Factor">
        <n-select 
          v-model:value="localProduct.formFactor"
          :options="psuFormFactorOptions"
          :disabled="!editable"
        />
      </n-form-item-gi>

      <!-- Length and PCIe 8-Pin Connectors -->
      <n-form-item-gi label="Length">
        <LengthInput 
          v-model="localProduct.length"
          :editable="editable"
        />
      </n-form-item-gi>
      
      <n-form-item-gi label="PCIe 8-Pin Connectors">
        <n-input-number 
          v-model:value="localProduct.pcie8Pin"
          :disabled="!editable"
          style="width: 100%"
        />
      </n-form-item-gi>
    </n-grid>
  </n-form>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { NForm, NFormItemGi, NGrid, NInputNumber, NSelect } from 'naive-ui';
import type { PsuProductRequest, PsuProductResponse } from '@/types/products';
import { psuEfficiencyOptions, psuModularityOptions, psuFormFactorOptions } from '@/constants/enumOptions';
import PowerInput from '@/components/ValueObjects/PowerInput.vue';
import LengthInput from '@/components/ValueObjects/LengthInput.vue';

interface Props {
  modelValue: Partial<PsuProductRequest> | Partial<PsuProductResponse>;
  editable?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  editable: true
});

const emit = defineEmits<{
  'update:modelValue': [value: Partial<PsuProductRequest>]
}>();

const localProduct = ref<Partial<PsuProductRequest>>({
  wattage: props.modelValue.wattage ?? { valueInWatts: 750 },
  efficiency: props.modelValue.efficiency ?? '80+ Gold',
  modular: props.modelValue.modular ?? 'Fully Modular',
  formFactor: props.modelValue.formFactor ?? 'ATX',
  length: props.modelValue.length ?? { valueInMm: 160 },
  pcie8Pin: props.modelValue.pcie8Pin ?? 4
});

watch(
  () => props.modelValue,
  (newValue) => {
    Object.assign(localProduct.value, {
      wattage: newValue.wattage ?? { valueInWatts: 750 },
      efficiency: newValue.efficiency ?? '80+ Gold',
      modular: newValue.modular ?? 'Fully Modular',
      formFactor: newValue.formFactor ?? 'ATX',
      length: newValue.length ?? { valueInMm: 160 },
      pcie8Pin: newValue.pcie8Pin ?? 4
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
