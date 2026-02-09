<script setup lang="ts">
import { ref } from 'vue'

interface Props {
  defaultOpen?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  defaultOpen: false
})

const isOpen = ref(props.defaultOpen)

function toggle() {
  isOpen.value = !isOpen.value
}

defineExpose({ isOpen, toggle })
</script>

<template>
  <div class="space-y-2">
    <slot name="trigger" :is-open="isOpen" :toggle="toggle" />
    <div v-if="isOpen" class="pl-4 border-l-2 border-border">
      <slot />
    </div>
  </div>
</template>
