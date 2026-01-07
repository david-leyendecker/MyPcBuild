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

export interface FieldDefinition {
  name: string;
  type: string;
  required: boolean;
  unit: string | null;
  options: string[] | null;
}

export interface CreateProductRequest {
  category: string;
  name: string;
  price: number;
  manufacturer: string;
  fields: Record<string, string>;
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
  },

  async getFieldDefinitions(category: string): Promise<FieldDefinition[]> {
    const response = await apiClient.get<{ category: string; fields: FieldDefinition[] }>(
      `/catalog/field-definitions/${category}`
    );
    return response.data.fields;
  },

  async createProduct(request: CreateProductRequest): Promise<{ id: string }> {
    const response = await apiClient.post<{ id: string }>('/catalog/products', request);
    return response.data;
  }
};
