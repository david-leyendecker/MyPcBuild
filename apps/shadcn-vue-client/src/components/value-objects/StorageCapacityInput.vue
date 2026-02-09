<script setup lang="ts">
import { ref, watch } from 'vue'
import type { StorageCapacity } from '@/types/product'
import Input from '@/components/ui/input/Input.vue'

interface Props {
  modelValue?: StorageCapacity
  editable?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => ({ valueInGB: 0 }),
  editable: true
})

const emit = defineEmits<{
  'update:modelValue': [value: StorageCapacity]
}>()

const value = ref(props.modelValue?.valueInGB || 0)

watch(value, () => {
  emit('update:modelValue', { valueInGB: value.value })
})

watch(() => props.modelValue, (newValue) => {
  if (newValue) {
    value.value = newValue.valueInGB || 0
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
    <span class="text-sm text-muted-foreground">GB</span>
  </div>
</template>
