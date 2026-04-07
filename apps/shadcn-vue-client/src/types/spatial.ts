/**
 * Spatial types for 3D positioning and dimensions
 */

export type ProductCategory = 'cpu' | 'gpu' | 'motherboard' | 'ram' | 'storage' | 'powersupply' | 'cooler' | 'case';

export interface Vector3 {
  x: number;
  y: number;
  z: number;
}

export interface Rotation {
  x: number;
  y: number;
  z: number;
}

export interface Dimensions {
  length: number;
  width: number;
  height: number;
}

export interface Slot {
  id?: string;
  name: string;
  allowedCategory: ProductCategory;
  relativePosition: Vector3;
  maxDimensions: Dimensions;
  rotation?: Rotation | null;
  subSlots?: Slot[];
}

export interface Chamber {
  id?: string;
  name: string;
  relativePosition: Vector3;
  dimensions: Dimensions;
  slots: Slot[];
}
