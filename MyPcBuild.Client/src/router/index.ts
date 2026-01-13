import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router';
import BuildsListView from '@/views/BuildsListView.vue';
import BuildDetailView from '@/views/BuildDetailView.vue';
import CatalogView from '@/views/CatalogView.vue';
import ProductCreateView from '@/views/ProductCreateView.vue';
import ProductDetailView from '@/views/ProductDetailView.vue';

const routes: Array<RouteRecordRaw> = [
  {
    path: '/',
    name: 'builds-list',
    component: BuildsListView
  },
  {
    path: '/builds/:id',
    name: 'build-detail',
    component: BuildDetailView,
    props: true
  },
  {
    path: '/catalog',
    name: 'catalog',
    component: CatalogView
  },
  {
    path: '/catalog/create',
    name: 'product-create',
    component: ProductCreateView
  },
  {
    path: '/catalog/product/:id',
    name: 'product-detail',
    component: ProductDetailView,
    props: true
  }
];

export const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes
});
