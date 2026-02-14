<script setup lang="ts">
import { ref, watch } from 'vue'
import type { StorageProductRequest, ProductRequest } from '@/types/product'
import Input from '@/components/ui/input/Input.vue'
import Label from '@/components/ui/label/Label.vue'

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
    <div class="space-y-2">
      <Label for="type">Storage Type *</Label>
      <Input id="type" v-model="type" type="text" placeholder="e.g., SSD, HDD" />
    </div>

    <div class="space-y-2">
      <Label for="interface">Interface *</Label>
      <Input id="interface" v-model="interfaceType" type="text" placeholder="e.g., NVMe, SATA" />
    </div>

    <div class="space-y-2">
      <Label for="formFactor">Form Factor *</Label>
      <Input id="formFactor" v-model="storageFormFactor" type="text" placeholder="e.g., M.2, 2.5-inch" />
    </div>

    <div class="space-y-2">
      <Label for="capacity">Capacity (GB) *</Label>
      <Input id="capacity" v-model.number="capacityGB" type="number" min="1" />
    </div>

    <div class="space-y-2">
      <Label for="readSpeed">Read Speed (MB/s) *</Label>
      <Input id="readSpeed" v-model.number="readSpeedMBps" type="number" min="0" />
    </div>

    <div class="space-y-2">
      <Label for="writeSpeed">Write Speed (MB/s) *</Label>
      <Input id="writeSpeed" v-model.number="writeSpeedMBps" type="number" min="0" />
    </div>
  </div>
</template>
