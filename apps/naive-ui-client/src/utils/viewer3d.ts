import * as THREE from 'three';

export interface DisposableObject {
  geometry?: THREE.BufferGeometry;
  material?: THREE.Material | THREE.Material[];
  traverse?: (callback: (child: any) => void) => void;
}

/**
 * Dispose of Three.js objects and free GPU memory
 */
export function disposeMaterial(material: THREE.Material | THREE.Material[]) {
  if (Array.isArray(material)) {
    material.forEach((m) => m.dispose());
  } else {
    material.dispose();
  }
}

export function disposeMesh(mesh: THREE.Mesh) {
  mesh.geometry.dispose();
  disposeMaterial(mesh.material);
}

export function disposeObject(object: DisposableObject) {
  if (object.geometry instanceof THREE.BufferGeometry) {
    object.geometry.dispose();
  }

  if (object.material) {
    disposeMaterial(object.material);
  }

  if (object.traverse) {
    object.traverse((child) => {
      if (child instanceof THREE.Mesh) {
        child.geometry.dispose();
        disposeMaterial(child.material);
      }
      if (child instanceof THREE.LineSegments) {
        child.geometry.dispose();
        disposeMaterial(child.material);
      }
    });
  }
}

/**
 * Clear all meshes from a map and dispose of their resources
 */
export function clearMeshMap(
  meshMap: Map<string, THREE.Mesh>,
  scene: THREE.Scene
) {
  meshMap.forEach((mesh) => {
    scene.remove(mesh);
    disposeObject(mesh);
  });
  meshMap.clear();
}

/**
 * Clear all meshes from an array and dispose of their resources
 */
export function clearMeshArray(
  meshArray: THREE.Mesh[],
  scene: THREE.Scene
) {
  meshArray.forEach((mesh) => {
    scene.remove(mesh);
    disposeObject(mesh);
  });
  meshArray.length = 0;
}

/**
 * Create a box mesh with wireframe edges
 */
export function createBoxMesh(
  width: number,
  height: number,
  depth: number,
  color: number,
  opacity: number = 0.8
): THREE.Mesh {
  const geometry = new THREE.BoxGeometry(width, height, depth);
  // Offset geometry so that corner (0,0,0) aligns with mesh position
  geometry.translate(width / 2, height / 2, depth / 2);
  
  const material = new THREE.MeshStandardMaterial({
    color,
    transparent: true,
    opacity
  });

  const mesh = new THREE.Mesh(geometry, material);

  // Add wireframe
  const edges = new THREE.EdgesGeometry(geometry);
  const lineMaterial = new THREE.LineBasicMaterial({ color: 0xffffff, linewidth: 1 });
  const wireframe = new THREE.LineSegments(edges, lineMaterial);
  mesh.add(wireframe);

  return mesh;
}

/**
 * Create a box mesh with custom wireframe color
 */
export function createBoxMeshWithWireframeColor(
  width: number,
  height: number,
  depth: number,
  color: number,
  wireframeColor: number,
  opacity: number = 0.8
): THREE.Mesh {
  const geometry = new THREE.BoxGeometry(width, height, depth);
  // Offset geometry so that corner (0,0,0) aligns with mesh position
  geometry.translate(width / 2, height / 2, depth / 2);
  
  const material = new THREE.MeshStandardMaterial({
    color,
    transparent: true,
    opacity
  });

  const mesh = new THREE.Mesh(geometry, material);

  // Add wireframe with custom color
  const edges = new THREE.EdgesGeometry(geometry);
  const lineMaterial = new THREE.LineBasicMaterial({ color: wireframeColor, linewidth: 1 });
  const wireframe = new THREE.LineSegments(edges, lineMaterial);
  mesh.add(wireframe);

  return mesh;
}

/**
 * Common color mappings for product categories
 */
export const CATEGORY_COLORS: Record<string, number> = {
  'CPU': 0xff6b6b,
  'GPU': 0x4ecdc4,
  'Motherboard': 0x45b7d1,
  'RAM': 0x96ceb4,
  'Storage': 0xffeaa7,
  'PowerSupply': 0xfd79a8,
  'Cooler': 0x74b9ff,
  'Case': 0xa29bfe,
  // Build viewer colors
  'CPU_BUILD': 0x3b82f6,
  'GPU_BUILD': 0x10b981,
  'Motherboard_BUILD': 0x8b5cf6,
  'RAM_BUILD': 0xf59e0b,
  'Storage_BUILD': 0xef4444,
  'PowerSupply_BUILD': 0x06b6d4,
  'Cooler_BUILD': 0xec4899,
  'Case_BUILD': 0x6b7280,
};

/**
 * Get color for a category (product viewer style)
 */
export function getProductCategoryColor(category: string): number {
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

/**
 * Get color for a category (build viewer style)
 */
export function getBuildCategoryColor(category: string): number {
  const colors: Record<string, number> = {
    'CPU': 0x3b82f6,
    'GPU': 0x10b981,
    'Motherboard': 0x8b5cf6,
    'RAM': 0xf59e0b,
    'Storage': 0xef4444,
    'PowerSupply': 0x06b6d4,
    'Cooler': 0xec4899,
    'Case': 0x6b7280
  };
  return colors[category] || 0x9ca3af;
}

/**
 * Convert degrees to radians
 */
export function degreesToRadians(degrees: number): number {
  return (degrees * Math.PI) / 180;
}

/**
 * Perform raycasting intersection test
 */
export function getIntersectedMesh(
  raycaster: THREE.Raycaster,
  meshObjects: THREE.Mesh[]
): THREE.Mesh | null {
  const intersects = raycaster.intersectObjects(meshObjects);
  return intersects.length > 0 && intersects[0] ? (intersects[0].object as THREE.Mesh) : null;
}

/**
 * Update raycaster from mouse position
 */
export function updateRaycasterFromMouse(
  raycaster: THREE.Raycaster,
  mouse: THREE.Vector2,
  camera: THREE.PerspectiveCamera,
  containerRect: DOMRect
): void {
  mouse.x = ((event as MouseEvent).clientX - containerRect.left) / containerRect.width * 2 - 1;
  mouse.y = -((event as MouseEvent).clientY - containerRect.top) / containerRect.height * 2 + 1;
  raycaster.setFromCamera(mouse, camera);
}
