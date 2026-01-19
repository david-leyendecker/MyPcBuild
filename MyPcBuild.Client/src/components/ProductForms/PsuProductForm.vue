<template>
  <n-flex vertical :size="12">
    <!-- Wattage and Efficiency - Side by side -->
    <n-flex :size="12">
      <div style="flex: 1; min-width: 150px;">
        <PowerInput 
          v-model="localProduct.wattage"
          label="Wattage"
          :editable="editable"
        />
      </div>
      <div style="flex: 1; min-width: 150px;">
        <label style="display: block; margin-bottom: 4px; font-size: 14px;">Efficiency Rating</label>
        <n-input 
          v-model:value="localProduct.efficiency"
          :disabled="!editable"
          placeholder="e.g., 80+ Gold, 80+ Platinum"
        />
      </div>
    </n-flex>

    <!-- Modular and Form Factor - Side by side -->
    <n-flex :size="12">
      <div style="flex: 1; min-width: 150px;">
        <label style="display: block; margin-bottom: 4px; font-size: 14px;">Modularity</label>
        <n-input 
          v-model:value="localProduct.modular"
          :disabled="!editable"
          placeholder="e.g., Fully Modular, Semi-Modular"
        />
      </div>
      <div style="flex: 1; min-width: 150px;">
        <label style="display: block; margin-bottom: 4px; font-size: 14px;">Form Factor</label>
        <n-input 
          v-model:value="localProduct.formFactor"
          :disabled="!editable"
          placeholder="e.g., ATX, SFX"
        />
      </div>
    </n-flex>

    <!-- Length and PCIe 8-Pin Connectors - Side by side -->
    <n-flex :size="12">
      <div style="flex: 1; min-width: 150px;">
        <LengthInput 
          v-model="localProduct.length"
          label="Length"
          :editable="editable"
        />
      </div>
      <div style="flex: 1; min-width: 150px;">
        <label style="display: block; margin-bottom: 4px; font-size: 14px;">PCIe 8-Pin Connectors</label>
        <n-input-number 
          v-model:value="localProduct.pcie8Pin"
          :disabled="!editable"
        />
      </div>
    </n-flex>
  </n-flex>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { NFlex, NInput, NInputNumber } from 'naive-ui';
import type { PsuProductRequest, PsuProductResponse } from '@/types/products';
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
