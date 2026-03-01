<script setup lang="ts">
import Label from '@/components/ui/label/Label.vue'
import { FormItemNumber } from '@/components/form-items'

interface Props {
  length: number
  width: number
  height: number
  label?: string
}

const props = withDefaults(defineProps<Props>(), {
  label: 'Dimensions (mm) *'
})

const emit = defineEmits<{
  'update:length': [value: number]
  'update:width': [value: number]
  'update:height': [value: number]
}>()
</script>

<template>
  <div class="space-y-2 col-span-2">
    <Label>{{ label }}</Label>
    <div class="grid grid-cols-3 gap-2">
      <FormItemNumber
        label="Length"
        hide-label
        :model-value="length"
        placeholder="Length"
        :min="0"
        @update:model-value="emit('update:length', $event)"
      />
      <FormItemNumber
        label="Width"
        hide-label
        :model-value="width"
        placeholder="Width"
        :min="0"
        @update:model-value="emit('update:width', $event)"
      />
      <FormItemNumber
        label="Height"
        hide-label
        :model-value="height"
        placeholder="Height"
        :min="0"
        @update:model-value="emit('update:height', $event)"
      />
    </div>
  </div>
</template>
