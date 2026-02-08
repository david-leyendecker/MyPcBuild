<script setup lang="ts">
import { ref, watch } from 'vue'
import type { PcCaseProductRequest } from '@/types/product'
import Input from '@/components/ui/input/Input.vue'
import Label from '@/components/ui/label/Label.vue'

interface Props {
  modelValue?: Partial<PcCaseProductRequest>
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:modelValue': [value: Partial<PcCaseProductRequest>]
}>()

const formFactor = ref(props.modelValue?.formFactor || 'Mid Tower')
const color = ref(props.modelValue?.color || 'Black')
const sidePanelWindow = ref(props.modelValue?.sidePanelWindow || 'Tempered Glass')
const dimensionLength = ref(props.modelValue?.dimensions?.length || 450)
const dimensionWidth = ref(props.modelValue?.dimensions?.width || 210)
const dimensionHeight = ref(props.modelValue?.dimensions?.height || 460)

watch([formFactor, color, sidePanelWindow, dimensionLength, dimensionWidth, dimensionHeight], () => {
  emit('update:modelValue', {
    category: 'case',
    formFactor: formFactor.value,
    color: color.value,
    sidePanelWindow: sidePanelWindow.value,
    dimensions: {
      length: dimensionLength.value,
      width: dimensionWidth.value,
      height: dimensionHeight.value
    }
  })
})

defineExpose({
  getFormData: () => ({
    formFactor: formFactor.value,
    color: color.value,
    sidePanelWindow: sidePanelWindow.value,
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
      <select
        id="sidePanelWindow"
        v-model="sidePanelWindow"
        class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm"
      >
        <option value="None">None</option>
        <option value="Acrylic">Acrylic</option>
        <option value="Tempered Glass">Tempered Glass</option>
      </select>
    </div>

    <div class="space-y-2 col-span-2">
      <Label>Dimensions (mm) *</Label>
      <div class="grid grid-cols-3 gap-2">
        <div>
          <Input v-model.number="dimensionLength" type="number" placeholder="Length" min="0" />
        </div>
        <div>
          <Input v-model.number="dimensionWidth" type="number" placeholder="Width" min="0" />
        </div>
        <div>
          <Input v-model.number="dimensionHeight" type="number" placeholder="Height" min="0" />
        </div>
      </div>
    </div>
  </div>
</template>
