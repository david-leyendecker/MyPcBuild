import apiClient from './client';

export interface Vector3 {
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
  id: string;
  name: string;
  allowedCategory: string;
  relativePosition: Vector3;
  maxDimensions: Dimensions;
}

export interface Chamber {
  id: string;
  name: string;
  dimensions: Dimensions;
  slots: Slot[];
}

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
  isOccupied: boolean;
  parentProductId: string;
  parentProductName: string;
}

export interface AddPartToSlotRequest {
  productId: string;
  pricePaid: number;
  slotId: string;
  position: Vector3;
}

export const buildsApi = {
  async getBuilds(): Promise<GetBuildsResponse[]> {
    const response = await apiClient.get<GetBuildsResponse[]>('/builds');
    return response.data;
  },

  async getBuild(id: string): Promise<GetBuildResponse> {
    const response = await apiClient.get<GetBuildResponse>(`/builds/${id}`);
    return response.data;
  },

  async createBuild(name: string): Promise<CreateBuildResponse> {
    const response = await apiClient.post<CreateBuildResponse>('/builds', { name });
    return response.data;
  },

  async updateBuild(buildId: string, name: string): Promise<void> {
    await apiClient.put(`/builds/${buildId}`, { name });
  },

  async addPart(buildId: string, productId: string, pricePaid: number = 0): Promise<void> {
    await apiClient.post(`/builds/${buildId}/parts`, { productId, pricePaid });
  },

  async addPartToSlot(buildId: string, request: AddPartToSlotRequest): Promise<void> {
    await apiClient.post(`/builds/${buildId}/parts/slot`, request);
  },

  async removePart(buildId: string, productId: string): Promise<void> {
    await apiClient.delete(`/builds/${buildId}/parts/${productId}`);
  },

  async validateBuild(buildId: string): Promise<BuildValidation> {
    const response = await apiClient.get<BuildValidation>(`/builds/${buildId}/compatibility`);
    return response.data;
  },

  async getAvailableSlots(buildId: string): Promise<AvailableSlot[]> {
    const response = await apiClient.get<AvailableSlot[]>(`/builds/${buildId}/slots`);
    return response.data;
  }
};

