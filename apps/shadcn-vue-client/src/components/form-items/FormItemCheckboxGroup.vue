<script setup lang="ts">
import { useId, type HTMLAttributes } from 'vue'
import Label from '@/components/ui/label/Label.vue'

interface Option<T = string> {
  value: T
  label: string
}

interface Props<T = string> {
  label: string
  modelValue: T[]
  options: Option<T>[]
  error?: string
  disabled?: boolean
  class?: HTMLAttributes['class']
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:modelValue': [value: (typeof props)['modelValue']]
}>()

const baseId = useId()

function getId(index: number) {
  return `${baseId}-${index}`
}

function toggle(optionValue: (typeof props.options)[number]['value']) {
  const current = [...props.modelValue]
  const index = current.indexOf(optionValue)
  if (index === -1) {
    emit('update:modelValue', [...current, optionValue] as (typeof props)['modelValue'])
  } else {
    current.splice(index, 1)
    emit('update:modelValue', current as (typeof props)['modelValue'])
  }
}
</script>

<template>
  <div class="space-y-2" :class="props.class">
    <Label class="block">{{ label }}</Label>
    <div class="grid grid-cols-4 gap-2">
      <div
        v-for="(option, index) in options"
        :key="option.value"
        class="flex items-center space-x-2"
      >
        <input
          :id="getId(index)"
          type="checkbox"
          :checked="modelValue.includes(option.value)"
          :disabled="disabled"
          class="h-4 w-4 rounded border-gray-300"
          @change="toggle(option.value)"
        />
        <Label :for="getId(index)" class="text-sm cursor-pointer">
          {{ option.label }}
        </Label>
      </div>
    </div>
    <span v-if="error" class="text-sm text-red-500">{{ error }}</span>
  </div>
</template>
