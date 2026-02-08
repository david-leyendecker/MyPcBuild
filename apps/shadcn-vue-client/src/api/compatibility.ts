/**
 * Compatibility validation API
 */

import apiClient from './client';
import type { BuildValidation } from '@/types/build';

export const compatibilityApi = {
  /**
   * Validates a build for compatibility issues
   */
  async validateBuild(buildId: string): Promise<BuildValidation> {
    const response = await apiClient.get<BuildValidation>(`/builds/${buildId}/compatibility`);
    return response.data;
  }
};
