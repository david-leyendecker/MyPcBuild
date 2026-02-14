<script setup lang="ts">
import { ref, watch } from 'vue'
import type { MotherboardProductRequest, CpuSocket, FormFactor, MemoryType, ProductRequest } from '@/types/product'
import type { Slot } from '@/types/spatial'
import Input from '@/components/ui/input/Input.vue'
import Label from '@/components/ui/label/Label.vue'
import FormSelect from '@/components/shared/FormSelect.vue'
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

const socketOptions = (['LGA1700', 'LGA1200', 'LGA1151', 'LGA2066', 'AM5', 'AM4', 'sTRX4', 'TR4'] as CpuSocket[]).map(v => ({ value: v, label: v }))
const formFactorOptions = (['ATX', 'MicroATX', 'MiniITX', 'EATX'] as FormFactor[]).map(v => ({ value: v, label: v }))
const memoryTypeOptions = (['DDR3', 'DDR4', 'DDR5'] as MemoryType[]).map(v => ({ value: v, label: v }))

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
    <div class="space-y-2">
      <Label for="socket">CPU Socket *</Label>
      <FormSelect v-model="socket" :options="socketOptions" />
    </div>

    <div class="space-y-2">
      <Label for="chipset">Chipset *</Label>
      <Input id="chipset" v-model="chipset" type="text" placeholder="e.g., X670, Z790" />
    </div>

    <div class="space-y-2">
      <Label for="formFactor">Form Factor *</Label>
      <FormSelect v-model="formFactor" :options="formFactorOptions" />
    </div>

    <div class="space-y-2">
      <Label for="memoryType">Memory Type *</Label>
      <FormSelect v-model="memoryType" :options="memoryTypeOptions" />
    </div>

    <div class="space-y-2">
      <Label for="maxMemory">Max Memory (GB) *</Label>
      <Input id="maxMemory" v-model.number="maxMemoryGB" type="number" min="1" />
    </div>

    <div class="space-y-2 col-span-2">
      <DimensionsInput v-model="dimensions" label="Dimensions (mm) *" />
    </div>

    <div class="col-span-2">
      <SlotsInput v-model="slots" label="PCIe/Memory Slots (optional for spatial layout)" />
    </div>
  </div>
</template>
