<script setup lang="ts">
import { ref, watch } from 'vue'
import type { Rotation } from '@/types/spatial'
import Label from '@/components/ui/label/Label.vue'
import { FormItemNumber } from '@/components/form-items'

interface Props {
  modelValue?: Rotation | null
  editable?: boolean
  label?: string
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => ({ x: 0, y: 0, z: 0 }),
  editable: true,
  label: 'Rotation (degrees)'
})

const emit = defineEmits<{
  'update:modelValue': [value: Rotation | null]
}>()

const x = ref(props.modelValue?.x || 0)
const y = ref(props.modelValue?.y || 0)
const z = ref(props.modelValue?.z || 0)

watch([x, y, z], () => {
  emit('update:modelValue', { x: x.value, y: y.value, z: z.value })
})

watch(() => props.modelValue, (newValue) => {
  if (newValue) {
    x.value = newValue.x || 0
    y.value = newValue.y || 0
    z.value = newValue.z || 0
  }
}, { deep: true })
</script>

<template>
  <div class="space-y-2">
    <Label v-if="label">{{ label }}</Label>
    <div class="grid grid-cols-3 gap-2">
      <FormItemNumber
        label="X"
        hide-label
        v-model="x"
        placeholder="X"
        :disabled="!editable"
        :step="1"
      />
      <FormItemNumber
        label="Y"
        hide-label
        v-model="y"
        placeholder="Y"
        :disabled="!editable"
        :step="1"
      />
      <FormItemNumber
        label="Z"
        hide-label
        v-model="z"
        placeholder="Z"
        :disabled="!editable"
        :step="1"
      />
    </div>
  </div>
</template>
