<script setup lang="ts">
import { useId, type HTMLAttributes } from 'vue'
import Label from '@/components/ui/label/Label.vue'

interface Props {
  label: string
  modelValue?: boolean
  error?: string
  disabled?: boolean
  class?: HTMLAttributes['class']
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:modelValue': [value: boolean]
}>()

const id = useId()
</script>

<template>
  <div class="flex items-center space-x-2" :class="props.class">
    <input
      :id="id"
      type="checkbox"
      :checked="modelValue"
      :disabled="disabled"
      class="h-4 w-4 rounded border-gray-300"
      @change="emit('update:modelValue', ($event.target as HTMLInputElement).checked)"
    />
    <Label :for="id" class="cursor-pointer">{{ label }}</Label>
    <span v-if="error" class="text-sm text-red-500">{{ error }}</span>
  </div>
</template>
