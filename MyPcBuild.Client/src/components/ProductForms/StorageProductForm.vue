<template>
  <v-container fluid class="pa-0">
    <!-- Type and Interface - Side by side -->
    <v-row>
      <v-col cols="12" md="6">
        <v-text-field 
          v-model="localProduct.type"
          label="Storage Type"
          :readonly="!editable"
          :variant="editable ? 'filled' : 'outlined'"
          placeholder="e.g., SSD, HDD"
        ></v-text-field>
      </v-col>
      <v-col cols="12" md="6">
        <v-text-field 
          v-model="localProduct.interface"
          label="Interface"
          :readonly="!editable"
          :variant="editable ? 'filled' : 'outlined'"
          placeholder="e.g., NVMe, SATA"
        ></v-text-field>
      </v-col>
    </v-row>

    <!-- Form Factor and Capacity - Side by side -->
    <v-row>
      <v-col cols="12" md="6">
        <v-text-field 
          v-model="localProduct.storageFormFactor"
          label="Form Factor"
          :readonly="!editable"
          :variant="editable ? 'filled' : 'outlined'"
          placeholder="e.g., M.2 2280, 2.5 inch"
        ></v-text-field>
      </v-col>
      <v-col cols="12" md="6">
        <StorageCapacityInput 
          v-model="localProduct.capacity"
          label="Capacity"
          :editable="editable"
        />
      </v-col>
    </v-row>

    <!-- Read Speed and Write Speed - Side by side -->
    <v-row>
      <v-col cols="12" md="6">
        <DataSpeedInput 
          v-model="localProduct.readSpeed"
          label="Read Speed"
          :editable="editable"
        />
      </v-col>
      <v-col cols="12" md="6">
        <DataSpeedInput 
          v-model="localProduct.writeSpeed"
          label="Write Speed"
          :editable="editable"
        />
      </v-col>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
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
