<template>
  <n-modal
    v-model:show="isOpen"
    :mask-closable="false"
    :show-mask="false"
    :draggable="true"
    :style="modalStyle"
    transform-origin="center"
  >
    <n-card
      :title="title"
      closable
      @close="close"
      :bordered="true"
      :style="cardStyle"
      class="popout-viewer"
    >
      <template #header-extra>
        <n-flex :size="4">
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
      
      <div v-show="!isMinimized" class="popout-content">
        <slot></slot>
      </div>
    </n-card>
  </n-modal>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { NModal, NCard, NButton, NFlex, NIcon } from 'naive-ui';
import { Icons } from '@/utils/icons';

interface Props {
  title?: string;
}

const props = withDefaults(defineProps<Props>(), {
  title: '3D Preview',
});

const emit = defineEmits<{
  close: [];
  resize: [];
}>();

const isOpen = ref(true);
const isMinimized = ref(false);
const isMaximized = ref(false);

// Calculate 25% of viewport size
const viewportWidth = ref(0);
const viewportHeight = ref(0);

onMounted(() => {
  viewportWidth.value = window.innerWidth;
  viewportHeight.value = window.innerHeight;
  
  // Listen for window resize
  const handleResize = () => {
    viewportWidth.value = window.innerWidth;
    viewportHeight.value = window.innerHeight;
  };
  window.addEventListener('resize', handleResize);
  
  return () => window.removeEventListener('resize', handleResize);
});

// Position at bottom right, 25% of viewport
const modalStyle = computed(() => {
  const width = viewportWidth.value * 0.25;
  const height = viewportHeight.value * 0.25;
  const right = 20;
  const bottom = 20;
  const left = viewportWidth.value - width - right;
  const top = viewportHeight.value - height - bottom;
  
  if (isMaximized.value) {
    return {
      position: 'fixed' as const,
      top: '0',
      left: '0',
      right: '0',
      bottom: '0',
      width: '100vw',
      height: '100vh',
      zIndex: 9999,
    };
  }
  
  if (isMinimized.value) {
    return {
      position: 'fixed' as const,
      top: `${top}px`,
      left: `${left}px`,
      width: 'auto',
      zIndex: 9999,
    };
  }
  
  return {
    position: 'fixed' as const,
    top: `${top}px`,
    left: `${left}px`,
    width: `${width}px`,
    height: `${height}px`,
    zIndex: 9999,
  };
});

const cardStyle = computed(() => {
  if (isMaximized.value) {
    return {
      width: '100%',
      height: '100%',
    };
  }
  
  if (isMinimized.value) {
    return {
      width: '250px',
      height: 'auto',
    };
  }
  
  return {
    width: '100%',
    height: '100%',
  };
});

function toggleMinimize() {
  isMinimized.value = !isMinimized.value;
  if (!isMinimized.value) {
    setTimeout(() => emit('resize'), 50);
  }
}

function toggleMaximize() {
  isMaximized.value = !isMaximized.value;
  setTimeout(() => emit('resize'), 50);
}

function close() {
  isOpen.value = false;
  emit('close');
}

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

.popout-content {
  overflow: hidden;
  width: 100%;
  height: 100%;
}
</style>
