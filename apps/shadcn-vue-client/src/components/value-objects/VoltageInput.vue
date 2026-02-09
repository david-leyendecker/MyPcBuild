<script setup lang="ts">
import { ref, watch } from 'vue'
import type { Voltage } from '@/types/product'
import Input from '@/components/ui/input/Input.vue'

interface Props {
  modelValue?: Voltage
  editable?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => ({ valueInVolts: 0 }),
  editable: true
})

const emit = defineEmits<{
  'update:modelValue': [value: Voltage]
}>()

const value = ref(props.modelValue?.valueInVolts || 0)

watch(value, () => {
  emit('update:modelValue', { valueInVolts: value.value })
})

watch(() => props.modelValue, (newValue) => {
  if (newValue) {
    value.value = newValue.valueInVolts || 0
  }
})
</script>

<template>
  <div class="flex items-center gap-2">
    <Input
      v-model.number="value"
      type="number"
      :disabled="!editable"
      step="0.1"
      min="0"
      class="flex-1"
    />
    <span class="text-sm text-muted-foreground">V</span>
  </div>
</template>
