<template>
  <div class="viewer-root">
    <div class="viewer-3d-container" ref="containerRef">
      <canvas ref="canvasRef"></canvas>

      <ViewerControls
        :show-grid="showGrid"
        @toggle-grid="() => { showGrid = !showGrid }"
        @reset-camera="resetCamera"
      />

      <div v-if="hoveredPart" class="part-info-overlay" :style="{ left: mousePos.x + 'px', top: mousePos.y + 'px' }">
        <Card class="max-w-xs">
          <CardHeader class="p-3">
            <CardTitle class="text-sm">{{ hoveredPart.name }}</CardTitle>
          </CardHeader>
          <CardContent class="p-3 pt-0 text-xs space-y-1">
            <div><strong>Category:</strong> {{ hoveredPart.categoryName }}</div>
            <div><strong>Manufacturer:</strong> {{ hoveredPart.manufacturer }}</div>
            <div v-if="hoveredPart.dimensions">
              <strong>Dimensions:</strong>
              {{ hoveredPart.dimensions.length }} × {{ hoveredPart.dimensions.width }} × {{ hoveredPart.dimensions.height }} mm
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from 'vue';
import * as THREE from 'three';
import type { BuildPart } from '@/types/build';
import type { Chamber } from '@/types/spatial';
import { use3DViewer } from '@/composables/use3DViewer';
import {
  clearMeshMap,
  createBoxMeshWithWireframeColor,
  getBuildCategoryColor,
  degreesToRadians
} from '@/utils/viewer3d';
import ViewerControls from './ViewerControls.vue';
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';

interface Props {
  parts: BuildPart[];
  collisions?: string[];
  title?: string;
}

const props = withDefaults(defineProps<Props>(), {
  collisions: () => []
});

const containerRef = ref<HTMLDivElement | null>(null);
const canvasRef = ref<HTMLCanvasElement | null>(null);
const hoveredPart = ref<BuildPart | null>(null);
const mousePos = ref({ x: 0, y: 0 });
const showGrid = ref(true);

const partMeshes = new Map<string, THREE.Mesh>();
const chamberMeshes: THREE.Mesh[] = [];
let hasInitialFit = false;
let viewer3DInstance: ReturnType<typeof use3DViewer> | null = null;
let state: any = null;

onMounted(() => {
  if (!containerRef.value || !canvasRef.value) return;

  viewer3DInstance = use3DViewer({
    containerElement: containerRef.value,
    canvasElement: canvasRef.value,
    showGrid: showGrid.value,
    sceneBgColor: 0x1a1a1a
  });

  state = viewer3DInstance.mount();
  
  watch(showGrid, (value) => {
    if (state) {
      state.gridHelper.visible = value;
    }
  });

  updateScene();
  containerRef.value.addEventListener('mousemove', onMouseMove);
});

onUnmounted(() => {
  if (containerRef.value) {
    containerRef.value.removeEventListener('mousemove', onMouseMove);
  }
  if (viewer3DInstance) {
    viewer3DInstance.unmount();
  }
});

watch(() => props.parts, () => {
  updateScene();
}, { deep: true });

watch(() => props.collisions, () => {
  highlightCollisions();
}, { deep: true });

function updateScene() {
  if (!state) return;

  clearMeshMap(partMeshes, state.scene);

  // Clear previously created chamber meshes to prevent leaks
  for (const mesh of chamberMeshes) {
    state.scene.remove(mesh);
    mesh.geometry.dispose();
    if (Array.isArray(mesh.material)) {
      mesh.material.forEach((m: THREE.Material) => m.dispose());
    } else {
      mesh.material.dispose();
    }
    // Also dispose children (wireframe edges)
    mesh.children.forEach((child: THREE.Object3D) => {
      if (child instanceof THREE.LineSegments) {
        child.geometry.dispose();
        if (child.material instanceof THREE.Material) {
          child.material.dispose();
        }
      }
    });
  }
  chamberMeshes.length = 0;

  props.parts.forEach(part => {
    if (!part.dimensions) return;

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

    mesh.position.set(position.x, position.y, position.z);
    mesh.rotation.set(
      degreesToRadians(rotation.x),
      degreesToRadians(rotation.y),
      degreesToRadians(rotation.z)
    );

    mesh.userData = { part, id: part.id };
    state.scene.add(mesh);
    partMeshes.set(part.id, mesh);

    if (part.chambers && part.chambers.length > 0) {
      part.chambers.forEach((chamber: Chamber) => {
        const chamberGeom = new THREE.BoxGeometry(
          chamber.dimensions.length,
          chamber.dimensions.height,
          chamber.dimensions.width
        );
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

        chamberMesh.position.set(position.x, position.y, position.z);

        const chamberEdges = new THREE.EdgesGeometry(chamberGeom);
        const chamberWireframe = new THREE.LineSegments(
          chamberEdges,
          new THREE.LineBasicMaterial({ color: 0xaaaaaa, linewidth: 1 })
        );
        chamberMesh.add(chamberWireframe);

        state.scene.add(chamberMesh);
        chamberMeshes.push(chamberMesh);
      });
    }
  });

  highlightCollisions();

  if (!hasInitialFit && viewer3DInstance) {
    viewer3DInstance.fitCameraToContent();
    hasInitialFit = true;
  }
}

function highlightCollisions() {
  if (!viewer3DInstance) return;

  viewer3DInstance.highlightCollisions(
    partMeshes,
    props.collisions,
    (mesh: THREE.Mesh) => getBuildCategoryColor((mesh.userData.part as BuildPart).categoryName)
  );
}

function onMouseMove(event: MouseEvent) {
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

.part-info-overlay {
  position: fixed;
  z-index: 100;
  pointer-events: none;
}
</style>
