import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router';
import BuildsListView from '@/views/BuildsListView.vue';
import BuildDetailView from '@/views/BuildDetailView.vue';
import CatalogView from '@/views/CatalogView.vue';

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
  }
];

export const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes
});
