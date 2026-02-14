import axios, { type AxiosInstance } from 'axios';

// Get API service URL from Aspire service discovery
const httpsUrl = import.meta.env.services__apiservice__https__0;
const httpUrl = import.meta.env.services__apiservice__http__0;

// Prefer HTTPS, fallback to HTTP, or use dev default
const serviceUrl = httpsUrl || httpUrl || 'http://localhost:5000';

const baseURL = `${serviceUrl}/api`;

const apiClient: AxiosInstance = axios.create({
  baseURL,
  headers: {
    'Content-Type': 'application/json'
  }
});

export default apiClient;
