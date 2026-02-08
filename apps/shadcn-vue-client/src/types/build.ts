/**
 * Build-related types
 */

import type { Vector3, Rotation, Dimensions, Slot, Chamber } from './spatial';

export interface Build {
  id: string;
  name: string;
  parts: BuildPart[];
  createdAt: string;
  updatedAt: string;
}

export interface GetBuildsResponse {
  id: string;
  name: string;
  totalPrice: number;
}

export interface GetBuildResponse {
  id: string;
  name: string;
  userId: string;
  parts: BuildPart[];
  isCompatible: boolean;
  compatibilityIssues: CompatibilityIssue[];
  createdAt: string;
}

export interface CreateBuildResponse {
  id: string;
  name: string;
  userId: string;
}

export interface BuildPart {
  id: string;
  name: string;
  category: string;
  categoryName: string;
  manufacturer: string;
  pricePaid: number;
  slotId?: string | null;
  position?: Vector3 | null;
  rotation?: Rotation | null;
  dimensions?: Dimensions | null;
  slots?: Slot[] | null;
  chambers?: Chamber[] | null;
}

export interface CompatibilityIssue {
  message: string;
  severity: 'Error' | 'Warning';
  category: string;
}

export interface BuildValidation {
  buildId: string;
  buildName: string;
  isCompatible: boolean;
  hasErrros: boolean;
  hasWarnings: boolean;
  issues: CompatibilityIssue[];
  products: BuildPart[];
}

export interface AvailableSlot {
  id: string;
  name: string;
  allowedCategory: string;
  absolutePosition: Vector3;
  maxDimensions: Dimensions;
  rotation?: Rotation | null;
  isOccupied: boolean;
  parentProductId: string;
  parentProductName: string;
}

export interface AddPartToSlotRequest {
  productId: string;
  pricePaid: number;
  slotId: string;
  parentProductId: string;
}
