<script setup lang="ts">
import { ref, watch } from 'vue'
import type { CoolerProductRequest, CoolerType, CpuSocket } from '@/types/product'
import Input from '@/components/ui/input/Input.vue'
import Label from '@/components/ui/label/Label.vue'

interface Props {
  modelValue?: Partial<CoolerProductRequest>
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:modelValue': [value: Partial<CoolerProductRequest>]
}>()

const coolerType = ref<CoolerType>(props.modelValue?.coolerType || 'Air')
const heightMm = ref(props.modelValue?.height?.valueInMm || 160)
const tdpWatts = ref(props.modelValue?.tdp?.valueInWatts || 180)
const selectedSockets = ref<CpuSocket[]>(props.modelValue?.sockets || ['AM5', 'LGA1700'])
const dimensionLength = ref(props.modelValue?.dimensions?.length || 120)
const dimensionWidth = ref(props.modelValue?.dimensions?.width || 120)
const dimensionHeight = ref(props.modelValue?.dimensions?.height || 160)

const coolerTypeOptions: CoolerType[] = ['Air', 'AIO', 'CustomLoop']
const socketOptions: CpuSocket[] = ['LGA1700', 'LGA1200', 'LGA1151', 'LGA2066', 'AM5', 'AM4', 'sTRX4', 'TR4']

function toggleSocket(socket: CpuSocket) {
  const index = selectedSockets.value.indexOf(socket)
  if (index === -1) {
    selectedSockets.value.push(socket)
  } else {
    selectedSockets.value.splice(index, 1)
  }
}

watch([coolerType, heightMm, tdpWatts, selectedSockets, dimensionLength, dimensionWidth, dimensionHeight], () => {
  emit('update:modelValue', {
    category: 'cooler',
    coolerType: coolerType.value,
    height: { valueInMm: heightMm.value },
    tdp: { valueInWatts: tdpWatts.value },
    sockets: selectedSockets.value,
    dimensions: {
      length: dimensionLength.value,
      width: dimensionWidth.value,
      height: dimensionHeight.value
    }
  })
}, { deep: true })

defineExpose({
  getFormData: () => ({
    coolerType: coolerType.value,
    height: { valueInMm: heightMm.value },
    tdp: { valueInWatts: tdpWatts.value },
    sockets: selectedSockets.value,
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
      <Label for="coolerType">Cooler Type *</Label>
      <select
        id="coolerType"
        v-model="coolerType"
        class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm"
      >
        <option v-for="opt in coolerTypeOptions" :key="opt" :value="opt">{{ opt }}</option>
      </select>
    </div>

    <div class="space-y-2">
      <Label for="height">Height (mm) *</Label>
      <Input id="height" v-model.number="heightMm" type="number" min="0" />
    </div>

    <div class="space-y-2">
      <Label for="tdp">Max TDP (Watts) *</Label>
      <Input id="tdp" v-model.number="tdpWatts" type="number" min="0" />
    </div>

    <div class="space-y-2 col-span-2">
      <Label>Supported Sockets *</Label>
      <div class="grid grid-cols-4 gap-2">
        <div
          v-for="socket in socketOptions"
          :key="socket"
          class="flex items-center space-x-2"
        >
          <input
            :id="`socket-${socket}`"
            type="checkbox"
            :checked="selectedSockets.includes(socket)"
            class="h-4 w-4 rounded border-gray-300"
            @change="toggleSocket(socket)"
          />
          <Label :for="`socket-${socket}`" class="text-sm cursor-pointer">
            {{ socket }}
          </Label>
        </div>
      </div>
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
