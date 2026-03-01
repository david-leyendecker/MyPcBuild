<script setup lang="ts">
import { useId, type HTMLAttributes } from 'vue'
import Input from '@/components/ui/input/Input.vue'
import Label from '@/components/ui/label/Label.vue'
import { cn } from '@/lib/utils'

interface Props {
  label?: string
  modelValue?: number
  placeholder?: string
  min?: number
  max?: number
  step?: number
  error?: string
  disabled?: boolean
  hideLabel?: boolean
  class?: HTMLAttributes['class']
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:modelValue': [value: number]
}>()

const id = useId()
</script>

<template>
  <div class="space-y-2" :class="props.class">
    <Label v-if="label && !hideLabel" :for="id">{{ label }}</Label>
    <Input
      :id="id"
      type="number"
      :model-value="modelValue"
      :placeholder="placeholder"
      :min="min"
      :max="max"
      :step="step"
      :disabled="disabled"
      :class="cn(error && 'border-red-500')"
      @update:model-value="emit('update:modelValue', Number($event))"
    />
    <span v-if="error" class="text-sm text-red-500">{{ error }}</span>
  </div>
</template>
