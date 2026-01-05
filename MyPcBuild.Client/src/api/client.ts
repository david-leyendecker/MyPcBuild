import axios, { type AxiosInstance } from 'axios';

// Get API service URL from Aspire service discovery
const httpsUrl = import.meta.env.services__apiservice__https__0;
const httpUrl = import.meta.env.services__apiservice__http__0;

// Prefer HTTPS, fallback to HTTP, or throw error if neither is available
const serviceUrl = httpsUrl || httpUrl;

if (!serviceUrl) {
  throw new Error(
    'API service URL not found. Please ensure either services__apiservice__https__0 or services__apiservice__http__0 environment variable is set.'
  );
}

const baseURL = `${serviceUrl}/api`;

const apiClient: AxiosInstance = axios.create({
  baseURL,
  headers: {
    'Content-Type': 'application/json'
  }
});

export default apiClient;
