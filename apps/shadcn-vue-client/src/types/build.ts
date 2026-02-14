/**
 * Build-related types
 */

import type { Vector3, Rotation, Dimensions, Slot, Chamber } from './spatial';

export type HttpMethod = 'GET' | 'POST' | 'PUT' | 'DELETE' | 'PATCH';

export interface HateoasLink {
  href: string;
  rel: string;
  method: HttpMethod;
}

export interface Build {
  id: string;
  name: string;
  parts: BuildPart[];
  createdAt: string;
  updatedAt: string;
}

export interface GetBuildsResponseItem {
  id: string;
  name: string;
  totalPrice: number;
  links: HateoasLink[];
}

export interface GetBuildsResponse {
  items: GetBuildsResponseItem[];
  links: HateoasLink[];
}

export interface GetBuildResponse {
  id: string;
  name: string;
  userId: string;
  parts: BuildPart[];
  isCompatible: boolean;
  compatibilityIssues: CompatibilityIssue[];
  createdAt: string;
  links?: HateoasLink[];
}

export interface CreateBuildRequest {
  name: string;
  userId: string;
}

export interface CreateBuildResponse {
  id: string;
  name: string;
  userId: string;
  links?: HateoasLink[];
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
  hasErrors: boolean;
  hasWarnings: boolean;
  issues: CompatibilityIssue[];
  products: BuildPart[];
  links?: HateoasLink[];
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
  links?: HateoasLink[];
}

export interface AddPartToSlotRequest {
  productId: string;
  pricePaid: number;
  slotId: string;
  parentProductId: string;
}
