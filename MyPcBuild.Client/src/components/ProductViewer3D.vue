<template>
  <div class="viewer-container">
    <div ref="viewerEl" class="viewer-canvas"></div>
    
    <div class="viewer-controls">
      <v-btn 
        size="small" 
        icon 
        variant="tonal"
        @click="showGrid = !showGrid"
        :color="showGrid ? 'primary' : undefined"
      >
        <v-icon>mdi-grid</v-icon>
        <v-tooltip activator="parent" location="top">Toggle Grid</v-tooltip>
      </v-btn>
      
      <v-btn 
        size="small" 
        icon 
        variant="tonal"
        @click="showAxes = !showAxes"
        :color="showAxes ? 'primary' : undefined"
      >
        <v-icon>mdi-axis-arrow</v-icon>
        <v-tooltip activator="parent" location="top">Toggle Axes</v-tooltip>
      </v-btn>
      
      <v-btn 
        size="small" 
        icon 
        variant="tonal"
        @click="resetCamera"
      >
        <v-icon>mdi-restore</v-icon>
        <v-tooltip activator="parent" location="top">Reset View</v-tooltip>
      </v-btn>
    </div>

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
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from 'vue';
import * as THREE from 'three';
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls.js';
import type { Slot, Chamber, Dimensions } from '@/types/products';

interface Props {
  dimensions?: Dimensions | null;
  slots?: Slot[];
  chambers?: Chamber[];
}

const props = withDefaults(defineProps<Props>(), {
  slots: () => [],
  chambers: () => []
});

const viewerEl = ref<HTMLDivElement | null>(null);
const hoveredSlot = ref<Slot | null>(null);
const showGrid = ref(true);
const showAxes = ref(true);

let scene: THREE.Scene;
let camera: THREE.PerspectiveCamera;
let renderer: THREE.WebGLRenderer;
let controls: OrbitControls;
let gridHelper: THREE.GridHelper;
let axesHelper: THREE.AxesHelper;
let raycaster: THREE.Raycaster;
let mouse: THREE.Vector2;
let slotMeshes: Map<string, THREE.Mesh> = new Map();

onMounted(() => {
  if (!viewerEl.value) return;
  
  initScene();
  updateVisualization();
  animate();
  
  window.addEventListener('resize', onWindowResize);
  viewerEl.value.addEventListener('mousemove', onMouseMove);
});

onUnmounted(() => {
  window.removeEventListener('resize', onWindowResize);
  if (viewerEl.value) {
    viewerEl.value.removeEventListener('mousemove', onMouseMove);
  }
  
  if (renderer) {
    renderer.dispose();
  }
});

watch(() => [props.dimensions, props.slots, props.chambers], () => {
  updateVisualization();
}, { deep: true });

watch(showGrid, (value) => {
  if (gridHelper) {
    gridHelper.visible = value;
  }
});

watch(showAxes, (value) => {
  if (axesHelper) {
    axesHelper.visible = value;
  }
});

function initScene() {
  if (!viewerEl.value) return;
  
  // Scene
  scene = new THREE.Scene();
  scene.background = new THREE.Color(0x1a1a1a);
  
  // Camera
  camera = new THREE.PerspectiveCamera(
    75,
    viewerEl.value.clientWidth / viewerEl.value.clientHeight,
    0.1,
    10000
  );
  camera.position.set(400, 300, 400);
  
  // Renderer
  renderer = new THREE.WebGLRenderer({ antialias: true });
  renderer.setSize(viewerEl.value.clientWidth, viewerEl.value.clientHeight);
  renderer.setPixelRatio(window.devicePixelRatio);
  viewerEl.value.appendChild(renderer.domElement);
  
  // Controls
  controls = new OrbitControls(camera, renderer.domElement);
  controls.enableDamping = true;
  controls.dampingFactor = 0.05;
  
  // Lights
  const ambientLight = new THREE.AmbientLight(0xffffff, 0.6);
  scene.add(ambientLight);
  
  const directionalLight = new THREE.DirectionalLight(0xffffff, 0.8);
  directionalLight.position.set(100, 200, 100);
  scene.add(directionalLight);
  
  // Grid
  gridHelper = new THREE.GridHelper(1000, 20);
  scene.add(gridHelper);
  
  // Axes
  axesHelper = new THREE.AxesHelper(150);
  scene.add(axesHelper);
  
  // Raycaster for hover detection
  raycaster = new THREE.Raycaster();
  mouse = new THREE.Vector2();
}

function updateVisualization() {
  // Clear existing slot meshes
  slotMeshes.forEach(mesh => {
    scene.remove(mesh);
    mesh.geometry.dispose();
    if (Array.isArray(mesh.material)) {
      mesh.material.forEach(m => m.dispose());
    } else {
      mesh.material.dispose();
    }
  });
  slotMeshes.clear();
  
  // Render main product dimensions if available
  if (props.dimensions) {
    const geometry = new THREE.BoxGeometry(
      props.dimensions.length,
      props.dimensions.height,
      props.dimensions.width
    );
    
    const material = new THREE.MeshStandardMaterial({
      color: 0x666666,
      transparent: true,
      opacity: 0.2
    });
    
    const mesh = new THREE.Mesh(geometry, material);
    mesh.position.set(
      props.dimensions.length / 2,
      props.dimensions.height / 2,
      props.dimensions.width / 2
    );
    
    const edges = new THREE.EdgesGeometry(geometry);
    const lineMaterial = new THREE.LineBasicMaterial({ color: 0xaaaaaa, linewidth: 2 });
    const wireframe = new THREE.LineSegments(edges, lineMaterial);
    mesh.add(wireframe);
    
    scene.add(mesh);
  }
  
  // Render chambers
  if (props.chambers && props.chambers.length > 0) {
    props.chambers.forEach((chamber) => {
      const chamberGeom = new THREE.BoxGeometry(
        chamber.dimensions.length,
        chamber.dimensions.height,
        chamber.dimensions.width
      );
      
      const chamberMat = new THREE.MeshStandardMaterial({
        color: 0x4444ff,
        transparent: true,
        opacity: 0.15,
        wireframe: false
      });
      
      const chamberMesh = new THREE.Mesh(chamberGeom, chamberMat);
      chamberMesh.position.set(
        chamber.dimensions.length / 2,
        chamber.dimensions.height / 2,
        chamber.dimensions.width / 2
      );
      
      const chamberEdges = new THREE.EdgesGeometry(chamberGeom);
      const chamberWireframe = new THREE.LineSegments(
        chamberEdges,
        new THREE.LineBasicMaterial({ color: 0x6666ff, linewidth: 2 })
      );
      chamberMesh.add(chamberWireframe);
      
      scene.add(chamberMesh);
      
      // Render slots in chamber
      if (chamber.slots) {
        chamber.slots.forEach(slot => renderSlot(slot));
      }
    });
  }
  
  // Render direct slots
  if (props.slots && props.slots.length > 0) {
    props.slots.forEach(slot => renderSlot(slot));
  }
  
  // Adjust camera to fit content
  fitCameraToContent();
}

function renderSlot(slot: Slot) {
  const slotGeom = new THREE.BoxGeometry(
    slot.maxDimensions.length,
    slot.maxDimensions.height,
    slot.maxDimensions.width
  );
  
  const slotMat = new THREE.MeshStandardMaterial({
    color: getSlotColor(slot.allowedCategory),
    transparent: true,
    opacity: 0.4
  });
  
  const slotMesh = new THREE.Mesh(slotGeom, slotMat);
  
  // Position
  slotMesh.position.set(
    slot.relativePosition.x + slot.maxDimensions.length / 2,
    slot.relativePosition.y + slot.maxDimensions.height / 2,
    slot.relativePosition.z + slot.maxDimensions.width / 2
  );
  
  // Rotation
  if (slot.rotation) {
    slotMesh.rotation.set(
      (slot.rotation.x * Math.PI) / 180,
      (slot.rotation.y * Math.PI) / 180,
      (slot.rotation.z * Math.PI) / 180
    );
  }
  
  const slotEdges = new THREE.EdgesGeometry(slotGeom);
  const slotWireframe = new THREE.LineSegments(
    slotEdges,
    new THREE.LineBasicMaterial({ color: 0xffffff, linewidth: 1 })
  );
  slotMesh.add(slotWireframe);
  
  slotMesh.userData = { slot };
  scene.add(slotMesh);
  slotMeshes.set(slot.name, slotMesh);
  
  // Render sub-slots
  if (slot.subSlots) {
    slot.subSlots.forEach(subSlot => renderSlot(subSlot));
  }
}

function getSlotColor(category: string): number {
  const colors: Record<string, number> = {
    'CPU': 0xff6b6b,
    'GPU': 0x4ecdc4,
    'Motherboard': 0x45b7d1,
    'RAM': 0x96ceb4,
    'Storage': 0xffeaa7,
    'PowerSupply': 0xfd79a8,
    'Cooler': 0x74b9ff,
    'Case': 0xa29bfe
  };
  return colors[category] || 0x888888;
}

function fitCameraToContent() {
  // Calculate bounding box of all content
  const box = new THREE.Box3();
  
  scene.traverse((object) => {
    if (object instanceof THREE.Mesh && !(object instanceof THREE.GridHelper) && !(object instanceof THREE.AxesHelper)) {
      box.expandByObject(object);
    }
  });
  
  if (box.isEmpty()) {
    // Default view if no content
    camera.position.set(400, 300, 400);
    controls.target.set(0, 0, 0);
  } else {
    const center = box.getCenter(new THREE.Vector3());
    const size = box.getSize(new THREE.Vector3());
    const maxDim = Math.max(size.x, size.y, size.z);
    const fov = camera.fov * (Math.PI / 180);
    let cameraZ = Math.abs(maxDim / 2 / Math.tan(fov / 2));
    cameraZ *= 1.5; // Add some padding
    
    camera.position.set(center.x + cameraZ * 0.7, center.y + cameraZ * 0.5, center.z + cameraZ * 0.7);
    controls.target.copy(center);
  }
  
  controls.update();
}

function resetCamera() {
  fitCameraToContent();
}

function animate() {
  requestAnimationFrame(animate);
  controls.update();
  renderer.render(scene, camera);
}

function onWindowResize() {
  if (!viewerEl.value) return;
  
  camera.aspect = viewerEl.value.clientWidth / viewerEl.value.clientHeight;
  camera.updateProjectionMatrix();
  renderer.setSize(viewerEl.value.clientWidth, viewerEl.value.clientHeight);
}

function onMouseMove(event: MouseEvent) {
  if (!viewerEl.value) return;
  
  const rect = viewerEl.value.getBoundingClientRect();
  mouse.x = ((event.clientX - rect.left) / rect.width) * 2 - 1;
  mouse.y = -((event.clientY - rect.top) / rect.height) * 2 + 1;
  
  raycaster.setFromCamera(mouse, camera);
  
  const intersects = raycaster.intersectObjects(Array.from(slotMeshes.values()));
  
  if (intersects.length > 0 && intersects[0]) {
    const intersected = intersects[0].object as THREE.Mesh;
    hoveredSlot.value = intersected.userData?.slot || null;
  } else {
    hoveredSlot.value = null;
  }
}
</script>

<style scoped>
.viewer-container {
  position: relative;
  width: 100%;
  height: 500px;
  border-radius: 4px;
  overflow: hidden;
  background: #1a1a1a;
}

.viewer-canvas {
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
