<script setup lang="ts">
import Input from '@/components/ui/input/Input.vue'
import Label from '@/components/ui/label/Label.vue'

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
      <div>
        <Input
          :model-value="length"
          type="number"
          placeholder="Length"
          min="0"
          @update:model-value="emit('update:length', Number($event))"
        />
      </div>
      <div>
        <Input
          :model-value="width"
          type="number"
          placeholder="Width"
          min="0"
          @update:model-value="emit('update:width', Number($event))"
        />
      </div>
      <div>
        <Input
          :model-value="height"
          type="number"
          placeholder="Height"
          min="0"
          @update:model-value="emit('update:height', Number($event))"
        />
      </div>
    </div>
  </div>
</template>
