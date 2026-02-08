<script setup lang="ts">
import { ref, watch } from 'vue'
import type { PsuProductRequest } from '@/types/product'
import Input from '@/components/ui/input/Input.vue'
import Label from '@/components/ui/label/Label.vue'

interface Props {
  modelValue?: Partial<PsuProductRequest>
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:modelValue': [value: Partial<PsuProductRequest>]
}>()

const wattageWatts = ref(props.modelValue?.wattage?.valueInWatts || 750)
const efficiency = ref(props.modelValue?.efficiency || '80+ Gold')
const modular = ref(props.modelValue?.modular || 'Full')
const formFactor = ref(props.modelValue?.formFactor || 'ATX')
const lengthMm = ref(props.modelValue?.length?.valueInMm || 160)
const pcie8Pin = ref(props.modelValue?.pcie8Pin || 4)

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
    <div class="space-y-2">
      <Label for="wattage">Wattage (W) *</Label>
      <Input id="wattage" v-model.number="wattageWatts" type="number" min="1" />
    </div>

    <div class="space-y-2">
      <Label for="efficiency">Efficiency Certification *</Label>
      <Input id="efficiency" v-model="efficiency" type="text" placeholder="e.g., 80+ Gold" />
    </div>

    <div class="space-y-2">
      <Label for="modular">Modular Type *</Label>
      <select
        id="modular"
        v-model="modular"
        class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm"
      >
        <option value="Full">Full Modular</option>
        <option value="Semi">Semi Modular</option>
        <option value="Non">Non Modular</option>
      </select>
    </div>

    <div class="space-y-2">
      <Label for="formFactor">Form Factor *</Label>
      <Input id="formFactor" v-model="formFactor" type="text" placeholder="e.g., ATX, SFX" />
    </div>

    <div class="space-y-2">
      <Label for="length">Length (mm) *</Label>
      <Input id="length" v-model.number="lengthMm" type="number" min="0" />
    </div>

    <div class="space-y-2">
      <Label for="pcie8Pin">PCIe 8-Pin Connectors *</Label>
      <Input id="pcie8Pin" v-model.number="pcie8Pin" type="number" min="0" />
    </div>
  </div>
</template>
