<template>
  <n-modal
    v-model:show="isOpen"
    :mask-closable="false"
    :show-mask="false"
    :style="modalStyle"
    :content-style="contentStyle"
    transform-origin="center"
  >
    <n-card
      :title="title"
      closable
      @close="close"
      :bordered="true"
      class="popout-viewer"
      :class="{ 'maximized': isMaximized }"
    >
      <template #header-extra>
        <n-flex :size="4" @mousedown.stop>
          <n-button
            text
            @click.stop="toggleMinimize"
            :title="isMinimized ? 'Restore' : 'Minimize'"
          >
            <template #icon>
              <n-icon :component="isMinimized ? Icons.Expand : Icons.Minus" />
            </template>
          </n-button>
          <n-button
            text
            @click.stop="toggleMaximize"
            :title="isMaximized ? 'Restore' : 'Maximize'"
          >
            <template #icon>
              <n-icon :component="isMaximized ? Icons.Contract : Icons.Expand" />
            </template>
          </n-button>
        </n-flex>
      </template>
      
      <div v-show="!isMinimized" class="popout-content" :style="contentInnerStyle">
        <slot></slot>
      </div>

      <!-- Resize Handle (bottom-right corner) -->
      <div 
        v-show="!isMinimized && !isMaximized"
        class="resize-handle"
        @mousedown.stop="startResize"
      >
        <n-icon :component="Icons.Resize" style="font-size: 16px;" />
      </div>
    </n-card>
  </n-modal>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue';
import { NModal, NCard, NButton, NFlex, NIcon } from 'naive-ui';
import { Icons } from '@/utils/icons';

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
const isResizing = ref(false);

const width = ref(props.initialWidth);
const height = ref(props.initialHeight);
const x = ref(props.initialX ?? window.innerWidth - props.initialWidth - 20);
const y = ref(props.initialY ?? 20);

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

const modalStyle = computed(() => ({
  position: 'fixed' as const,
  top: `${y.value}px`,
  left: `${x.value}px`,
  zIndex: 9999,
  padding: 0,
  margin: 0
}));

const contentStyle = computed(() => {
  if (isMaximized.value) {
    return {
      width: '100vw',
      height: '100vh',
      top: '0',
      left: '0',
      padding: 0,
      margin: 0
    };
  }
  
  if (isMinimized.value) {
    return {
      width: '250px',
      height: 'auto'
    };
  }

  return {
    width: `${width.value}px`,
    height: `${height.value}px`,
    padding: 0,
    margin: 0
  };
});

const contentInnerStyle = computed(() => ({
  height: '100%',
  overflow: 'hidden' as const
}));

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
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.5);
  transition: box-shadow 0.2s;
}

.popout-viewer:hover {
  box-shadow: 0 12px 48px rgba(0, 0, 0, 0.7);
}

.popout-viewer.maximized {
  border-radius: 0 !important;
  box-shadow: none;
}

.popout-viewer :deep(.n-card-header) {
  cursor: move;
  user-select: none;
}

.popout-content {
  overflow: hidden;
  position: relative;
  width: 100%;
  height: 100%;
}

.resize-handle {
  position: absolute;
  bottom: 4px;
  right: 4px;
  width: 24px;
  height: 24px;
  cursor: nwse-resize;
  display: flex;
  align-items: center;
  justify-content: center;
  color: rgba(255, 255, 255, 0.5);
}

.resize-handle:hover {
  color: rgba(255, 255, 255, 0.8);
}
</style>
