<script setup lang="ts">
import { ref, watch } from 'vue'
import type { Dimensions } from '@/types/spatial'
import Input from '@/components/ui/input/Input.vue'
import Label from '@/components/ui/label/Label.vue'

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
      <div>
        <Input
          v-model.number="length"
          type="number"
          placeholder="Length"
          :disabled="!editable"
          min="0"
          step="0.1"
        />
      </div>
      <div>
        <Input
          v-model.number="width"
          type="number"
          placeholder="Width"
          :disabled="!editable"
          min="0"
          step="0.1"
        />
      </div>
      <div>
        <Input
          v-model.number="height"
          type="number"
          placeholder="Height"
          :disabled="!editable"
          min="0"
          step="0.1"
        />
      </div>
    </div>
  </div>
</template>
