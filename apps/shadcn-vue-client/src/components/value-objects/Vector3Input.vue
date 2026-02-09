<script setup lang="ts">
import { ref, watch } from 'vue'
import type { Vector3 } from '@/types/spatial'
import Input from '@/components/ui/input/Input.vue'
import Label from '@/components/ui/label/Label.vue'

interface Props {
  modelValue?: Vector3
  editable?: boolean
  label?: string
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => ({ x: 0, y: 0, z: 0 }),
  editable: true,
  label: 'Position (mm)'
})

const emit = defineEmits<{
  'update:modelValue': [value: Vector3]
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
      <div>
        <Input
          v-model.number="x"
          type="number"
          placeholder="X"
          :disabled="!editable"
          step="0.1"
        />
      </div>
      <div>
        <Input
          v-model.number="y"
          type="number"
          placeholder="Y"
          :disabled="!editable"
          step="0.1"
        />
      </div>
      <div>
        <Input
          v-model.number="z"
          type="number"
          placeholder="Z"
          :disabled="!editable"
          step="0.1"
        />
      </div>
    </div>
  </div>
</template>
