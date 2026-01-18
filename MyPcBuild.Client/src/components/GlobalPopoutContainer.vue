<template>
  <Popout3DViewer
    v-if="isPopoutOpen && popoutContent"
    :title="popoutContent.title || '3D Preview'"
    @close="closePopout"
  >
    <component
      :is="popoutContent.component"
      v-bind="popoutContent.props || {}"
    />
  </Popout3DViewer>
</template>

<script setup lang="ts">
import { use3DPopout } from '@/composables/use3DPopout';
import Popout3DViewer from './Popout3DViewer.vue';

const { isPopoutOpen, popoutContent, closePopout } = use3DPopout();
</script>
