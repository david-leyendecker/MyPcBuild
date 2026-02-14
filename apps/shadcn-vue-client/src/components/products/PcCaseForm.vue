<script setup lang="ts">
import { ref, watch } from 'vue'
import type { PcCaseProductRequest, ProductRequest } from '@/types/product'
import type { Chamber } from '@/types/spatial'
import Input from '@/components/ui/input/Input.vue'
import Label from '@/components/ui/label/Label.vue'
import FormSelect from '@/components/shared/FormSelect.vue'
import DimensionsInput from '@/components/value-objects/DimensionsInput.vue'
import ChambersInput from '@/components/value-objects/ChambersInput.vue'

interface Props {
  modelValue?: Partial<ProductRequest>
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:modelValue': [value: Partial<PcCaseProductRequest>]
}>()

/** Narrow to PC Case shape - safe when this form is rendered for Case category */
const model = props.modelValue as Partial<PcCaseProductRequest> | undefined
const formFactor = ref(model?.formFactor || 'Mid Tower')
const color = ref(model?.color || 'Black')
const sidePanelWindow = ref(model?.sidePanelWindow || 'Tempered Glass')
const dimensions = ref(model?.dimensions || { length: 450, width: 210, height: 460 })
const chambers = ref<Chamber[]>(model?.chambers || [])

watch([formFactor, color, sidePanelWindow, dimensions, chambers], () => {
  emit('update:modelValue', {
    category: 'case',
    formFactor: formFactor.value,
    color: color.value,
    sidePanelWindow: sidePanelWindow.value,
    dimensions: dimensions.value,
    chambers: chambers.value
  })
}, { deep: true })

defineExpose({
  getFormData: () => ({
    formFactor: formFactor.value,
    color: color.value,
    sidePanelWindow: sidePanelWindow.value,
    dimensions: dimensions.value,
    chambers: chambers.value
  })
})
</script>

<template>
  <div class="grid gap-4 md:grid-cols-2">
    <div class="space-y-2">
      <Label for="formFactor">Form Factor *</Label>
      <Input id="formFactor" v-model="formFactor" type="text" placeholder="e.g., Mid Tower, Full Tower" />
    </div>

    <div class="space-y-2">
      <Label for="color">Color *</Label>
      <Input id="color" v-model="color" type="text" placeholder="e.g., Black, White" />
    </div>

    <div class="space-y-2">
      <Label for="sidePanelWindow">Side Panel Window *</Label>
      <FormSelect
        v-model="sidePanelWindow"
        :options="[
          { value: 'None', label: 'None' },
          { value: 'Acrylic', label: 'Acrylic' },
          { value: 'Tempered Glass', label: 'Tempered Glass' },
        ]"
      />
    </div>

    <div class="space-y-2 col-span-2">
      <DimensionsInput v-model="dimensions" label="Dimensions (mm) *" />
    </div>

    <div class="col-span-2">
      <ChambersInput v-model="chambers" label="Chambers (for spatial layout)" />
    </div>
  </div>
</template>
