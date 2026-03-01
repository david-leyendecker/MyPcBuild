<script setup lang="ts">
import { useId, type HTMLAttributes } from 'vue'
import Input from '@/components/ui/input/Input.vue'
import Label from '@/components/ui/label/Label.vue'
import { cn } from '@/lib/utils'

interface Props {
  label: string
  modelValue?: string
  placeholder?: string
  error?: string
  disabled?: boolean
  class?: HTMLAttributes['class']
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

const id = useId()
</script>

<template>
  <div class="space-y-2" :class="props.class">
    <Label :for="id">{{ label }}</Label>
    <Input
      :id="id"
      type="text"
      :model-value="modelValue"
      :placeholder="placeholder"
      :disabled="disabled"
      :class="cn(error && 'border-red-500')"
      @update:model-value="emit('update:modelValue', $event as string)"
    />
    <span v-if="error" class="text-sm text-red-500">{{ error }}</span>
  </div>
</template>
