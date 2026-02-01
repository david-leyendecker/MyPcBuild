<template>
  <CFloatingModal
    v-if="isPopoutOpen && popoutContent"
    v-model:show="isPopoutOpen"
    class="popout-shell"
    title="3D Preview"
    :width="600"
    :height="640"
    :position="{ bottom: 20, right: 20 }"
    @update:show="closePopout"
    @resize="handleResize">
    <div class="popout-content">
      <component
        :is="popoutContent.component"
        v-bind="popoutContent.props || {}"
      />
    </div>
  </CFloatingModal>
</template>

<script setup lang="ts">
import { use3DPopout } from '@/composables/use3DPopout';
import CFloatingModal from './CFloatingModal.vue';

const { isPopoutOpen, popoutContent, closePopout } = use3DPopout();

function handleResize() {
  // Trigger a window resize event so 3D viewers can adjust
  window.dispatchEvent(new Event('resize'));
}
</script>

<style scoped>
.popout-content {
  display: flex;
  flex-direction: column;
  height: 100%;
  min-height: 0;
}

:deep(.n-modal .n-card) {
  display: flex;
  flex-direction: column;
  height: 100%;
}

:deep(.n-modal .n-card__content) {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 0;
}
</style>
