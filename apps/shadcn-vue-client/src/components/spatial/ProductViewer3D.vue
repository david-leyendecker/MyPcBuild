<template>
  <div class="viewer-root">
    <div class="viewer-container" ref="viewerEl">
      <ViewerControls
        :show-grid="showGrid"
        :show-axes="showAxes"
        show-axes-toggle
        @toggle-grid="() => { showGrid = !showGrid }"
        @toggle-axes="() => { showAxes = !showAxes }"
        @reset-camera="resetCamera"
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
import * as THREE from 'three';
import type { Slot, Chamber, Dimensions, Vector3 } from '@/types/product';
import { use3DViewer } from '@/composables/use3DViewer';
import {
  clearMeshMap,
  clearMeshArray,
  createBoxMeshWithWireframeColor,
  getProductCategoryColor,
  degreesToRadians
} from '@/utils/viewer3d';
import ViewerControls from './ViewerControls.vue';

interface Props {
  dimensions?: Dimensions | null;
  slots?: Slot[];
  chambers?: Chamber[];
  title?: string;
}

const props = withDefaults(defineProps<Props>(), {
  slots: () => [],
  chambers: () => []
});

const viewerEl = ref<HTMLDivElement | null>(null);
const hoveredSlot = ref<Slot | null>(null);
const showGrid = ref(true);
const showAxes = ref(true);

let slotMeshes: Map<string, THREE.Mesh> = new Map();
let productMeshes: THREE.Mesh[] = [];
let hasInitialFit = false;
let viewer3DInstance: ReturnType<typeof use3DViewer> | null = null;
let state: any = null;

onMounted(() => {
  if (!viewerEl.value) return;

  viewer3DInstance = use3DViewer({
    containerElement: viewerEl.value,
    showGrid: showGrid.value,
    showAxes: showAxes.value,
    sceneBgColor: 0x1a1a1a
  });

  state = viewer3DInstance.mount();

  watch(showGrid, (value) => {
    if (state) {
      state.gridHelper.visible = value;
    }
  });

  watch(showAxes, (value) => {
    if (state) {
      state.axesHelper.visible = value;
    }
  });

  updateVisualization();
  viewerEl.value.addEventListener('mousemove', onMouseMove);
});

onUnmounted(() => {
  if (viewerEl.value) {
    viewerEl.value.removeEventListener('mousemove', onMouseMove);
  }
  if (viewer3DInstance) {
    viewer3DInstance.unmount();
  }
});

watch(() => [props.dimensions, props.slots, props.chambers], () => {
  updateVisualization();
}, { deep: true });

function updateVisualization() {
  if (!state) return;

  clearMeshMap(slotMeshes, state.scene);
  clearMeshArray(productMeshes, state.scene);

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

  if (props.chambers && props.chambers.length > 0) {
    props.chambers.forEach((chamber: Chamber) => {
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

      if (chamber.slots) {
        chamber.slots.forEach((slot: Slot) => renderSlot(slot, chamber.relativePosition));
      }
    });
  }

  if (props.slots && props.slots.length > 0) {
    props.slots.forEach((slot: Slot) => renderSlot(slot, { x: 0, y: 0, z: 0 }));
  }

  if (!hasInitialFit && viewer3DInstance) {
    viewer3DInstance.fitCameraToContent();
    hasInitialFit = true;
  }
}

function renderSlot(slot: Slot, offset: Vector3 = { x: 0, y: 0, z: 0 }) {
  if (!state) return;

  const slotMesh = createBoxMeshWithWireframeColor(
    slot.maxDimensions.length,
    slot.maxDimensions.height,
    slot.maxDimensions.width,
    getProductCategoryColor(slot.allowedCategory),
    0xffffff,
    0.4
  );

  slotMesh.position.set(
    offset.x + slot.relativePosition.x,
    offset.y + slot.relativePosition.y,
    offset.z + slot.relativePosition.z
  );

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

  if (slot.subSlots) {
    const subSlotOffset = {
      x: offset.x + slot.relativePosition.x,
      y: offset.y + slot.relativePosition.y,
      z: offset.z + slot.relativePosition.z
    };
    slot.subSlots.forEach((subSlot: Slot) => renderSlot(subSlot, subSlotOffset));
  }
}

function onMouseMove(event: MouseEvent) {
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

function resetCamera() {
  if (viewer3DInstance) {
    viewer3DInstance.resetCamera();
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

.viewer-container {
  position: relative;
  width: 100%;
  flex: 1;
  min-height: 0;
  border-radius: 4px;
  overflow: hidden;
  background: #1a1a1a;
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
