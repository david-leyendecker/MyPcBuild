<script setup lang="ts">
import { ref, watch } from 'vue'
import type { RamProductRequest, MemoryType } from '@/types/product'
import Input from '@/components/ui/input/Input.vue'
import Label from '@/components/ui/label/Label.vue'

interface Props {
  modelValue?: Partial<RamProductRequest>
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:modelValue': [value: Partial<RamProductRequest>]
}>()

const type = ref<MemoryType>(props.modelValue?.type || 'DDR5')
const capacityGB = ref(props.modelValue?.capacity?.valueInGB || 16)
const configuration = ref(props.modelValue?.configuration || '2x8GB')
const speedGHz = ref(props.modelValue?.speed?.valueInGHz || 3.2)
const casLatency = ref(props.modelValue?.casLatency || 'CL16')
const voltageVolts = ref(props.modelValue?.voltage?.valueInVolts || 1.35)

const memoryTypes: MemoryType[] = ['DDR3', 'DDR4', 'DDR5']

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
    <div class="space-y-2">
      <Label for="type">Memory Type *</Label>
      <select
        id="type"
        v-model="type"
        class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm"
      >
        <option v-for="opt in memoryTypes" :key="opt" :value="opt">{{ opt }}</option>
      </select>
    </div>

    <div class="space-y-2">
      <Label for="capacity">Capacity (GB) *</Label>
      <Input id="capacity" v-model.number="capacityGB" type="number" min="1" />
    </div>

    <div class="space-y-2">
      <Label for="configuration">Configuration *</Label>
      <Input id="configuration" v-model="configuration" type="text" placeholder="e.g., 2x8GB" />
    </div>

    <div class="space-y-2">
      <Label for="speed">Speed (GHz) *</Label>
      <Input id="speed" v-model.number="speedGHz" type="number" step="0.1" min="0" />
    </div>

    <div class="space-y-2">
      <Label for="casLatency">CAS Latency *</Label>
      <Input id="casLatency" v-model="casLatency" type="text" placeholder="e.g., CL16" />
    </div>

    <div class="space-y-2">
      <Label for="voltage">Voltage (V) *</Label>
      <Input id="voltage" v-model.number="voltageVolts" type="number" step="0.01" min="0" />
    </div>
  </div>
</template>
