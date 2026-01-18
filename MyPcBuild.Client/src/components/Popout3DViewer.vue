<template>
  <Teleport to="body">
    <div 
      v-if="isOpen"
      :class="['popout-viewer', { 'minimized': isMinimized, 'maximized': isMaximized, 'dragging': isDragging }]"
      :style="viewerStyle"
      @mousedown="startDrag"
    >
      <!-- Header -->
      <div class="popout-header">
        <div class="popout-title">
          <v-icon size="small" class="mr-2">mdi-cube-outline</v-icon>
          <span>{{ title }}</span>
        </div>
        <div class="popout-controls">
          <v-btn
            icon
            size="x-small"
            variant="text"
            @click.stop="toggleMinimize"
            :title="isMinimized ? 'Restore' : 'Minimize'"
          >
            <v-icon size="small">{{ isMinimized ? 'mdi-window-restore' : 'mdi-window-minimize' }}</v-icon>
          </v-btn>
          <v-btn
            icon
            size="x-small"
            variant="text"
            @click.stop="toggleMaximize"
            :title="isMaximized ? 'Restore' : 'Maximize'"
          >
            <v-icon size="small">{{ isMaximized ? 'mdi-window-maximize' : 'mdi-fullscreen' }}</v-icon>
          </v-btn>
          <v-btn
            icon
            size="x-small"
            variant="text"
            @click.stop="close"
            title="Close"
          >
            <v-icon size="small">mdi-close</v-icon>
          </v-btn>
        </div>
      </div>

      <!-- Content -->
      <div v-show="!isMinimized" class="popout-content">
        <slot></slot>
      </div>

      <!-- Resize Handle (bottom-right corner) -->
      <div 
        v-show="!isMinimized && !isMaximized"
        class="resize-handle"
        @mousedown.stop="startResize"
      >
        <v-icon size="small">mdi-resize-bottom-right</v-icon>
      </div>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue';

interface Props {
  title?: string;
  initialWidth?: number;
  initialHeight?: number;
  initialX?: number;
  initialY?: number;
  minWidth?: number;
  minHeight?: number;
}

const props = withDefaults(defineProps<Props>(), {
  title: '3D Preview',
  initialWidth: 400,
  initialHeight: 400,
  initialX: undefined,
  initialY: undefined,
  minWidth: 300,
  minHeight: 300,
});

const emit = defineEmits<{
  close: [];
  resize: [];
}>();

const isOpen = ref(true);
const isMinimized = ref(false);
const isMaximized = ref(false);
const isDragging = ref(false);
const isResizing = ref(false);

const width = ref(props.initialWidth);
const height = ref(props.initialHeight);
const x = ref(props.initialX ?? window.innerWidth - props.initialWidth - 20);
const y = ref(props.initialY ?? 20);

const dragStartX = ref(0);
const dragStartY = ref(0);
const resizeStartWidth = ref(0);
const resizeStartHeight = ref(0);
const resizeStartX = ref(0);
const resizeStartY = ref(0);

// Store pre-maximize state
const preMaximizeState = ref({
  width: width.value,
  height: height.value,
  x: x.value,
  y: y.value,
});

const viewerStyle = computed(() => {
  if (isMaximized.value) {
    return {
      top: '0px',
      left: '0px',
      width: '100vw',
      height: '100vh',
    };
  }
  
  if (isMinimized.value) {
    return {
      top: `${y.value}px`,
      left: `${x.value}px`,
      width: '250px',
      height: 'auto',
    };
  }

  return {
    top: `${y.value}px`,
    left: `${x.value}px`,
    width: `${width.value}px`,
    height: `${height.value}px`,
  };
});

function toggleMinimize() {
  isMinimized.value = !isMinimized.value;
}

function toggleMaximize() {
  if (isMaximized.value) {
    // Restore
    width.value = preMaximizeState.value.width;
    height.value = preMaximizeState.value.height;
    x.value = preMaximizeState.value.x;
    y.value = preMaximizeState.value.y;
    isMaximized.value = false;
  } else {
    // Save current state
    preMaximizeState.value = {
      width: width.value,
      height: height.value,
      x: x.value,
      y: y.value,
    };
    isMaximized.value = true;
  }
  
  // Emit resize event so child components can adjust
  setTimeout(() => emit('resize'), 50);
}

function close() {
  isOpen.value = false;
  emit('close');
}

function startDrag(event: MouseEvent) {
  if (isMaximized.value || isMinimized.value) return;
  if ((event.target as HTMLElement).closest('.popout-controls')) return;
  if ((event.target as HTMLElement).closest('.resize-handle')) return;
  
  isDragging.value = true;
  dragStartX.value = event.clientX - x.value;
  dragStartY.value = event.clientY - y.value;
  
  document.addEventListener('mousemove', onDrag);
  document.addEventListener('mouseup', stopDrag);
  event.preventDefault();
}

function onDrag(event: MouseEvent) {
  if (!isDragging.value) return;
  
  x.value = event.clientX - dragStartX.value;
  y.value = event.clientY - dragStartY.value;
  
  // Keep within viewport
  x.value = Math.max(0, Math.min(x.value, window.innerWidth - width.value));
  y.value = Math.max(0, Math.min(y.value, window.innerHeight - 50));
}

function stopDrag() {
  isDragging.value = false;
  document.removeEventListener('mousemove', onDrag);
  document.removeEventListener('mouseup', stopDrag);
}

function startResize(event: MouseEvent) {
  isResizing.value = true;
  resizeStartX.value = event.clientX;
  resizeStartY.value = event.clientY;
  resizeStartWidth.value = width.value;
  resizeStartHeight.value = height.value;
  
  document.addEventListener('mousemove', onResize);
  document.addEventListener('mouseup', stopResize);
  event.preventDefault();
}

function onResize(event: MouseEvent) {
  if (!isResizing.value) return;
  
  const deltaX = event.clientX - resizeStartX.value;
  const deltaY = event.clientY - resizeStartY.value;
  
  width.value = Math.max(props.minWidth, resizeStartWidth.value + deltaX);
  height.value = Math.max(props.minHeight, resizeStartHeight.value + deltaY);
  
  // Keep within viewport
  if (x.value + width.value > window.innerWidth) {
    width.value = window.innerWidth - x.value;
  }
  if (y.value + height.value > window.innerHeight) {
    height.value = window.innerHeight - y.value;
  }
}

function stopResize() {
  isResizing.value = false;
  document.removeEventListener('mousemove', onResize);
  document.removeEventListener('mouseup', stopResize);
  
  // Emit resize event so child components can adjust
  emit('resize');
}

onMounted(() => {
  // Ensure initial position is within viewport
  if (props.initialX === undefined || props.initialY === undefined) {
    x.value = Math.max(0, Math.min(x.value, window.innerWidth - width.value));
    y.value = Math.max(0, Math.min(y.value, window.innerHeight - height.value));
  }
});

onUnmounted(() => {
  document.removeEventListener('mousemove', onDrag);
  document.removeEventListener('mouseup', stopDrag);
  document.removeEventListener('mousemove', onResize);
  document.removeEventListener('mouseup', stopResize);
});

// Expose methods for external control
defineExpose({
  open: () => { isOpen.value = true; },
  close,
  toggleMinimize,
  toggleMaximize,
});
</script>

<style scoped>
.popout-viewer {
  position: fixed;
  background: #1e1e1e;
  border: 1px solid rgba(255, 255, 255, 0.12);
  border-radius: 8px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.5);
  z-index: 9999;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  transition: box-shadow 0.2s;
}

.popout-viewer:hover {
  box-shadow: 0 12px 48px rgba(0, 0, 0, 0.7);
}

.popout-viewer.dragging {
  cursor: move;
  user-select: none;
}

.popout-viewer.maximized {
  border-radius: 0;
}

.popout-viewer.minimized {
  height: auto !important;
}

.popout-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 12px;
  background: #2a2a2a;
  border-bottom: 1px solid rgba(255, 255, 255, 0.08);
  cursor: move;
  user-select: none;
}

.popout-title {
  display: flex;
  align-items: center;
  font-size: 14px;
  font-weight: 500;
  color: rgba(255, 255, 255, 0.87);
}

.popout-controls {
  display: flex;
  gap: 4px;
}

.popout-content {
  flex: 1;
  overflow: hidden;
  position: relative;
}

.resize-handle {
  position: absolute;
  bottom: 0;
  right: 0;
  width: 24px;
  height: 24px;
  cursor: nwse-resize;
  display: flex;
  align-items: flex-end;
  justify-content: flex-end;
  color: rgba(255, 255, 255, 0.5);
  padding: 2px;
}

.resize-handle:hover {
  color: rgba(255, 255, 255, 0.8);
}
</style>
