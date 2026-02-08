<script setup lang="ts">
import { ref, watch } from 'vue'
import type { MotherboardProductRequest, CpuSocket, FormFactor, MemoryType } from '@/types/product'
import Input from '@/components/ui/input/Input.vue'
import Label from '@/components/ui/label/Label.vue'

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
const dimensionLength = ref(props.modelValue?.dimensions?.length || 305)
const dimensionWidth = ref(props.modelValue?.dimensions?.width || 244)
const dimensionHeight = ref(props.modelValue?.dimensions?.height || 69)

const socketOptions: CpuSocket[] = ['LGA1700', 'LGA1200', 'LGA1151', 'LGA2066', 'AM5', 'AM4', 'sTRX4', 'TR4']
const formFactorOptions: FormFactor[] = ['ATX', 'MicroATX', 'MiniITX', 'EATX']
const memoryTypes: MemoryType[] = ['DDR3', 'DDR4', 'DDR5']

watch([socket, chipset, formFactor, memoryType, maxMemoryGB, dimensionLength, dimensionWidth, dimensionHeight], () => {
  emit('update:modelValue', {
    category: 'motherboard',
    socket: socket.value,
    chipset: chipset.value,
    formFactor: formFactor.value,
    memoryType: memoryType.value,
    maxMemory: { valueInGB: maxMemoryGB.value },
    dimensions: {
      length: dimensionLength.value,
      width: dimensionWidth.value,
      height: dimensionHeight.value
    }
  })
})

defineExpose({
  getFormData: () => ({
    socket: socket.value,
    chipset: chipset.value,
    formFactor: formFactor.value,
    memoryType: memoryType.value,
    maxMemory: { valueInGB: maxMemoryGB.value },
    dimensions: {
      length: dimensionLength.value,
      width: dimensionWidth.value,
      height: dimensionHeight.value
    }
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
      <Label>Dimensions (mm) *</Label>
      <div class="grid grid-cols-3 gap-2">
        <div>
          <Input v-model.number="dimensionLength" type="number" placeholder="Length" min="0" />
        </div>
        <div>
          <Input v-model.number="dimensionWidth" type="number" placeholder="Width" min="0" />
        </div>
        <div>
          <Input v-model.number="dimensionHeight" type="number" placeholder="Height" min="0" />
        </div>
      </div>
    </div>
  </div>
</template>
