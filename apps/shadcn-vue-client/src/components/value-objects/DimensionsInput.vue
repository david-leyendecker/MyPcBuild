<script setup lang="ts">
import { ref, watch } from 'vue'
import type { Dimensions } from '@/types/spatial'
import Label from '@/components/ui/label/Label.vue'
import { FormItemNumber } from '@/components/form-items'

interface Props {
  modelValue?: Dimensions
  editable?: boolean
  label?: string
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => ({ length: 0, width: 0, height: 0 }),
  editable: true,
  label: 'Dimensions (mm)'
})

const emit = defineEmits<{
  'update:modelValue': [value: Dimensions]
}>()

const length = ref(props.modelValue?.length || 0)
const width = ref(props.modelValue?.width || 0)
const height = ref(props.modelValue?.height || 0)

watch([length, width, height], () => {
  emit('update:modelValue', {
    length: length.value,
    width: width.value,
    height: height.value
  })
})

watch(() => props.modelValue, (newValue) => {
  if (newValue) {
    length.value = newValue.length || 0
    width.value = newValue.width || 0
    height.value = newValue.height || 0
  }
}, { deep: true })
</script>

<template>
  <div class="space-y-2">
    <Label v-if="label">{{ label }}</Label>
    <div class="grid grid-cols-3 gap-2">
      <FormItemNumber
        label="Length"
        hide-label
        v-model="length"
        placeholder="Length"
        :disabled="!editable"
        :min="0"
        :step="0.1"
      />
      <FormItemNumber
        label="Width"
        hide-label
        v-model="width"
        placeholder="Width"
        :disabled="!editable"
        :min="0"
        :step="0.1"
      />
      <FormItemNumber
        label="Height"
        hide-label
        v-model="height"
        placeholder="Height"
        :disabled="!editable"
        :min="0"
        :step="0.1"
      />
    </div>
  </div>
</template>
