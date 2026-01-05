import apiClient from './client';

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
  manufacturer: string;
  pricePaid: number;
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

  async removePart(buildId: string, productId: string): Promise<void> {
    await apiClient.delete(`/builds/${buildId}/parts/${productId}`);
  },

  async validateBuild(buildId: string): Promise<BuildValidation> {
    const response = await apiClient.get<BuildValidation>(`/builds/${buildId}/compatibility`);
    return response.data;
  }
};
