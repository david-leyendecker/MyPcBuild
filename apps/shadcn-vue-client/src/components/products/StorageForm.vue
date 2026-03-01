<script setup lang="ts">
import { ref, watch } from 'vue'
import type { StorageProductRequest, ProductRequest } from '@/types/product'
import { FormItemText, FormItemNumber } from '@/components/form-items'

interface Props {
  modelValue?: Partial<ProductRequest>
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:modelValue': [value: Partial<StorageProductRequest>]
}>()

/** Narrow to Storage shape - safe when this form is rendered for Storage category */
const model = props.modelValue as Partial<StorageProductRequest> | undefined
const type = ref(model?.type || 'SSD')
const interfaceType = ref(model?.interface || 'NVMe')
const storageFormFactor = ref(model?.storageFormFactor || 'M.2')
const capacityGB = ref(model?.capacity?.valueInGB || 1000)
const readSpeedMBps = ref(model?.readSpeed?.valueInMBps || 3500)
const writeSpeedMBps = ref(model?.writeSpeed?.valueInMBps || 3000)

watch([type, interfaceType, storageFormFactor, capacityGB, readSpeedMBps, writeSpeedMBps], () => {
  emit('update:modelValue', {
    category: 'storage',
    type: type.value,
    interface: interfaceType.value,
    storageFormFactor: storageFormFactor.value,
    capacity: { valueInGB: capacityGB.value },
    readSpeed: { valueInMBps: readSpeedMBps.value },
    writeSpeed: { valueInMBps: writeSpeedMBps.value }
  })
})

defineExpose({
  getFormData: () => ({
    type: type.value,
    interface: interfaceType.value,
    storageFormFactor: storageFormFactor.value,
    capacity: { valueInGB: capacityGB.value },
    readSpeed: { valueInMBps: readSpeedMBps.value },
    writeSpeed: { valueInMBps: writeSpeedMBps.value }
  })
})
</script>

<template>
  <div class="grid gap-4 md:grid-cols-2">
    <FormItemText label="Storage Type *" v-model="type" placeholder="e.g., SSD, HDD" />

    <FormItemText label="Interface *" v-model="interfaceType" placeholder="e.g., NVMe, SATA" />

    <FormItemText label="Form Factor *" v-model="storageFormFactor" placeholder="e.g., M.2, 2.5-inch" />

    <FormItemNumber label="Capacity (GB) *" v-model="capacityGB" :min="1" />

    <FormItemNumber label="Read Speed (MB/s) *" v-model="readSpeedMBps" :min="0" />

    <FormItemNumber label="Write Speed (MB/s) *" v-model="writeSpeedMBps" :min="0" />
  </div>
</template>
