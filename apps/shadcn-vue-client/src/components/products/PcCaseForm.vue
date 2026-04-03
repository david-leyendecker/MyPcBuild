<script setup lang="ts">
import { ref, watch } from 'vue'
import type { PcCaseProductRequest, ProductRequest } from '@/types/product'
import type { Chamber } from '@/types/spatial'
import { FormItemText, FormItemSelect } from '@/components/form-items'
import { formFactorOptions, sidePanelTypeOptions } from '@/constants/enumOptions'
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
const formFactor = ref(model?.formFactor || 'ATX')
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
    <FormItemSelect
      label="Form Factor *"
      v-model="formFactor"
      :options="formFactorOptions"
    />

    <FormItemText label="Color *" v-model="color" placeholder="e.g., Black, White" />

    <FormItemSelect
      label="Side Panel Window *"
      v-model="sidePanelWindow"
      :options="sidePanelTypeOptions"
    />

    <div class="col-span-2">
      <DimensionsInput v-model="dimensions" label="Dimensions (mm) *" />
    </div>

    <div class="col-span-2">
      <ChambersInput v-model="chambers" label="Chambers (for spatial layout)" />
    </div>
  </div>
</template>
