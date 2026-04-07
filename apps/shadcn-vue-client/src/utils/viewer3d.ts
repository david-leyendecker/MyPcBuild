import * as THREE from 'three';

export interface DisposableObject {
  geometry?: THREE.BufferGeometry;
  material?: THREE.Material | THREE.Material[];
  traverse?: (callback: (child: any) => void) => void;
}

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

export function createBoxMesh(
  width: number,
  height: number,
  depth: number,
  color: number,
  opacity: number = 0.8
): THREE.Mesh {
  const geometry = new THREE.BoxGeometry(width, height, depth);
  geometry.translate(width / 2, height / 2, depth / 2);
  
  const material = new THREE.MeshStandardMaterial({
    color,
    transparent: true,
    opacity
  });

  const mesh = new THREE.Mesh(geometry, material);

  const edges = new THREE.EdgesGeometry(geometry);
  const lineMaterial = new THREE.LineBasicMaterial({ color: 0xffffff, linewidth: 1 });
  const wireframe = new THREE.LineSegments(edges, lineMaterial);
  mesh.add(wireframe);

  return mesh;
}

export function createBoxMeshWithWireframeColor(
  width: number,
  height: number,
  depth: number,
  color: number,
  wireframeColor: number,
  opacity: number = 0.8
): THREE.Mesh {
  const geometry = new THREE.BoxGeometry(width, height, depth);
  geometry.translate(width / 2, height / 2, depth / 2);
  
  const material = new THREE.MeshStandardMaterial({
    color,
    transparent: true,
    opacity
  });

  const mesh = new THREE.Mesh(geometry, material);

  const edges = new THREE.EdgesGeometry(geometry);
  const lineMaterial = new THREE.LineBasicMaterial({ color: wireframeColor, linewidth: 1 });
  const wireframe = new THREE.LineSegments(edges, lineMaterial);
  mesh.add(wireframe);

  return mesh;
}

export const CATEGORY_COLORS: Record<string, number> = {
  'cpu': 0xff6b6b,
  'gpu': 0x4ecdc4,
  'motherboard': 0x45b7d1,
  'ram': 0x96ceb4,
  'storage': 0xffeaa7,
  'powersupply': 0xfd79a8,
  'cooler': 0x74b9ff,
  'case': 0xa29bfe,
  'cpu_build': 0x3b82f6,
  'gpu_build': 0x10b981,
  'motherboard_build': 0x8b5cf6,
  'ram_build': 0xf59e0b,
  'storage_build': 0xef4444,
  'powersupply_build': 0x06b6d4,
  'cooler_build': 0xec4899,
  'case_build': 0x6b7280,
};

export function getProductCategoryColor(category: string): number {
  return CATEGORY_COLORS[category.toLowerCase()] || 0x888888;
}

export function getBuildCategoryColor(category: string): number {
  const key = `${category.toLowerCase()}_build`;
  return CATEGORY_COLORS[key] || 0x9ca3af;
}

export function degreesToRadians(degrees: number): number {
  return (degrees * Math.PI) / 180;
}

export function getIntersectedMesh(
  raycaster: THREE.Raycaster,
  meshObjects: THREE.Mesh[]
): THREE.Mesh | null {
  const intersects = raycaster.intersectObjects(meshObjects);
  return intersects.length > 0 && intersects[0] ? (intersects[0].object as THREE.Mesh) : null;
}
