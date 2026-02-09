/**
 * Builds API client
 */

import apiClient from './client';
import type {
  GetBuildsResponse,
  GetBuildResponse,
  CreateBuildRequest,
  CreateBuildResponse,
  BuildValidation,
  AvailableSlot,
  AddPartToSlotRequest
} from '@/types/build';

export const buildsApi = {
  async getBuilds(): Promise<GetBuildsResponse> {
    const response = await apiClient.get<GetBuildsResponse>('/builds');
    return response.data;
  },

  async getBuild(id: string): Promise<GetBuildResponse> {
    const response = await apiClient.get<GetBuildResponse>(`/builds/${id}`);
    return response.data;
  },

  async createBuild(name: string, userId: string): Promise<CreateBuildResponse> {
    const request: CreateBuildRequest = { name, userId };
    const response = await apiClient.post<CreateBuildResponse>('/builds', request);
    return response.data;
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
