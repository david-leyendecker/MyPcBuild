<template>
  <v-container fluid class="pa-0">
    <!-- Form Factor and Color - Side by side -->
    <v-row>
      <v-col cols="12" md="6">
        <v-text-field 
          v-model="localProduct.formFactor"
          label="Form Factor"
          :readonly="!editable"
          :variant="editable ? 'filled' : 'outlined'"
          placeholder="e.g., Mid Tower, Full Tower"
        ></v-text-field>
      </v-col>
      <v-col cols="12" md="6">
        <v-text-field 
          v-model="localProduct.color"
          label="Color"
          :readonly="!editable"
          :variant="editable ? 'filled' : 'outlined'"
          placeholder="e.g., Black, White"
        ></v-text-field>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <!-- Side Panel Window -->
        <v-text-field 
          v-model="localProduct.sidePanelWindow"
          label="Side Panel Window"
          :readonly="!editable"
          :variant="editable ? 'filled' : 'outlined'"
          placeholder="e.g., Tempered Glass, Acrylic, None"
        ></v-text-field>
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
  </v-container>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import type { PcCaseProductRequest, PcCaseProductResponse } from '@/types/products';
import DimensionsInput from '@/components/ValueObjects/DimensionsInput.vue';

interface Props {
  modelValue: Partial<PcCaseProductRequest> | Partial<PcCaseProductResponse>;
  editable?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  editable: true
});

const emit = defineEmits<{
  'update:modelValue': [value: Partial<PcCaseProductRequest>]
}>();

const localProduct = ref<Partial<PcCaseProductRequest>>({
  formFactor: props.modelValue.formFactor ?? 'Mid Tower',
  color: props.modelValue.color ?? 'Black',
  sidePanelWindow: props.modelValue.sidePanelWindow ?? 'Tempered Glass',
  dimensions: props.modelValue.dimensions ?? { length: 450, width: 210, height: 450 }
});

watch(
  () => props.modelValue,
  (newValue) => {
    Object.assign(localProduct.value, {
      formFactor: newValue.formFactor ?? 'Mid Tower',
      color: newValue.color ?? 'Black',
      sidePanelWindow: newValue.sidePanelWindow ?? 'Tempered Glass',
      dimensions: newValue.dimensions ?? { length: 450, width: 210, height: 450 }
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
