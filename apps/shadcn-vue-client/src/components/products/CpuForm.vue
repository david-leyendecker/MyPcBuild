<script setup lang="ts">
import { ref, watch } from 'vue'
import type { CpuProductRequest, CpuSocket, ProductRequest } from '@/types/product'
import { FormItemSelect, FormItemNumber, FormItemCheckbox } from '@/components/form-items'

interface Props {
  modelValue?: Partial<ProductRequest>
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:modelValue': [value: Partial<CpuProductRequest>]
}>()

/** Narrow to CPU shape - safe when this form is rendered for CPU category */
const model = props.modelValue as Partial<CpuProductRequest> | undefined
const socket = ref<CpuSocket>(model?.socket || 'AM5')
const cores = ref(model?.cores || 8)
const threads = ref(model?.threads || 16)
const baseClockGHz = ref(model?.baseClock?.valueInGHz || 3.5)
const boostClockGHz = ref(model?.boostClock?.valueInGHz || 5.0)
const tdpWatts = ref(model?.tdp?.valueInWatts || 105)
const integratedGraphics = ref(model?.integratedGraphics || false)

const socketOptions = (['LGA1700', 'LGA1200', 'LGA1151', 'LGA2066', 'AM5', 'AM4', 'sTRX4', 'TR4'] as CpuSocket[]).map(v => ({ value: v, label: v }))

watch([socket, cores, threads, baseClockGHz, boostClockGHz, tdpWatts, integratedGraphics], () => {
  emit('update:modelValue', {
    category: 'cpu',
    socket: socket.value,
    cores: cores.value,
    threads: threads.value,
    baseClock: { valueInGHz: baseClockGHz.value },
    boostClock: { valueInGHz: boostClockGHz.value },
    tdp: { valueInWatts: tdpWatts.value },
    integratedGraphics: integratedGraphics.value
  })
})

defineExpose({
  getFormData: () => ({
    socket: socket.value,
    cores: cores.value,
    threads: threads.value,
    baseClock: { valueInGHz: baseClockGHz.value },
    boostClock: { valueInGHz: boostClockGHz.value },
    tdp: { valueInWatts: tdpWatts.value },
    integratedGraphics: integratedGraphics.value
  })
})
</script>

<template>
  <div class="grid gap-4 md:grid-cols-2">
    <FormItemSelect label="CPU Socket *" v-model="socket" :options="socketOptions" />

    <FormItemNumber label="Cores *" v-model="cores" :min="1" />

    <FormItemNumber label="Threads *" v-model="threads" :min="1" />

    <FormItemNumber label="Base Clock (GHz) *" v-model="baseClockGHz" :step="0.1" :min="0" />

    <FormItemNumber label="Boost Clock (GHz) *" v-model="boostClockGHz" :step="0.1" :min="0" />

    <FormItemNumber label="TDP (Watts) *" v-model="tdpWatts" :min="0" />

    <FormItemCheckbox label="Integrated Graphics" v-model="integratedGraphics" />
  </div>
</template>
