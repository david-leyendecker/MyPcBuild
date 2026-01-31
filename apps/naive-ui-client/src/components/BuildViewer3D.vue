<template>
  <div v-if="!inPopout && isPopoutOpen" class="viewer-empty">
    <n-empty
      description="3D viewer is open in popout window"
      style="padding: 80px 0;"
    />
  </div>
  <div v-show="!(  !inPopout && isPopoutOpen )" class="viewer-root">
    <div v-if="showHeader && !inPopout" class="viewer-header">
      <n-flex justify="space-between" style="margin-bottom: 12px;">
        <n-h3>{{ props.title || '3D Build Visualization' }}</n-h3>
        <n-button text @click="openInPopout">
          <template #icon>
            <n-icon :component="Icons.Open" />
          </template>
          Open in Popout
        </n-button>
      </n-flex>
    </div>
    <div ref="containerRef" class="viewer-3d-container">
      <canvas ref="canvasRef"></canvas>

      <Viewer3DControls
        :show-grid="viewer3D.showGrid.value"
        @toggle-grid="() => { viewer3D.showGrid.value = !viewer3D.showGrid.value }"
        @reset-camera="() => {
          const instance = (viewer3D as any)._instance;
          if (instance) {
            instance.resetCamera();
          }
        }"
      />

      <!-- Part Info Overlay -->
      <div v-if="hoveredPart" class="part-info-overlay" :style="{ left: mousePos.x + 'px', top: mousePos.y + 'px' }">
        <n-card size="small" :bordered="true">
          <template #header>
            <span style="font-size: 12px;">{{ hoveredPart.name }}</span>
          </template>
          <div style="font-size: 12px;">
            <div><strong>Category:</strong> {{ hoveredPart.categoryName }}</div>
            <div><strong>Manufacturer:</strong> {{ hoveredPart.manufacturer }}</div>
            <div v-if="hoveredPart.dimensions">
              <strong>Dimensions:</strong>
              {{ hoveredPart.dimensions.length }} × {{ hoveredPart.dimensions.width }} × {{ hoveredPart.dimensions.height }} mm
            </div>
          </div>
        </n-card>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from 'vue';
import { NButton, NFlex, NCard, NIcon, NEmpty, NH3 } from 'naive-ui';
import * as THREE from 'three';
import type { BuildPart } from '@/api/builds';
import { Icons } from '@/utils/icons';
import { use3DPopout } from '@/composables/use3DPopout';
import { use3DViewer } from '@/composables/use3DViewer';
import {
  clearMeshMap,
  createBoxMeshWithWireframeColor,
  getBuildCategoryColor,
  degreesToRadians
} from '@/utils/viewer3d';
import Viewer3DControls from './Viewer3DControls.vue';

interface Props {
  parts: BuildPart[];
  collisions?: string[];
  inPopout?: boolean;
  showHeader?: boolean;
  title?: string;
}

const props = withDefaults(defineProps<Props>(), {
  collisions: () => [],
  inPopout: false,
  showHeader: true
});

const { isPopoutOpen, openPopout, updatePopoutProps } = use3DPopout();

const containerRef = ref<HTMLDivElement | null>(null);
const canvasRef = ref<HTMLCanvasElement | null>(null);
const hoveredPart = ref<BuildPart | null>(null);
const mousePos = ref({ x: 0, y: 0 });

const partMeshes = new Map<string, THREE.Mesh>();
let hasInitialFit = false;

const viewer3D = use3DViewer({
  containerElement: null as any,
  canvasElement: undefined,
  showGrid: true,
  sceneBgColor: 0x1a1a1a
});

onMounted(() => {
  if (!containerRef.value || !canvasRef.value) return;

  // Initialize viewer with the container element and canvas
  const viewer3DInstance = use3DViewer({
    containerElement: containerRef.value,
    canvasElement: canvasRef.value,
    showGrid: true,
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

  updateScene();
  window.addEventListener('mousemove', onMouseMove);
});

onUnmounted(() => {
  window.removeEventListener('mousemove', onMouseMove);
  const instance = (viewer3D as any)._instance;
  if (instance) {
    instance.unmount();
  }
});

watch(() => props.parts, () => {
  updateScene();
  if (!props.inPopout && isPopoutOpen.value) {
    updatePopoutProps({
      parts: props.parts,
      collisions: props.collisions,
      title: props.title
    });
  }
}, { deep: true });

watch(() => props.collisions, () => {
  highlightCollisions();
  if (!props.inPopout && isPopoutOpen.value) {
    updatePopoutProps({
      collisions: props.collisions,
      title: props.title
    });
  }
}, { deep: true });

function updateScene() {
  const state = (viewer3D as any)._state;
  if (!state) return;

  // Clear existing meshes
  clearMeshMap(partMeshes, state.scene);

  // Add parts
  props.parts.forEach(part => {
    if (!part.dimensions) return;

    // Use position if available, otherwise place at origin (for cases)
    const position = part.position || { x: 0, y: 0, z: 0 };
    const rotation = part.rotation || { x: 0, y: 0, z: 0 };

    const mesh = createBoxMeshWithWireframeColor(
      part.dimensions.length,
      part.dimensions.height,
      part.dimensions.width,
      getBuildCategoryColor(part.categoryName),
      0xffffff,
      part.categoryName === 'Case' ? 0.3 : 0.8
    );

    // Set position at the origin point
    mesh.position.set(
      position.x,
      position.y,
      position.z
    );

    // Apply rotation (convert degrees to radians)
    mesh.rotation.set(
      degreesToRadians(rotation.x),
      degreesToRadians(rotation.y),
      degreesToRadians(rotation.z)
    );

    mesh.userData = { part, id: part.id };
    state.scene.add(mesh);
    partMeshes.set(part.id, mesh);

    // Render chambers if this is a case
    if (part.chambers && part.chambers.length > 0) {
      part.chambers.forEach((chamber) => {
        const chamberGeom = new THREE.BoxGeometry(
          chamber.dimensions.length,
          chamber.dimensions.height,
          chamber.dimensions.width
        );
        // Offset geometry so corner aligns with position
        chamberGeom.translate(
          chamber.dimensions.length / 2,
          chamber.dimensions.height / 2,
          chamber.dimensions.width / 2
        );
        const chamberMat = new THREE.MeshStandardMaterial({
          color: 0x888888,
          transparent: true,
          opacity: 0.1,
          wireframe: false
        });
        const chamberMesh = new THREE.Mesh(chamberGeom, chamberMat);

        chamberMesh.position.set(
          position.x,
          position.y,
          position.z
        );

        // Add chamber wireframe
        const chamberEdges = new THREE.EdgesGeometry(chamberGeom);
        const chamberWireframe = new THREE.LineSegments(
          chamberEdges,
          new THREE.LineBasicMaterial({ color: 0xaaaaaa, linewidth: 1 })
        );
        chamberMesh.add(chamberWireframe);

        state.scene.add(chamberMesh);
      });
    }
  });

  highlightCollisions();

  // Fit camera to content on initial load
  if (!hasInitialFit) {
    const instance = (viewer3D as any)._instance;
    if (instance) {
      instance.fitCameraToContent();
    }
    hasInitialFit = true;
  }
}

function highlightCollisions() {
  const instance = (viewer3D as any)._instance;
  if (!instance) return;

  instance.highlightCollisions(
    partMeshes,
    props.collisions,
    (mesh: THREE.Mesh) => getBuildCategoryColor((mesh.userData.part as BuildPart).categoryName)
  );
}

function onMouseMove(event: MouseEvent) {
  const state = (viewer3D as any)._state;
  if (!containerRef.value || !state) return;

  const rect = containerRef.value.getBoundingClientRect();
  state.mouse.x = ((event.clientX - rect.left) / rect.width) * 2 - 1;
  state.mouse.y = -((event.clientY - rect.top) / rect.height) * 2 + 1;

  mousePos.value = { x: event.clientX + 10, y: event.clientY + 10 };

  state.raycaster.setFromCamera(state.mouse, state.camera);
  const intersects = state.raycaster.intersectObjects(Array.from(partMeshes.values()));

  if (intersects.length > 0 && intersects[0]) {
    const intersectedMesh = intersects[0].object as THREE.Mesh;
    hoveredPart.value = intersectedMesh.userData.part;
  } else {
    hoveredPart.value = null;
  }
}

async function openInPopout() {
  openPopout({
    component: (await import('./BuildViewer3D.vue')).default,
    props: {
      parts: props.parts,
      collisions: props.collisions,
      inPopout: true,
      showHeader: false,
      title: props.title,
    },
    title: props.title || '3D Build Visualization',
  });
}
</script>

<style scoped>
.viewer-root {
  display: flex;
  flex-direction: column;
  height: 100%;
  min-height: 400px;
}

.viewer-3d-container {
  position: relative;
  width: 100%;
  flex: 1;
  height: 100%;
  min-height: 0;
  border-radius: 8px;
  overflow: hidden;
}

.viewer-3d-container canvas {
  position: absolute;
  top: 0;
  left: 0;
  display: block;
  width: 100%;
  height: 100%;
}

.viewer-header .text-h3 {
  font-size: 1.5rem;
  font-weight: 500;
}

.part-info-overlay {
  position: fixed;
  z-index: 100;
  pointer-events: none;
  max-width: 300px;
}
</style>
