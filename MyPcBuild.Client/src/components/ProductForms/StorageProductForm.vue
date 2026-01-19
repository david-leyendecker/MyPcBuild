<template>
  <n-flex vertical :size="12">
    <!-- Type and Interface - Side by side -->
    <n-flex :size="12">
      <div style="flex: 1; min-width: 150px;">
        <label style="display: block; margin-bottom: 4px; font-size: 14px;">Storage Type</label>
        <n-input 
          v-model:value="localProduct.type"
          :disabled="!editable"
          placeholder="e.g., SSD, HDD"
        />
      </div>
      <div style="flex: 1; min-width: 150px;">
        <label style="display: block; margin-bottom: 4px; font-size: 14px;">Interface</label>
        <n-input 
          v-model:value="localProduct.interface"
          :disabled="!editable"
          placeholder="e.g., NVMe, SATA"
        />
      </div>
    </n-flex>

    <!-- Form Factor and Capacity - Side by side -->
    <n-flex :size="12">
      <div style="flex: 1; min-width: 150px;">
        <label style="display: block; margin-bottom: 4px; font-size: 14px;">Form Factor</label>
        <n-input 
          v-model:value="localProduct.storageFormFactor"
          :disabled="!editable"
          placeholder="e.g., M.2 2280, 2.5 inch"
        />
      </div>
      <div style="flex: 1; min-width: 150px;">
        <StorageCapacityInput 
          v-model="localProduct.capacity"
          label="Capacity"
          :editable="editable"
        />
      </div>
    </n-flex>

    <!-- Read Speed and Write Speed - Side by side -->
    <n-flex :size="12">
      <div style="flex: 1; min-width: 150px;">
        <DataSpeedInput 
          v-model="localProduct.readSpeed"
          label="Read Speed"
          :editable="editable"
        />
      </div>
      <div style="flex: 1; min-width: 150px;">
        <DataSpeedInput 
          v-model="localProduct.writeSpeed"
          label="Write Speed"
          :editable="editable"
        />
      </div>
    </n-flex>
  </n-flex>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { NFlex, NInput } from 'naive-ui';
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
