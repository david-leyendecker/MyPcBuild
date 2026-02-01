<template>
  <div v-if="!inPopout && isPopoutOpen" class="viewer-empty">
    <n-empty
      description="3D viewer is open in popout window"
      style="padding: 80px 0;"
    />
  </div>
  <div v-show="!( !inPopout && isPopoutOpen )" class="viewer-root">
    <div v-if="showHeader && !inPopout" class="viewer-header">
      <n-flex justify="space-between" style="margin-bottom: 12px;">
        <n-h3>{{ props.title || '3D Preview' }}</n-h3>
        <n-button text @click="openInPopout">
          <template #icon>
            <n-icon :component="Icons.Open" />
          </template>
          Open in Popout
        </n-button>
      </n-flex>
      <p>
        Interactive visualization of slots and chambers
      </p>
    </div>
    <div class="viewer-container">
    <div ref="viewerEl" class="viewer-canvas"></div>

    <Viewer3DControls
      :show-grid="viewer3D.showGrid.value"
      :show-axes="viewer3D.showAxes.value"
      show-axes-toggle
      @toggle-grid="() => { viewer3D.showGrid.value = !viewer3D.showGrid.value }"
      @toggle-axes="() => { viewer3D.showAxes.value = !viewer3D.showAxes.value }"
      @reset-camera="() => viewer3D.resetCamera()"
    />

    <div v-if="hoveredSlot" class="slot-tooltip">
      <strong>{{ hoveredSlot.name }}</strong><br>
      Category: {{ hoveredSlot.allowedCategory }}<br>
      Position: ({{ hoveredSlot.relativePosition.x }}, {{ hoveredSlot.relativePosition.y }}, {{ hoveredSlot.relativePosition.z }})<br>
      Max Size: {{ hoveredSlot.maxDimensions.length }} × {{ hoveredSlot.maxDimensions.width }} × {{ hoveredSlot.maxDimensions.height }} mm
      <span v-if="hoveredSlot.rotation && (hoveredSlot.rotation.x || hoveredSlot.rotation.y || hoveredSlot.rotation.z)">
        <br>Rotation: ({{ hoveredSlot.rotation.x }}°, {{ hoveredSlot.rotation.y }}°, {{ hoveredSlot.rotation.z }}°)
      </span>
    </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from 'vue';
import { NEmpty, NFlex, NIcon, NH3, NButton } from 'naive-ui';
import * as THREE from 'three';
import type { Slot, Chamber, Dimensions, Vector3 } from '@/types/products';
import { use3DPopout } from '@/composables/use3DPopout';
import { use3DViewer } from '@/composables/use3DViewer';
import { Icons } from '@/utils/icons';
import {
  clearMeshMap,
  clearMeshArray,
  createBoxMeshWithWireframeColor,
  getProductCategoryColor,
  degreesToRadians
} from '@/utils/viewer3d';
import Viewer3DControls from './Viewer3DControls.vue';

interface Props {
  dimensions?: Dimensions | null;
  slots?: Slot[];
  chambers?: Chamber[];
  inPopout?: boolean;
  showHeader?: boolean;
  title?: string;
}

const props = withDefaults(defineProps<Props>(), {
  slots: () => [],
  chambers: () => [],
  inPopout: false,
  showHeader: true
});

const { isPopoutOpen, openPopout, updatePopoutProps } = use3DPopout();

async function openInPopout() {
  openPopout({
    component: (await import('./ProductViewer3D.vue')).default,
    props: {
      dimensions: props.dimensions,
      slots: props.slots,
      chambers: props.chambers,
      inPopout: true,
      showHeader: false,
      title: props.title,
    },
    title: props.title || '3D Preview',
  });
}

const viewerEl = ref<HTMLDivElement | null>(null);
const hoveredSlot = ref<Slot | null>(null);

let slotMeshes: Map<string, THREE.Mesh> = new Map();
let productMeshes: THREE.Mesh[] = [];
let hasInitialFit = false;

const viewer3D = use3DViewer({
  containerElement: viewerEl.value as HTMLElement,
  showGrid: true,
  showAxes: true,
  sceneBgColor: 0x1a1a1a,
  onAnimate: () => {
    // Animation is handled by the composable
  }
});

onMounted(() => {
  if (!viewerEl.value) return;

  // Initialize viewer with the container element
  const container = viewerEl.value;
  const viewer3DInstance = use3DViewer({
    containerElement: container,
    showGrid: true,
    showAxes: true,
    sceneBgColor: 0x1a1a1a
  });

  const state = viewer3DInstance.mount();
  Object.defineProperty(viewer3D, '_state', {
    value: state,
    configurable: true
  });
  Object.defineProperty(viewer3D, '_instance', {
    value: viewer3DInstance,
    configurable: true
  });

  updateVisualization();
  viewerEl.value.addEventListener('mousemove', onMouseMove);
});

onUnmounted(() => {
  if (viewerEl.value) {
    viewerEl.value.removeEventListener('mousemove', onMouseMove);
  }
  const instance = (viewer3D as any)._instance;
  if (instance) {
    instance.unmount();
  }
});

watch(() => [props.dimensions, props.slots, props.chambers], () => {
  updateVisualization();
  // Keep popout viewer props in sync when open
  if (!props.inPopout && isPopoutOpen.value) {
    updatePopoutProps({
      dimensions: props.dimensions,
      slots: props.slots,
      chambers: props.chambers,
      title: props.title
    });
  }
}, { deep: true });

function updateVisualization() {
  const state = (viewer3D as any)._state;
  if (!state) return;

  // Clear existing slot meshes
  clearMeshMap(slotMeshes, state.scene);

  // Clear existing product meshes (dimensions box and chambers)
  clearMeshArray(productMeshes, state.scene);

  // Render main product dimensions if available
  if (props.dimensions) {
    const mesh = createBoxMeshWithWireframeColor(
      props.dimensions.length,
      props.dimensions.height,
      props.dimensions.width,
      0x666666,
      0xaaaaaa,
      0.2
    );
    mesh.position.set(0, 0, 0);

    state.scene.add(mesh);
    productMeshes.push(mesh);
  }

  // Render chambers
  if (props.chambers && props.chambers.length > 0) {
    props.chambers.forEach((chamber) => {
      const chamberMesh = createBoxMeshWithWireframeColor(
        chamber.dimensions.length,
        chamber.dimensions.height,
        chamber.dimensions.width,
        0x4444ff,
        0x6666ff,
        0.15
      );
      chamberMesh.position.set(
        chamber.relativePosition.x,
        chamber.relativePosition.y,
        chamber.relativePosition.z
      );

      state.scene.add(chamberMesh);
      productMeshes.push(chamberMesh);

      // Render slots in chamber (offset by chamber position)
      if (chamber.slots) {
        chamber.slots.forEach(slot => renderSlot(slot, chamber.relativePosition));
      }
    });
  }

  // Render direct slots
  if (props.slots && props.slots.length > 0) {
    props.slots.forEach(slot => renderSlot(slot, { x: 0, y: 0, z: 0 }));
  }

  // Adjust camera to fit content only on initial load
  if (!hasInitialFit) {
    const instance = (viewer3D as any)._instance;
    if (instance) {
      instance.fitCameraToContent();
    }
    hasInitialFit = true;
  }
}

function renderSlot(slot: Slot, offset: Vector3 = { x: 0, y: 0, z: 0 }) {
  const state = (viewer3D as any)._state;
  if (!state) return;

  const slotMesh = createBoxMeshWithWireframeColor(
    slot.maxDimensions.length,
    slot.maxDimensions.height,
    slot.maxDimensions.width,
    getProductCategoryColor(slot.allowedCategory),
    0xffffff,
    0.4
  );

  // Position (add chamber offset)
  slotMesh.position.set(
    offset.x + slot.relativePosition.x,
    offset.y + slot.relativePosition.y,
    offset.z + slot.relativePosition.z
  );

  // Rotation
  if (slot.rotation) {
    slotMesh.rotation.set(
      degreesToRadians(slot.rotation.x),
      degreesToRadians(slot.rotation.y),
      degreesToRadians(slot.rotation.z)
    );
  }

  slotMesh.userData = { slot };
  state.scene.add(slotMesh);
  slotMeshes.set(slot.name, slotMesh);

  // Render sub-slots (with accumulated offset)
  if (slot.subSlots) {
    const subSlotOffset = {
      x: offset.x + slot.relativePosition.x,
      y: offset.y + slot.relativePosition.y,
      z: offset.z + slot.relativePosition.z
    };
    slot.subSlots.forEach(subSlot => renderSlot(subSlot, subSlotOffset));
  }
}

function onMouseMove(event: MouseEvent) {
  const state = (viewer3D as any)._state;
  if (!viewerEl.value || !state) return;

  const rect = viewerEl.value.getBoundingClientRect();
  state.mouse.x = ((event.clientX - rect.left) / rect.width) * 2 - 1;
  state.mouse.y = -((event.clientY - rect.top) / rect.height) * 2 + 1;

  state.raycaster.setFromCamera(state.mouse, state.camera);

  const intersects = state.raycaster.intersectObjects(Array.from(slotMeshes.values()));

  if (intersects.length > 0 && intersects[0]) {
    const intersected = intersects[0].object as THREE.Mesh;
    hoveredSlot.value = intersected.userData?.slot || null;
  } else {
    hoveredSlot.value = null;
  }
}
</script>

<style scoped>
.viewer-root {
  display: flex;
  flex-direction: column;
  height: 100%;
  min-height: 400px;
}

.viewer-header .text-h5 {
  font-size: 1.5rem;
  font-weight: 500;
}

.viewer-header .text-body-2 {
  font-size: 0.875rem;
}

.viewer-container {
  position: relative;
  width: 100%;
  flex: 1;
  min-height: 0;
  border-radius: 4px;
  overflow: hidden;
  background: #1a1a1a;
}

.viewer-canvas {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
}

.viewer-controls {
  position: absolute;
  top: 10px;
  right: 10px;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.slot-tooltip {
  position: absolute;
  bottom: 10px;
  left: 10px;
  background: rgba(0, 0, 0, 0.8);
  color: white;
  padding: 12px;
  border-radius: 4px;
  font-size: 12px;
  line-height: 1.5;
  pointer-events: none;
  max-width: 300px;
}
</style>
