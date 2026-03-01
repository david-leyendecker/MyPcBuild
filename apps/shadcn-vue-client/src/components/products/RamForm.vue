<script setup lang="ts">
import { ref, watch } from 'vue'
import type { RamProductRequest, MemoryType, ProductRequest } from '@/types/product'
import { FormItemSelect, FormItemNumber, FormItemText } from '@/components/form-items'

interface Props {
  modelValue?: Partial<ProductRequest>
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:modelValue': [value: Partial<RamProductRequest>]
}>()

/** Narrow to RAM shape - safe when this form is rendered for RAM category */
const model = props.modelValue as Partial<RamProductRequest> | undefined
const type = ref<MemoryType>(model?.type || 'DDR5')
const capacityGB = ref(model?.capacity?.valueInGB || 16)
const configuration = ref(model?.configuration || '2x8GB')
const speedGHz = ref(model?.speed?.valueInGHz || 3.2)
const casLatency = ref(model?.casLatency || 'CL16')
const voltageVolts = ref(model?.voltage?.valueInVolts || 1.35)

const memoryTypeOptions = (['DDR3', 'DDR4', 'DDR5'] as MemoryType[]).map(v => ({ value: v, label: v }))

watch([type, capacityGB, configuration, speedGHz, casLatency, voltageVolts], () => {
  emit('update:modelValue', {
    category: 'ram',
    type: type.value,
    capacity: { valueInGB: capacityGB.value },
    configuration: configuration.value,
    speed: { valueInGHz: speedGHz.value },
    casLatency: casLatency.value,
    voltage: { valueInVolts: voltageVolts.value }
  })
})

defineExpose({
  getFormData: () => ({
    type: type.value,
    capacity: { valueInGB: capacityGB.value },
    configuration: configuration.value,
    speed: { valueInGHz: speedGHz.value },
    casLatency: casLatency.value,
    voltage: { valueInVolts: voltageVolts.value }
  })
})
</script>

<template>
  <div class="grid gap-4 md:grid-cols-2">
    <FormItemSelect label="Memory Type *" v-model="type" :options="memoryTypeOptions" />

    <FormItemNumber label="Capacity (GB) *" v-model="capacityGB" :min="1" />

    <FormItemText label="Configuration *" v-model="configuration" placeholder="e.g., 2x8GB" />

    <FormItemNumber label="Speed (GHz) *" v-model="speedGHz" :step="0.1" :min="0" />

    <FormItemText label="CAS Latency *" v-model="casLatency" placeholder="e.g., CL16" />

    <FormItemNumber label="Voltage (V) *" v-model="voltageVolts" :step="0.01" :min="0" />
  </div>
</template>
