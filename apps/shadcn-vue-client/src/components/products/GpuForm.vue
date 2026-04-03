<script setup lang="ts">
import { ref, watch } from 'vue'
import type { GpuProductRequest, MemoryType, GpuPowerConnector, GpuChipsetManufacturer, ProductRequest } from '@/types/product'
import { FormItemNumber, FormItemSelect, FormItemCheckbox } from '@/components/form-items'
import { gpuChipsetManufacturerOptions, gpuMemoryTypeOptions, gpuPowerConnectorOptions } from '@/constants/enumOptions'
import DimensionsInput from '@/components/shared/DimensionsInput.vue'

interface Props {
  modelValue?: Partial<ProductRequest>
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:modelValue': [value: Partial<GpuProductRequest>]
}>()

/** Narrow to GPU shape - safe when this form is rendered for GPU category */
const model = props.modelValue as Partial<GpuProductRequest> | undefined
const chipsetManufacturer = ref<GpuChipsetManufacturer>(model?.chipsetManufacturer || 'NVIDIA')
const series = ref(model?.series || 'RTX 4000')
const vramGB = ref(model?.vram?.valueInGB || 16)
const memoryType = ref<MemoryType>(model?.memoryType || 'GDDR6X')
const coreClockGHz = ref(model?.coreClock?.valueInGHz || 2.2)
const boostClockGHz = ref(model?.boostClock?.valueInGHz || 2.5)
const tdpWatts = ref(model?.tdp?.valueInWatts || 320)
const powerConnectors = ref<GpuPowerConnector>(model?.powerConnectors || 'Dual8Pin')
const rayTracing = ref(model?.rayTracing ?? true)
const dimensionLength = ref(model?.dimensions?.length || 304)
const dimensionWidth = ref(model?.dimensions?.width || 137)
const dimensionHeight = ref(model?.dimensions?.height || 61)

watch([chipsetManufacturer, series, vramGB, memoryType, coreClockGHz, boostClockGHz, tdpWatts, powerConnectors, rayTracing, dimensionLength, dimensionWidth, dimensionHeight], () => {
  emit('update:modelValue', {
    category: 'gpu',
    chipsetManufacturer: chipsetManufacturer.value,
    series: series.value,
    vram: { valueInGB: vramGB.value },
    memoryType: memoryType.value,
    coreClock: { valueInGHz: coreClockGHz.value },
    boostClock: { valueInGHz: boostClockGHz.value },
    tdp: { valueInWatts: tdpWatts.value },
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
    <FormItemSelect label="Chipset Manufacturer *" v-model="chipsetManufacturer" :options="gpuChipsetManufacturerOptions" />

    <FormItemText label="Series *" v-model="series" placeholder="e.g., RTX 4000" />

    <FormItemNumber label="VRAM (GB) *" v-model="vramGB" :min="1" />

    <FormItemSelect label="Memory Type *" v-model="memoryType" :options="gpuMemoryTypeOptions" />

    <FormItemNumber label="Core Clock (GHz) *" v-model="coreClockGHz" :step="0.1" :min="0" />

    <FormItemNumber label="Boost Clock (GHz) *" v-model="boostClockGHz" :step="0.1" :min="0" />

    <FormItemNumber label="TDP (Watts) *" v-model="tdpWatts" :min="0" />

    <FormItemSelect label="Power Connectors *" v-model="powerConnectors" :options="gpuPowerConnectorOptions" />

    <DimensionsInput
      v-model:length="dimensionLength"
      v-model:width="dimensionWidth"
      v-model:height="dimensionHeight"
    />

    <FormItemCheckbox label="Ray Tracing Support" v-model="rayTracing" />
  </div>
</template>
