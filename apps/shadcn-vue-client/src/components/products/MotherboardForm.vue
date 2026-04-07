<script setup lang="ts">
import { ref, watch } from 'vue'
import type { MotherboardProductRequest, CpuSocket, FormFactor, MemoryType, ProductRequest } from '@/types/product'
import type { Slot } from '@/types/spatial'
import { FormItemSelect, FormItemText, FormItemNumber } from '@/components/form-items'
import { cpuSocketOptions, formFactorOptions, ramMemoryTypeOptions } from '@/constants/enumOptions'
import DimensionsInput from '@/components/value-objects/DimensionsInput.vue'
import SlotsInput from '@/components/value-objects/SlotsInput.vue'

interface Props {
  modelValue?: Partial<ProductRequest>
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:modelValue': [value: Partial<MotherboardProductRequest>]
}>()

/** Narrow to Motherboard shape - safe when this form is rendered for Motherboard category */
const model = props.modelValue as Partial<MotherboardProductRequest> | undefined
const socket = ref<CpuSocket>(model?.socket || 'AM5')
const chipset = ref(model?.chipset || 'X670')
const formFactor = ref<FormFactor>(model?.formFactor || 'ATX')
const memoryType = ref<MemoryType>(model?.memoryType || 'DDR5')
const maxMemoryGB = ref(model?.maxMemory?.valueInGB || 128)
const dimensions = ref(model?.dimensions || { length: 305, width: 244, height: 69 })
const slots = ref<Slot[]>(model?.slots || [])

watch([socket, chipset, formFactor, memoryType, maxMemoryGB, dimensions, slots], () => {
  emit('update:modelValue', {
    category: 'motherboard',
    socket: socket.value,
    chipset: chipset.value,
    formFactor: formFactor.value,
    memoryType: memoryType.value,
    maxMemory: { valueInGB: maxMemoryGB.value },
    dimensions: dimensions.value,
    slots: slots.value
  })
}, { deep: true })

defineExpose({
  getFormData: () => ({
    socket: socket.value,
    chipset: chipset.value,
    formFactor: formFactor.value,
    memoryType: memoryType.value,
    maxMemory: { valueInGB: maxMemoryGB.value },
    dimensions: dimensions.value,
    slots: slots.value
  })
})
</script>

<template>
  <div class="grid gap-4 md:grid-cols-2">
    <FormItemSelect label="CPU Socket *" v-model="socket" :options="cpuSocketOptions" />

    <FormItemText label="Chipset *" v-model="chipset" placeholder="e.g., X670, Z790" />

    <FormItemSelect label="Form Factor *" v-model="formFactor" :options="formFactorOptions" />

    <FormItemSelect label="Memory Type *" v-model="memoryType" :options="ramMemoryTypeOptions" />

    <FormItemNumber label="Max Memory (GB) *" v-model="maxMemoryGB" :min="1" />

    <div class="col-span-2">
      <DimensionsInput v-model="dimensions" label="Dimensions (mm) *" />
    </div>

    <div class="col-span-2">
      <SlotsInput v-model="slots" label="PCIe/Memory Slots (optional for spatial layout)" />
    </div>
  </div>
</template>
