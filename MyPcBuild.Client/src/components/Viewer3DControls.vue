<template>
  <div class="viewer-controls">
    <n-button
      size="small"
      text
      @click="emit('toggle-grid')"
      :type="showGrid ? 'primary' : 'default'"
    >
      <n-tooltip>
        <template #trigger>
          <span>☷</span>
        </template>
        Toggle Grid
      </n-tooltip>
    </n-button>

    <n-button
      v-if="showAxesToggle"
      size="small"
      text
      @click="emit('toggle-axes')"
      :type="showAxes ? 'primary' : 'default'"
    >
      <n-tooltip>
        <template #trigger>
          <span>⚏</span>
        </template>
        Toggle Axes
      </n-tooltip>
    </n-button>

    <n-button
      size="small"
      text
      @click="emit('reset-camera')"
    >
      <n-tooltip>
        <template #trigger>
          <span>↻</span>
        </template>
        Reset View
      </n-tooltip>
    </n-button>
  </div>
</template>

<script setup lang="ts">
import { NButton, NTooltip } from 'naive-ui';

interface Props {
  showGrid: boolean;
  showAxes?: boolean;
  showAxesToggle?: boolean;
}

withDefaults(defineProps<Props>(), {
  showAxes: false,
  showAxesToggle: false
});

const emit = defineEmits<{
  'toggle-grid': [];
  'toggle-axes': [];
  'reset-camera': [];
}>();
</script>

<style scoped>
.viewer-controls {
  position: absolute;
  top: 10px;
  right: 10px;
  display: flex;
  flex-direction: column;
  gap: 8px;
  z-index: 10;
}
</style>
