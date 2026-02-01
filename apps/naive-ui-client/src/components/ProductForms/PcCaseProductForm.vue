<template>
  <n-form>
    <!-- Form Factor and Color -->
    <n-grid :cols="2" :x-gap="12">
      <n-form-item-gi label="Form Factor">
        <n-input v-model:value="localProduct.formFactor" :disabled="!editable"
          placeholder="e.g., Mid Tower, Full Tower" />
      </n-form-item-gi>
      
      <n-form-item-gi label="Color">
        <n-input v-model:value="localProduct.color" :disabled="!editable" placeholder="e.g., Black, White" />
      </n-form-item-gi>

      <!-- Side Panel Window -->
      <n-form-item-gi label="Side Panel Window">
        <n-input v-model:value="localProduct.sidePanelWindow" :disabled="!editable"
          placeholder="e.g., Tempered Glass, Acrylic, None" />
      </n-form-item-gi>

      <!-- Dimensions -->
      <n-form-item-gi label="Dimensions" :span="2">
        <DimensionsInput v-model="localProduct.dimensions" :editable="editable" />
      </n-form-item-gi>

      <!-- Chambers -->
      <n-form-item-gi :span="2">
        <ChambersInput v-model="localProduct.chambers" :editable="editable" />
      </n-form-item-gi>
    </n-grid>
  </n-form>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { NForm, NFormItemGi, NGrid, NInput } from 'naive-ui';
import type { PcCaseProductRequest, PcCaseProductResponse } from '@/types/products';
import DimensionsInput from '@/components/ValueObjects/DimensionsInput.vue';
import ChambersInput from '@/components/ValueObjects/ChambersInput.vue';

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
  dimensions: props.modelValue.dimensions ?? { length: 450, width: 210, height: 450 },
  chambers: props.modelValue.chambers ?? []
});

watch(
  () => props.modelValue,
  (newValue) => {
    Object.assign(localProduct.value, {
      formFactor: newValue.formFactor ?? 'Mid Tower',
      color: newValue.color ?? 'Black',
      sidePanelWindow: newValue.sidePanelWindow ?? 'Tempered Glass',
      dimensions: newValue.dimensions ?? { length: 450, width: 210, height: 450 },
      chambers: newValue.chambers ?? []
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
