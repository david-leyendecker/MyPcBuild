/**
 * Updated catalog API client for working with typed product DTOs
 */

import apiClient from './client';
import type {
  ProductRequest,
  ProductResponse,
  CpuProductRequest,
  CpuProductResponse,
  GpuProductRequest,
  GpuProductResponse,
  MotherboardProductRequest,
  MotherboardProductResponse,
  RamProductRequest,
  RamProductResponse,
  StorageProductRequest,
  StorageProductResponse,
  PsuProductRequest,
  PsuProductResponse,
  CoolerProductRequest,
  CoolerProductResponse,
  PcCaseProductRequest,
  PcCaseProductResponse
} from '@/types/products';
import { ProductCategory as TypedProductCategory } from '@/types/products';

// Re-export old types for compatibility during migration
export { ProductCategory, categoryLabels, categoryMapping, getCategoryBackendValue, getCategoryFromBackend, getCategoryDisplayName } from './catalog';
export type { Product, ProductSummary, GetProductsParams, PaginationMetadata, GetProductsResponse, CatalogSearchParams, FieldDefinition, CreateProductRequest, GenerateProductRequest, GenerateProductResponse, PublishProductResponse } from './catalog';

/**
 * Creates a product using the new typed API
 */
export async function createTypedProduct(request: ProductRequest): Promise<{ id: string }> {
  const response = await apiClient.post<{ id: string }>('/catalog/products', request);
  return response.data;
}

/**
 * Gets a product by ID using the new typed API
 */
export async function getTypedProduct(id: string): Promise<ProductResponse> {
  const response = await apiClient.get<ProductResponse>(`/catalog/products/${id}`);
  return response.data;
}

/**
 * Updates a product using the new typed API
 */
export async function updateTypedProduct(id: string, request: ProductRequest): Promise<{ id: string }> {
  const response = await apiClient.put<{ id: string }>(`/catalog/products/${id}`, request);
  return response.data;
}
