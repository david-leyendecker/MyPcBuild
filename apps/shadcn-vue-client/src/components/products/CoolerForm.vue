<script setup lang="ts">
import { ref, watch } from 'vue'
import type { CoolerProductRequest, CoolerType, CpuSocket, ProductRequest } from '@/types/product'
import { FormItemSelect, FormItemNumber, FormItemCheckboxGroup } from '@/components/form-items'
import DimensionsInput from '@/components/shared/DimensionsInput.vue'

interface Props {
  modelValue?: Partial<ProductRequest>
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:modelValue': [value: Partial<CoolerProductRequest>]
}>()

/** Narrow to Cooler shape - safe when this form is rendered for Cooler category */
const model = props.modelValue as Partial<CoolerProductRequest> | undefined
const coolerType = ref<CoolerType>(model?.coolerType || 'Air')
const heightMm = ref(model?.height?.valueInMm || 160)
const tdpWatts = ref(model?.tdp?.valueInWatts || 180)
const selectedSockets = ref<CpuSocket[]>(model?.sockets || ['AM5', 'LGA1700'])
const dimensionLength = ref(model?.dimensions?.length || 120)
const dimensionWidth = ref(model?.dimensions?.width || 120)
const dimensionHeight = ref(model?.dimensions?.height || 160)

const coolerTypeOptions = (['Air', 'AIO', 'CustomLoop'] as CoolerType[]).map(v => ({ value: v, label: v }))
const socketOptions = (['LGA1700', 'LGA1200', 'LGA1151', 'LGA2066', 'AM5', 'AM4', 'sTRX4', 'TR4'] as CpuSocket[]).map(v => ({ value: v, label: v }))

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
    <FormItemSelect label="Cooler Type *" v-model="coolerType" :options="coolerTypeOptions" />

    <FormItemNumber label="Height (mm) *" v-model="heightMm" :min="0" />

    <FormItemNumber label="Max TDP (Watts) *" v-model="tdpWatts" :min="0" />

    <FormItemCheckboxGroup
      label="Supported Sockets *"
      v-model="selectedSockets"
      :options="socketOptions"
      class="col-span-2"
    />

    <DimensionsInput
      v-model:length="dimensionLength"
      v-model:width="dimensionWidth"
      v-model:height="dimensionHeight"
    />
  </div>
</template>
