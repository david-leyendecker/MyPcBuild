<script setup lang="ts">
import { ref, watch } from 'vue'
import type { DataSpeed } from '@/types/product'
import Input from '@/components/ui/input/Input.vue'

interface Props {
  modelValue?: DataSpeed
  editable?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => ({ valueInMBps: 0 }),
  editable: true
})

const emit = defineEmits<{
  'update:modelValue': [value: DataSpeed]
}>()

const value = ref(props.modelValue?.valueInMBps || 0)

watch(value, () => {
  emit('update:modelValue', { valueInMBps: value.value })
})

watch(() => props.modelValue, (newValue) => {
  if (newValue) {
    value.value = newValue.valueInMBps || 0
  }
})
</script>

<template>
  <div class="flex items-center gap-2">
    <Input
      v-model.number="value"
      type="number"
      :disabled="!editable"
      step="1"
      min="0"
      class="flex-1"
    />
    <span class="text-sm text-muted-foreground">MB/s</span>
  </div>
</template>
