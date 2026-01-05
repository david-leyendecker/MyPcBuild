import apiClient from './client';

export interface Product {
  id: string;
  name: string;
  category: string;
  price: number;
  specifications: Record<string, string | number>;
}

export interface CatalogSearchParams {
  category?: string;
  search?: string;
  limit?: number;
  offset?: number;
}

export const catalogApi = {
  async searchProducts(params: CatalogSearchParams): Promise<Product[]> {
    const response = await apiClient.get<Product[]>('/catalog/search', { params });
    return response.data;
  },

  async getProductsByCategory(category: string): Promise<Product[]> {
    const response = await apiClient.get<Product[]>(`/catalog/category/${category}`);
    return response.data;
  },

  async getProduct(id: string): Promise<Product> {
    const response = await apiClient.get<Product>(`/catalog/${id}`);
    return response.data;
  }
};
