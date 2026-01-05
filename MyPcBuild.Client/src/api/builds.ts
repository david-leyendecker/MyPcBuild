import apiClient from './client';

export interface Build {
  id: string;
  name: string;
  parts: BuildPart[];
  createdAt: string;
  updatedAt: string;
}

export interface BuildPart {
  productId: string;
  productName: string;
  category: string;
  price: number;
}

export interface CompatibilityIssue {
  message: string;
  severity: 'Error' | 'Warning';
}

export interface BuildValidation {
  isValid: boolean;
  issues: CompatibilityIssue[];
}

export const buildsApi = {
  async getBuilds(): Promise<Build[]> {
    const response = await apiClient.get<Build[]>('/builds');
    return response.data;
  },

  async getBuild(id: string): Promise<Build> {
    const response = await apiClient.get<Build>(`/builds/${id}`);
    return response.data;
  },

  async createBuild(name: string): Promise<Build> {
    const response = await apiClient.post<Build>('/builds', { name });
    return response.data;
  },

  async updateBuild(id: string, name: string): Promise<Build> {
    const response = await apiClient.put<Build>(`/builds/${id}`, { name });
    return response.data;
  },

  async addPart(buildId: string, productId: string): Promise<Build> {
    const response = await apiClient.post<Build>(`/builds/${buildId}/parts`, { productId });
    return response.data;
  },

  async removePart(buildId: string, productId: string): Promise<Build> {
    const response = await apiClient.delete<Build>(`/builds/${buildId}/parts/${productId}`);
    return response.data;
  },

  async validateBuild(buildId: string): Promise<BuildValidation> {
    const response = await apiClient.get<BuildValidation>(`/builds/${buildId}/validate`);
    return response.data;
  }
};
