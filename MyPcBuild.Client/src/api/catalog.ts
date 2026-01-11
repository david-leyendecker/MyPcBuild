import apiClient from './client';

export interface Product {
  id: string;
  name: string;
  category: string;
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

export interface GenerateProductRequest {
  category: string;
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

  async getProductsByCategory(category: string): Promise<Product[]> {
    const response = await apiClient.get<Product[]>(`/catalog/category/${category}`);
    return response.data;
  },

  async getProduct(id: string): Promise<Product> {
    const response = await apiClient.get<Product>(`/catalog/products/${id}`);
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
  },

  async generateProductWithAi(request: GenerateProductRequest): Promise<GenerateProductResponse> {
    const response = await apiClient.post<GenerateProductResponse>('/catalog/products/generate-with-ai', request);
    return response.data;
  },

  async publishProduct(id: string): Promise<PublishProductResponse> {
    const response = await apiClient.post<PublishProductResponse>(`/catalog/products/${id}/publish`);
    return response.data;
  }
};
