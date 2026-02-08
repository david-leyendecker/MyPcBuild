import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'dashboard',
      component: () => import('@/views/DashboardView.vue'),
    },
    {
      path: '/builds',
      name: 'builds',
      component: () => import('@/views/builds/BuildsListView.vue'),
    },
    {
      path: '/builds/:id',
      name: 'build-detail',
      component: () => import('@/views/builds/BuildDetailView.vue'),
      props: true,
    },
    {
      path: '/catalog',
      name: 'catalog',
      component: () => import('@/views/catalog/CatalogView.vue'),
    },
    {
      path: '/catalog/new',
      name: 'product-create',
      component: () => import('@/views/catalog/ProductCreateView.vue'),
    },
    {
      path: '/catalog/:id',
      name: 'product-detail',
      component: () => import('@/views/catalog/ProductDetailView.vue'),
      props: true,
    },
    {
      path: '/:pathMatch(.*)*',
      name: 'not-found',
      component: () => import('@/views/NotFoundView.vue'),
    },
  ],
})

export default router
