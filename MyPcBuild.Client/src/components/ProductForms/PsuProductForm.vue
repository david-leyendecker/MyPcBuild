<template>
  <v-container fluid class="pa-0">
    <!-- Wattage and Efficiency - Side by side -->
    <v-row>
      <v-col cols="12" md="6">
        <PowerInput 
          v-model="localProduct.wattage"
          label="Wattage"
          :editable="editable"
        />
      </v-col>
      <v-col cols="12" md="6">
        <v-text-field 
          v-model="localProduct.efficiency"
          label="Efficiency Rating"
          :readonly="!editable"
          :variant="editable ? 'filled' : 'outlined'"
          placeholder="e.g., 80+ Gold, 80+ Platinum"
        ></v-text-field>
      </v-col>
    </v-row>

    <!-- Modular and Form Factor - Side by side -->
    <v-row>
      <v-col cols="12" md="6">
        <v-text-field 
          v-model="localProduct.modular"
          label="Modularity"
          :readonly="!editable"
          :variant="editable ? 'filled' : 'outlined'"
          placeholder="e.g., Fully Modular, Semi-Modular"
        ></v-text-field>
      </v-col>
      <v-col cols="12" md="6">
        <v-text-field 
          v-model="localProduct.formFactor"
          label="Form Factor"
          :readonly="!editable"
          :variant="editable ? 'filled' : 'outlined'"
          placeholder="e.g., ATX, SFX"
        ></v-text-field>
      </v-col>
    </v-row>

    <!-- Length and PCIe 8-Pin Connectors - Side by side -->
    <v-row>
      <v-col cols="12" md="6">
        <LengthInput 
          v-model="localProduct.length"
          label="Length"
          :editable="editable"
        />
      </v-col>
      <v-col cols="12" md="6">
        <v-text-field 
          v-model.number="localProduct.pcie8Pin"
          label="PCIe 8-Pin Connectors"
          :readonly="!editable"
          type="number"
          :variant="editable ? 'filled' : 'outlined'"
        ></v-text-field>
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
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
