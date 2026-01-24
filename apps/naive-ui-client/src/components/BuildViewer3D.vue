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
        :show-grid="showGrid"
        @toggle-grid="toggleGrid"
        @reset-camera="resetCamera"
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
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js';
import type { BuildPart } from '@/api/builds';
import { Icons } from '@/utils/icons';
import { use3DPopout } from '@/composables/use3DPopout';
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
const showGrid = ref(true);
const hoveredPart = ref<BuildPart | null>(null);
const mousePos = ref({ x: 0, y: 0 });

let scene: THREE.Scene;
let camera: THREE.PerspectiveCamera;
let renderer: THREE.WebGLRenderer;
let controls: OrbitControls;
let gridHelper: THREE.GridHelper;
let animationId: number;
let raycaster: THREE.Raycaster;
let mouse: THREE.Vector2;

const partMeshes = new Map<string, THREE.Mesh>();
let resizeObserver: ResizeObserver | null = null;
let resizeTimeout: number | null = null;

onMounted(() => {
  initScene();
  animate();
  window.addEventListener('resize', onWindowResize);
  window.addEventListener('mousemove', onMouseMove);

  // Watch for container resize with debouncing
  if (containerRef.value) {
    resizeObserver = new ResizeObserver(() => {
      if (resizeTimeout !== null) {
        clearTimeout(resizeTimeout);
      }
      resizeTimeout = window.setTimeout(() => {
        onWindowResize();
      }, 100);
    });
    resizeObserver.observe(containerRef.value);
  }
});

onUnmounted(() => {
  window.removeEventListener('resize', onWindowResize);
  window.removeEventListener('mousemove', onMouseMove);

  if (resizeTimeout !== null) {
    clearTimeout(resizeTimeout);
  }

  if (resizeObserver) {
    resizeObserver.disconnect();
  }

  cancelAnimationFrame(animationId);
  renderer?.dispose();
  controls?.dispose();
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

function initScene() {
  if (!containerRef.value || !canvasRef.value) return;

  // Scene
  scene = new THREE.Scene();
  scene.background = new THREE.Color(0x1a1a1a);

  // Camera
  const width = containerRef.value.clientWidth;
  const height = containerRef.value.clientHeight;
  camera = new THREE.PerspectiveCamera(75, width / height, 0.1, 10000);
  camera.position.set(500, 500, 500);
  camera.lookAt(0, 0, 0);

  // Renderer
  renderer = new THREE.WebGLRenderer({
    canvas: canvasRef.value,
    antialias: true
  });
  renderer.setSize(width, height);
  renderer.setPixelRatio(window.devicePixelRatio);

  // Controls
  controls = new OrbitControls(camera, renderer.domElement);
  controls.enableDamping = true;
  controls.dampingFactor = 0.05;
  controls.minDistance = 100;
  controls.maxDistance = 2000;

  // Lighting
  const ambientLight = new THREE.AmbientLight(0xffffff, 0.6);
  scene.add(ambientLight);

  const directionalLight = new THREE.DirectionalLight(0xffffff, 0.8);
  directionalLight.position.set(500, 1000, 500);
  scene.add(directionalLight);

  const directionalLight2 = new THREE.DirectionalLight(0xffffff, 0.4);
  directionalLight2.position.set(-500, -500, -500);
  scene.add(directionalLight2);

  // Grid
  gridHelper = new THREE.GridHelper(1000, 20, 0x444444, 0x222222);
  scene.add(gridHelper);

  // Raycaster for hover detection
  raycaster = new THREE.Raycaster();
  mouse = new THREE.Vector2();

  // Axes helper
  const axesHelper = new THREE.AxesHelper(200);
  scene.add(axesHelper);

  updateScene();
}

function updateScene() {
  // Clear existing meshes
  partMeshes.forEach(mesh => {
    scene.remove(mesh);
    mesh.geometry.dispose();
    if (Array.isArray(mesh.material)) {
      mesh.material.forEach(m => m.dispose());
    } else {
      mesh.material.dispose();
    }
  });
  partMeshes.clear();

  // Add parts
  props.parts.forEach(part => {
    if (!part.dimensions) return;

    // Use position if available, otherwise place at origin (for cases)
    const position = part.position || { x: 0, y: 0, z: 0 };
    const rotation = part.rotation || { x: 0, y: 0, z: 0 };

    const geometry = new THREE.BoxGeometry(
      part.dimensions.length,
      part.dimensions.height,
      part.dimensions.width
    );

    const material = new THREE.MeshStandardMaterial({
      color: getCategoryColor(part.categoryName),
      transparent: true,
      opacity: part.categoryName === 'Case' ? 0.3 : 0.8
    });

    const mesh = new THREE.Mesh(geometry, material);

    // Set position (center of the box)
    mesh.position.set(
      position.x + part.dimensions.length / 2,
      position.y + part.dimensions.height / 2,
      position.z + part.dimensions.width / 2
    );

    // Apply rotation (convert degrees to radians)
    mesh.rotation.set(
      (rotation.x * Math.PI) / 180,
      (rotation.y * Math.PI) / 180,
      (rotation.z * Math.PI) / 180
    );

    // Add wireframe
    const edges = new THREE.EdgesGeometry(geometry);
    const lineMaterial = new THREE.LineBasicMaterial({ color: 0xffffff, linewidth: 1 });
    const wireframe = new THREE.LineSegments(edges, lineMaterial);
    mesh.add(wireframe);

    mesh.userData = { part };
    scene.add(mesh);
    partMeshes.set(part.id, mesh);

    // Render chambers if this is a case
    if (part.chambers && part.chambers.length > 0) {
      part.chambers.forEach((chamber) => {
        const chamberGeom = new THREE.BoxGeometry(
          chamber.dimensions.length,
          chamber.dimensions.height,
          chamber.dimensions.width
        );
        const chamberMat = new THREE.MeshStandardMaterial({
          color: 0x888888,
          transparent: true,
          opacity: 0.1,
          wireframe: false
        });
        const chamberMesh = new THREE.Mesh(chamberGeom, chamberMat);

        chamberMesh.position.set(
          position.x + chamber.dimensions.length / 2,
          position.y + chamber.dimensions.height / 2,
          position.z + chamber.dimensions.width / 2
        );

        // Add chamber wireframe
        const chamberEdges = new THREE.EdgesGeometry(chamberGeom);
        const chamberWireframe = new THREE.LineSegments(
          chamberEdges,
          new THREE.LineBasicMaterial({ color: 0xaaaaaa, linewidth: 1 })
        );
        chamberMesh.add(chamberWireframe);

        scene.add(chamberMesh);
      });
    }
  });

  highlightCollisions();
}

function highlightCollisions() {
  partMeshes.forEach((mesh, partId) => {
    const isColliding = props.collisions.includes(partId);
    if (Array.isArray(mesh.material)) {
      mesh.material.forEach(m => {
        if (m instanceof THREE.MeshStandardMaterial) {
          m.color.set(isColliding ? 0xff0000 : getCategoryColor(mesh.userData.part.categoryName));
        }
      });
    } else if (mesh.material instanceof THREE.MeshStandardMaterial) {
      mesh.material.color.set(isColliding ? 0xff0000 : getCategoryColor(mesh.userData.part.categoryName));
    }
  });
}

function getCategoryColor(category: string): number {
  const colors: Record<string, number> = {
    'CPU': 0x3b82f6,      // blue
    'GPU': 0x10b981,      // green
    'Motherboard': 0x8b5cf6, // purple
    'RAM': 0xf59e0b,      // amber
    'Storage': 0xef4444,  // red
    'PowerSupply': 0x06b6d4, // cyan
    'Cooler': 0xec4899,   // pink
    'Case': 0x6b7280      // gray
  };
  return colors[category] || 0x9ca3af;
}

function onMouseMove(event: MouseEvent) {
  if (!containerRef.value || !canvasRef.value) return;

  const rect = containerRef.value.getBoundingClientRect();
  mouse.x = ((event.clientX - rect.left) / rect.width) * 2 - 1;
  mouse.y = -((event.clientY - rect.top) / rect.height) * 2 + 1;

  mousePos.value = { x: event.clientX + 10, y: event.clientY + 10 };

  raycaster.setFromCamera(mouse, camera);
  const intersects = raycaster.intersectObjects(Array.from(partMeshes.values()));

  if (intersects.length > 0 && intersects[0]) {
    const intersectedMesh = intersects[0].object as THREE.Mesh;
    hoveredPart.value = intersectedMesh.userData.part;
  } else {
    hoveredPart.value = null;
  }
}

function animate() {
  animationId = requestAnimationFrame(animate);
  controls.update();
  renderer.render(scene, camera);
}

function onWindowResize() {
  if (!containerRef.value) return;

  const width = containerRef.value.clientWidth;
  const height = containerRef.value.clientHeight;

  camera.aspect = width / height;
  camera.updateProjectionMatrix();
  renderer.setSize(width, height);
}

function resetCamera() {
  camera.position.set(500, 500, 500);
  camera.lookAt(0, 0, 0);
}

function toggleGrid() {
  showGrid.value = !showGrid.value;
  gridHelper.visible = showGrid.value;
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
