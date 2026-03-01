<script setup lang="ts">
import { useId, type HTMLAttributes } from 'vue'
import FormSelect from '@/components/shared/FormSelect.vue'
import Label from '@/components/ui/label/Label.vue'
import { cn } from '@/lib/utils'

interface Props {
  label: string
  modelValue: string | number
  options: { value: string | number; label: string }[]
  error?: string
  disabled?: boolean
  class?: HTMLAttributes['class']
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:modelValue': [value: string | number]
}>()

const id = useId()
</script>

<template>
  <div class="space-y-2" :class="props.class">
    <Label :for="id">{{ label }}</Label>
    <FormSelect
      :id="id"
      :model-value="modelValue"
      :options="options"
      :disabled="disabled"
      :class="cn(error && 'border-red-500')"
      @update:model-value="emit('update:modelValue', $event)"
    />
    <span v-if="error" class="text-sm text-red-500">{{ error }}</span>
  </div>
</template>
