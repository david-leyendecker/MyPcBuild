<template>
  <div ref="containerRef" class="viewer-3d-container">
    <canvas ref="canvasRef"></canvas>
    
    <!-- Controls Overlay -->
    <div class="controls-overlay">
      <v-btn-group density="compact" variant="outlined">
        <v-btn @click="resetCamera" size="small" prepend-icon="mdi-crop-free">
          Reset View
        </v-btn>
        <v-btn @click="toggleGrid" size="small" :prepend-icon="showGrid ? 'mdi-grid-off' : 'mdi-grid'">
          {{ showGrid ? 'Hide Grid' : 'Show Grid' }}
        </v-btn>
      </v-btn-group>
    </div>

    <!-- Part Info Overlay -->
    <div v-if="hoveredPart" class="part-info-overlay" :style="{ left: mousePos.x + 'px', top: mousePos.y + 'px' }">
      <v-card>
        <v-card-title class="text-caption">{{ hoveredPart.name }}</v-card-title>
        <v-card-text class="text-caption">
          <div><strong>Category:</strong> {{ hoveredPart.categoryName }}</div>
          <div><strong>Manufacturer:</strong> {{ hoveredPart.manufacturer }}</div>
          <div v-if="hoveredPart.dimensions">
            <strong>Dimensions:</strong> 
            {{ hoveredPart.dimensions.length }} × {{ hoveredPart.dimensions.width }} × {{ hoveredPart.dimensions.height }} mm
          </div>
        </v-card-text>
      </v-card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from 'vue';
import * as THREE from 'three';
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js';
import type { BuildPart } from '@/api/builds';

interface Props {
  parts: BuildPart[];
  collisions?: string[];
}

const props = withDefaults(defineProps<Props>(), {
  collisions: () => []
});

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

onMounted(() => {
  initScene();
  animate();
  window.addEventListener('resize', onWindowResize);
  window.addEventListener('mousemove', onMouseMove);
});

onUnmounted(() => {
  window.removeEventListener('resize', onWindowResize);
  window.removeEventListener('mousemove', onMouseMove);
  cancelAnimationFrame(animationId);
  renderer?.dispose();
  controls?.dispose();
});

watch(() => props.parts, () => {
  updateScene();
}, { deep: true });

watch(() => props.collisions, () => {
  highlightCollisions();
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
    if (!part.dimensions || !part.position) return;

    const geometry = new THREE.BoxGeometry(
      part.dimensions.length,
      part.dimensions.height,
      part.dimensions.width
    );

    const material = new THREE.MeshStandardMaterial({
      color: getCategoryColor(part.categoryName),
      transparent: true,
      opacity: 0.8
    });

    const mesh = new THREE.Mesh(geometry, material);
    mesh.position.set(
      part.position.x + part.dimensions.length / 2,
      part.position.y + part.dimensions.height / 2,
      part.position.z + part.dimensions.width / 2
    );
    
    // Add wireframe
    const edges = new THREE.EdgesGeometry(geometry);
    const lineMaterial = new THREE.LineBasicMaterial({ color: 0xffffff, linewidth: 1 });
    const wireframe = new THREE.LineSegments(edges, lineMaterial);
    mesh.add(wireframe);

    mesh.userData = { part };
    scene.add(mesh);
    partMeshes.set(part.id, mesh);
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
  controls.reset();
}

function toggleGrid() {
  showGrid.value = !showGrid.value;
  gridHelper.visible = showGrid.value;
}
</script>

<style scoped>
.viewer-3d-container {
  position: relative;
  width: 100%;
  height: 600px;
  border-radius: 8px;
  overflow: hidden;
}

.viewer-3d-container canvas {
  display: block;
  width: 100%;
  height: 100%;
}

.controls-overlay {
  position: absolute;
  top: 16px;
  right: 16px;
  z-index: 10;
}

.part-info-overlay {
  position: fixed;
  z-index: 100;
  pointer-events: none;
  max-width: 300px;
}
</style>
