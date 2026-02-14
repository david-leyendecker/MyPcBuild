/**
 * Catalog API client
 */

import apiClient from './client';
import type { ProductCategory } from '@/types/product';

export const ProductCategoryConst = {
  CPU: 'cpu',
  GPU: 'gpu',
  Motherboard: 'motherboard',
  RAM: 'ram',
  Storage: 'storage',
  PowerSupply: 'powersupply',
  Cooler: 'cooler',
  Case: 'case'
} as const;

// Central mapping for all category-related conversions
export const categoryMapping = {
  'cpu': {
    displayName: 'Central Processing Unit',
    backendEnumName: 'cpu'
  },
  'gpu': {
    displayName: 'Graphics Processing Unit',
    backendEnumName: 'gpu'
  },
  'motherboard': {
    displayName: 'Motherboard',
    backendEnumName: 'motherboard'
  },
  'ram': {
    displayName: 'Memory (RAM)',
    backendEnumName: 'ram'
  },
  'storage': {
    displayName: 'Storage',
    backendEnumName: 'storage'
  },
  'powersupply': {
    displayName: 'Power Supply',
    backendEnumName: 'powersupply'
  },
  'cooler': {
    displayName: 'CPU/Case Cooler',
    backendEnumName: 'cooler'
  },
  'case': {
    displayName: 'PC Case',
    backendEnumName: 'case'
  }
} as const;

// Derived convenience exports
export const categoryLabels: Record<ProductCategory, string> = Object.entries(categoryMapping).reduce(
  (acc, [key, value]) => {
    acc[key as ProductCategory] = value.displayName;
    return acc;
  },
  {} as Record<ProductCategory, string>
);

// Helper functions for category conversions
export function getCategoryBackendValue(category: ProductCategory | string): string {
  return categoryMapping[category as ProductCategory]?.backendEnumName || category;
}

export function getCategoryFromBackend(backendName: string): ProductCategory | null {
  const entry = Object.entries(categoryMapping).find(
    ([, value]) => value.backendEnumName === backendName
  );
  return entry ? (entry[0] as ProductCategory) : null;
}

export function getCategoryDisplayName(category: ProductCategory | string): string {
  return categoryMapping[category as ProductCategory]?.displayName || category;
}

export interface Product {
  id: string;
  name: string;
  category: ProductCategory;
  price: number;
  manufacturer: string;
  isDraft : boolean;
  publishedAt: string | null;
  specifications: Record<string, string | number>;
}

export interface ProductSummary {
  id: string;
  name: string;
  categoryName: string;
  price: number;
  manufacturer: string;
  isDraft: boolean;
  publishedAt: string | null;
}

export interface GetProductsParams {
  filters?: string;
  search?: string;
  page?: number;
  itemsPerPage?: number;
  sortBy?: string;
  sortDesc?: boolean;
}

export interface PaginationMetadata {
  total: number;
  page: number;
  itemsPerPage: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface GetProductsResponse {
  items: ProductSummary[];
  pagination: PaginationMetadata;
}

export interface CatalogSearchParams {
  category?: ProductCategory;
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
  category: ProductCategory;
  name: string;
  price: number;
  manufacturer: string;
  fields: Record<string, string>;
}

export interface GenerateProductRequest {
  category: ProductCategory;
  description: string;
}

export interface GenerateProductResponse {
  id: string;
  product: Product;
}

export interface PublishProductResponse {
  id: string;
  product: Product;
}

export const catalogApi = {
  async getProducts(params: GetProductsParams): Promise<GetProductsResponse> {
    const response = await apiClient.get<GetProductsResponse>('/catalog/products', { params });
    return response.data;
  },

  async searchProducts(params: CatalogSearchParams): Promise<Product[]> {
    const response = await apiClient.get<Product[]>('/catalog/search', { params });
    return response.data;
  },

  async getProductsByCategory(category: ProductCategory): Promise<Product[]> {
    const response = await apiClient.get<Product[]>(`/catalog/category/${category}`);
    return response.data;
  },

  async getProduct(id: string): Promise<Product> {
    const response = await apiClient.get<{ product: Product }>(`/catalog/products/${id}`);
    return response.data.product;
  },

  async getFieldDefinitions(category: ProductCategory): Promise<FieldDefinition[]> {
    const response = await apiClient.get<{ category: string; fields: FieldDefinition[] }>(
      `/catalog/field-definitions/${category}`
    );
    return response.data.fields;
  },

  async createProduct(request: CreateProductRequest): Promise<{ id: string }> {
    const response = await apiClient.post<{ id: string }>('/catalog/products', request);
    return response.data;
  },

  async generateProductWithAi(request: GenerateProductRequest): Promise<GenerateProductResponse> {
    const response = await apiClient.post<GenerateProductResponse>('/catalog/products/generate-with-ai', request);
    return response.data;
  },

  async publishProduct(id: string): Promise<PublishProductResponse> {
    const response = await apiClient.post<PublishProductResponse>(`/catalog/products/${id}/publish`);
    return response.data;
  },

  async updateProduct(id: string, request: CreateProductRequest): Promise<{ id: string }> {
    const response = await apiClient.put<{ id: string }>(`/catalog/products/${id}`, request);
    return response.data;
  }
};
