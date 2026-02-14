<script setup lang="ts">
import { ref, watch } from 'vue'
import type { PsuProductRequest, ProductRequest } from '@/types/product'
import Input from '@/components/ui/input/Input.vue'
import Label from '@/components/ui/label/Label.vue'
import FormSelect from '@/components/shared/FormSelect.vue'

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
const efficiency = ref(model?.efficiency || '80+ Gold')
const modular = ref(model?.modular || 'Full')
const formFactor = ref(model?.formFactor || 'ATX')
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
      <FormSelect
        v-model="modular"
        :options="[
          { value: 'Full', label: 'Full Modular' },
          { value: 'Semi', label: 'Semi Modular' },
          { value: 'Non', label: 'Non Modular' },
        ]"
      />
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
