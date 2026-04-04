<script setup lang="ts">
import { ref, watch } from 'vue'
import type { PsuProductRequest, PsuEfficiency, PsuModularity, PsuFormFactor, ProductRequest } from '@/types/product'
import { FormItemNumber, FormItemSelect } from '@/components/form-items'
import { psuEfficiencyOptions, psuModularityOptions, psuFormFactorOptions } from '@/constants/enumOptions'

interface Props {
  modelValue?: Partial<ProductRequest>
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:modelValue': [value: Partial<PsuProductRequest>]
}>()

/** Narrow to PSU shape - safe when this form is rendered for PSU category */
const model = props.modelValue as Partial<PsuProductRequest> | undefined
const wattageWatts = ref(model?.wattage?.valueInWatts || 750)
const efficiency = ref<PsuEfficiency>(model?.efficiency || 'Gold')
const modular = ref<PsuModularity>(model?.modular || 'FullyModular')
const formFactor = ref<PsuFormFactor>(model?.formFactor || 'ATX')
const lengthMm = ref(model?.length?.valueInMm || 160)
const pcie8Pin = ref(model?.pcie8Pin || 4)

watch([wattageWatts, efficiency, modular, formFactor, lengthMm, pcie8Pin], () => {
  emit('update:modelValue', {
    category: 'powersupply',
    wattage: { valueInWatts: wattageWatts.value },
    efficiency: efficiency.value,
    modular: modular.value,
    formFactor: formFactor.value,
    length: { valueInMm: lengthMm.value },
    pcie8Pin: pcie8Pin.value
  })
})

defineExpose({
  getFormData: () => ({
    wattage: { valueInWatts: wattageWatts.value },
    efficiency: efficiency.value,
    modular: modular.value,
    formFactor: formFactor.value,
    length: { valueInMm: lengthMm.value },
    pcie8Pin: pcie8Pin.value
  })
})
</script>

<template>
  <div class="grid gap-4 md:grid-cols-2">
    <FormItemNumber label="Wattage (W) *" v-model="wattageWatts" :min="1" />

    <FormItemSelect
      label="Efficiency Certification *"
      v-model="efficiency"
      :options="psuEfficiencyOptions"
    />

    <FormItemSelect
      label="Modular Type *"
      v-model="modular"
      :options="psuModularityOptions"
    />

    <FormItemSelect
      label="Form Factor *"
      v-model="formFactor"
      :options="psuFormFactorOptions"
    />

    <FormItemNumber label="Length (mm) *" v-model="lengthMm" :min="0" />

    <FormItemNumber label="PCIe 8-Pin Connectors *" v-model="pcie8Pin" :min="0" />
  </div>
</template>
