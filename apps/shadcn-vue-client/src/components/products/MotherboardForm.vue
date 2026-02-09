<script setup lang="ts">
import { ref, watch } from 'vue'
import type { MotherboardProductRequest, CpuSocket, FormFactor, MemoryType } from '@/types/product'
import type { Slot } from '@/types/spatial'
import Input from '@/components/ui/input/Input.vue'
import Label from '@/components/ui/label/Label.vue'
import DimensionsInput from '@/components/value-objects/DimensionsInput.vue'
import SlotsInput from '@/components/value-objects/SlotsInput.vue'

interface Props {
  modelValue?: Partial<MotherboardProductRequest>
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:modelValue': [value: Partial<MotherboardProductRequest>]
}>()

const socket = ref<CpuSocket>(props.modelValue?.socket || 'AM5')
const chipset = ref(props.modelValue?.chipset || 'X670')
const formFactor = ref<FormFactor>(props.modelValue?.formFactor || 'ATX')
const memoryType = ref<MemoryType>(props.modelValue?.memoryType || 'DDR5')
const maxMemoryGB = ref(props.modelValue?.maxMemory?.valueInGB || 128)
const dimensions = ref(props.modelValue?.dimensions || { length: 305, width: 244, height: 69 })
const slots = ref<Slot[]>(props.modelValue?.slots || [])

const socketOptions: CpuSocket[] = ['LGA1700', 'LGA1200', 'LGA1151', 'LGA2066', 'AM5', 'AM4', 'sTRX4', 'TR4']
const formFactorOptions: FormFactor[] = ['ATX', 'MicroATX', 'MiniITX', 'EATX']
const memoryTypes: MemoryType[] = ['DDR3', 'DDR4', 'DDR5']

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
      <select
        id="socket"
        v-model="socket"
        class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm"
      >
        <option v-for="opt in socketOptions" :key="opt" :value="opt">{{ opt }}</option>
      </select>
    </div>

    <div class="space-y-2">
      <Label for="chipset">Chipset *</Label>
      <Input id="chipset" v-model="chipset" type="text" placeholder="e.g., X670, Z790" />
    </div>

    <div class="space-y-2">
      <Label for="formFactor">Form Factor *</Label>
      <select
        id="formFactor"
        v-model="formFactor"
        class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm"
      >
        <option v-for="opt in formFactorOptions" :key="opt" :value="opt">{{ opt }}</option>
      </select>
    </div>

    <div class="space-y-2">
      <Label for="memoryType">Memory Type *</Label>
      <select
        id="memoryType"
        v-model="memoryType"
        class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm"
      >
        <option v-for="opt in memoryTypes" :key="opt" :value="opt">{{ opt }}</option>
      </select>
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
