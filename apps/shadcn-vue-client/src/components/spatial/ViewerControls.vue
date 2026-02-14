<template>
  <div class="viewer-controls">
    <Button
      size="sm"
      variant="ghost"
      @click="emit('toggle-grid')"
      :class="{ 'bg-accent': showGrid }"
      aria-label="Toggle grid"
    >
      <Grid class="h-4 w-4" />
    </Button>

    <Button
      v-if="showAxesToggle"
      size="sm"
      variant="ghost"
      @click="emit('toggle-axes')"
      :class="{ 'bg-accent': showAxes }"
      aria-label="Toggle axes"
    >
      <Maximize class="h-4 w-4" />
    </Button>

    <Button
      size="sm"
      variant="ghost"
      @click="emit('reset-camera')"
      aria-label="Reset camera view"
    >
      <RotateCcw class="h-4 w-4" />
    </Button>
  </div>
</template>

<script setup lang="ts">
import { Grid, Maximize, RotateCcw } from 'lucide-vue-next';
import { Button } from '@/components/ui/button';

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
