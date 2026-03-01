<script setup lang="ts">
import { useId, type HTMLAttributes } from 'vue'
import Label from '@/components/ui/label/Label.vue'

interface Option<T = string> {
  value: T
  label: string
}

interface Props<T = string> {
  label: string
  modelValue: T
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
</script>

<template>
  <div class="space-y-2" :class="props.class">
    <Label class="block">{{ label }}</Label>
    <div class="space-y-2">
      <div
        v-for="(option, index) in options"
        :key="option.value"
        class="flex items-center space-x-2"
      >
        <input
          :id="getId(index)"
          type="radio"
          :name="baseId"
          :value="option.value"
          :checked="modelValue === option.value"
          :disabled="disabled"
          class="h-4 w-4"
          @change="emit('update:modelValue', option.value as typeof modelValue)"
        />
        <Label :for="getId(index)" class="text-sm font-normal cursor-pointer">
          {{ option.label }}
        </Label>
      </div>
    </div>
    <span v-if="error" class="text-sm text-red-500">{{ error }}</span>
  </div>
</template>
