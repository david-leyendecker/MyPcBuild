<script setup lang="ts">
import { ref, watch } from 'vue'
import type { GpuProductRequest, MemoryType, GpuPowerConnector } from '@/types/product'
import Input from '@/components/ui/input/Input.vue'
import Label from '@/components/ui/label/Label.vue'
import FormSelect from '@/components/shared/FormSelect.vue'
import DimensionsInput from '@/components/shared/DimensionsInput.vue'

interface Props {
  modelValue?: Partial<GpuProductRequest>
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:modelValue': [value: Partial<GpuProductRequest>]
}>()

const chipsetManufacturer = ref(props.modelValue?.chipsetManufacturer || 'NVIDIA')
const series = ref(props.modelValue?.series || 'RTX 4000')
const vramGB = ref(props.modelValue?.vram?.valueInGB || 16)
const memoryType = ref<MemoryType>(props.modelValue?.memoryType || 'GDDR6X')
const coreClockGHz = ref(props.modelValue?.coreClock?.valueInGHz || 2.2)
const boostClockGHz = ref(props.modelValue?.boostClock?.valueInGHz || 2.5)
const tdpWatts = ref(props.modelValue?.tdp?.valueInWatts || 320)
const lengthMm = ref(props.modelValue?.length?.valueInMm || 304)
const powerConnectors = ref<GpuPowerConnector>(props.modelValue?.powerConnectors || 'Dual8Pin')
const rayTracing = ref(props.modelValue?.rayTracing || true)
const dimensionLength = ref(props.modelValue?.dimensions?.length || 304)
const dimensionWidth = ref(props.modelValue?.dimensions?.width || 137)
const dimensionHeight = ref(props.modelValue?.dimensions?.height || 61)

const memoryTypeOptions = (['GDDR5', 'GDDR5X', 'GDDR6', 'GDDR6X', 'HBM2', 'HBM2E', 'HBM3'] as MemoryType[]).map(v => ({ value: v, label: v }))
const powerConnectorOptions = (['Dual8Pin', 'Triple8Pin', 'One16Pin'] as GpuPowerConnector[]).map(v => ({ value: v, label: v }))

watch([chipsetManufacturer, series, vramGB, memoryType, coreClockGHz, boostClockGHz, tdpWatts, lengthMm, powerConnectors, rayTracing, dimensionLength, dimensionWidth, dimensionHeight], () => {
  emit('update:modelValue', {
    category: 'gpu',
    chipsetManufacturer: chipsetManufacturer.value,
    series: series.value,
    vram: { valueInGB: vramGB.value },
    memoryType: memoryType.value,
    coreClock: { valueInGHz: coreClockGHz.value },
    boostClock: { valueInGHz: boostClockGHz.value },
    tdp: { valueInWatts: tdpWatts.value },
    length: { valueInMm: lengthMm.value },
    powerConnectors: powerConnectors.value,
    rayTracing: rayTracing.value,
    dimensions: {
      length: dimensionLength.value,
      width: dimensionWidth.value,
      height: dimensionHeight.value
    }
  })
})

defineExpose({
  getFormData: () => ({
    chipsetManufacturer: chipsetManufacturer.value,
    series: series.value,
    vram: { valueInGB: vramGB.value },
    memoryType: memoryType.value,
    coreClock: { valueInGHz: coreClockGHz.value },
    boostClock: { valueInGHz: boostClockGHz.value },
    tdp: { valueInWatts: tdpWatts.value },
    length: { valueInMm: lengthMm.value },
    powerConnectors: powerConnectors.value,
    rayTracing: rayTracing.value,
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
      <Label for="chipsetManufacturer">Chipset Manufacturer *</Label>
      <Input id="chipsetManufacturer" v-model="chipsetManufacturer" type="text" placeholder="e.g., NVIDIA, AMD" />
    </div>

    <div class="space-y-2">
      <Label for="series">Series *</Label>
      <Input id="series" v-model="series" type="text" placeholder="e.g., RTX 4000" />
    </div>

    <div class="space-y-2">
      <Label for="vram">VRAM (GB) *</Label>
      <Input id="vram" v-model.number="vramGB" type="number" min="1" />
    </div>

    <div class="space-y-2">
      <Label for="memoryType">Memory Type *</Label>
      <FormSelect v-model="memoryType" :options="memoryTypeOptions" />
    </div>

    <div class="space-y-2">
      <Label for="coreClock">Core Clock (GHz) *</Label>
      <Input id="coreClock" v-model.number="coreClockGHz" type="number" step="0.1" min="0" />
    </div>

    <div class="space-y-2">
      <Label for="boostClock">Boost Clock (GHz) *</Label>
      <Input id="boostClock" v-model.number="boostClockGHz" type="number" step="0.1" min="0" />
    </div>

    <div class="space-y-2">
      <Label for="tdp">TDP (Watts) *</Label>
      <Input id="tdp" v-model.number="tdpWatts" type="number" min="0" />
    </div>

    <div class="space-y-2">
      <Label for="powerConnectors">Power Connectors *</Label>
      <FormSelect v-model="powerConnectors" :options="powerConnectorOptions" />
    </div>

    <DimensionsInput
      v-model:length="dimensionLength"
      v-model:width="dimensionWidth"
      v-model:height="dimensionHeight"
    />

    <div class="space-y-2 flex items-center">
      <input
        id="rayTracing"
        v-model="rayTracing"
        type="checkbox"
        class="h-4 w-4 rounded border-gray-300"
      />
      <Label for="rayTracing" class="ml-2 cursor-pointer">
        Ray Tracing Support
      </Label>
    </div>
  </div>
</template>
