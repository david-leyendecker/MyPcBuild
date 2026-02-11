<script setup lang="ts">
import { ref, watch } from 'vue'
import type { CpuProductRequest, CpuSocket } from '@/types/product'
import Input from '@/components/ui/input/Input.vue'
import Label from '@/components/ui/label/Label.vue'
import FormSelect from '@/components/shared/FormSelect.vue'

interface Props {
  modelValue?: Partial<CpuProductRequest>
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:modelValue': [value: Partial<CpuProductRequest>]
}>()

const socket = ref<CpuSocket>(props.modelValue?.socket || 'AM5')
const cores = ref(props.modelValue?.cores || 8)
const threads = ref(props.modelValue?.threads || 16)
const baseClockGHz = ref(props.modelValue?.baseClock?.valueInGHz || 3.5)
const boostClockGHz = ref(props.modelValue?.boostClock?.valueInGHz || 5.0)
const tdpWatts = ref(props.modelValue?.tdp?.valueInWatts || 105)
const integratedGraphics = ref(props.modelValue?.integratedGraphics || false)

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
    <div class="space-y-2">
      <Label for="socket">CPU Socket *</Label>
      <FormSelect v-model="socket" :options="socketOptions" />
    </div>

    <div class="space-y-2">
      <Label for="cores">Cores *</Label>
      <Input id="cores" v-model.number="cores" type="number" min="1" />
    </div>

    <div class="space-y-2">
      <Label for="threads">Threads *</Label>
      <Input id="threads" v-model.number="threads" type="number" min="1" />
    </div>

    <div class="space-y-2">
      <Label for="baseClock">Base Clock (GHz) *</Label>
      <Input id="baseClock" v-model.number="baseClockGHz" type="number" step="0.1" min="0" />
    </div>

    <div class="space-y-2">
      <Label for="boostClock">Boost Clock (GHz) *</Label>
      <Input id="boostClock" v-model.number="boostClockGHz" type="number" step="0.1" min="0" />
    </div>

    <div class="space-y-2">
      <Label for="tdp">TDP (Watts) *</Label>
      <Input id="tdp" v-model.number="tdpWatts" type="number" min="0" />
    </div>

    <div class="space-y-2 flex items-center">
      <input
        id="integratedGraphics"
        v-model="integratedGraphics"
        type="checkbox"
        class="h-4 w-4 rounded border-gray-300"
      />
      <Label for="integratedGraphics" class="ml-2 cursor-pointer">
        Integrated Graphics
      </Label>
    </div>
  </div>
</template>
