<template>
  <n-form>
    <n-grid :cols="2">
      <!-- Type and Interface -->
      <n-form-item-gi label="Storage Type">
        <n-input 
          v-model:value="localProduct.type"
          :disabled="!editable"
          placeholder="e.g., SSD, HDD"
        />
      </n-form-item-gi>
      
      <n-form-item-gi label="Interface">
        <n-input 
          v-model:value="localProduct.interface"
          :disabled="!editable"
          placeholder="e.g., NVMe, SATA"
        />
      </n-form-item-gi>

      <!-- Form Factor and Capacity -->
      <n-form-item-gi label="Form Factor">
        <n-input 
          v-model:value="localProduct.storageFormFactor"
          :disabled="!editable"
          placeholder="e.g., M.2 2280, 2.5 inch"
        />
      </n-form-item-gi>
      
      <n-form-item-gi label="Capacity">
        <StorageCapacityInput 
          v-model="localProduct.capacity"
          :editable="editable"
        />
      </n-form-item-gi>

      <!-- Read Speed and Write Speed -->
      <n-form-item-gi label="Read Speed">
        <DataSpeedInput 
          v-model="localProduct.readSpeed"
          :editable="editable"
        />
      </n-form-item-gi>
      
      <n-form-item-gi label="Write Speed">
        <DataSpeedInput 
          v-model="localProduct.writeSpeed"
          :editable="editable"
        />
      </n-form-item-gi>
    </n-grid>
  </n-form>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { NForm, NFormItemGi, NGrid, NInput } from 'naive-ui';
import type { StorageProductRequest, StorageProductResponse } from '@/types/products';
import StorageCapacityInput from '@/components/ValueObjects/StorageCapacityInput.vue';
import DataSpeedInput from '@/components/ValueObjects/DataSpeedInput.vue';

interface Props {
  modelValue: Partial<StorageProductRequest> | Partial<StorageProductResponse>;
  editable?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  editable: true
});

const emit = defineEmits<{
  'update:modelValue': [value: Partial<StorageProductRequest>]
}>();

const localProduct = ref<Partial<StorageProductRequest>>({
  type: props.modelValue.type ?? 'SSD',
  interface: props.modelValue.interface ?? 'NVMe',
  storageFormFactor: props.modelValue.storageFormFactor ?? 'M.2 2280',
  capacity: props.modelValue.capacity ?? { valueInGB: 1000 },
  readSpeed: props.modelValue.readSpeed ?? { valueInMBps: 7000 },
  writeSpeed: props.modelValue.writeSpeed ?? { valueInMBps: 5000 }
});

watch(
  () => props.modelValue,
  (newValue) => {
    Object.assign(localProduct.value, {
      type: newValue.type ?? 'SSD',
      interface: newValue.interface ?? 'NVMe',
      storageFormFactor: newValue.storageFormFactor ?? 'M.2 2280',
      capacity: newValue.capacity ?? { valueInGB: 1000 },
      readSpeed: newValue.readSpeed ?? { valueInMBps: 7000 },
      writeSpeed: newValue.writeSpeed ?? { valueInMBps: 5000 }
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
