<script setup lang="ts">
import { type HTMLAttributes, computed } from 'vue'
import { cn } from '@/lib/utils'

interface Props {
  modelValue: string | number
  options: { value: string | number; label: string }[]
  id?: string
  disabled?: boolean
  class?: HTMLAttributes['class']
}

const props = defineProps<Props>()
const emit = defineEmits<{
  'update:modelValue': [value: string | number]
}>()

function handleChange(event: Event) {
  const stringValue = (event.target as HTMLSelectElement).value
  const matched = props.options.find((o) => String(o.value) === stringValue)
  if (matched === undefined) {
    console.warn(`[FormSelect] No matching option found for value "${stringValue}"`)
    emit('update:modelValue', stringValue)
    return
  }
  emit('update:modelValue', matched.value)
}

const classes = computed(() =>
  cn(
    'flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50',
    props.class,
  ),
)
</script>

<template>
  <select
    :id="id"
    :value="modelValue"
    :disabled="disabled"
    :class="classes"
    @change="handleChange"
  >
    <option
      v-for="option in options"
      :key="option.value"
      :value="option.value"
    >
      {{ option.label }}
    </option>
  </select>
</template>
