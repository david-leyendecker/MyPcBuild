export interface NavigationItem {
  title: string;
  icon: string;
  path: string;
}

// Individual navigation items
export const MY_BUILDS: NavigationItem = {
  title: 'My Builds',
  icon: 'mdi-hammer-wrench',
  path: '/'
} as const;

export const PRODUCT_CATALOG: NavigationItem = {
  title: 'Product Catalog',
  icon: 'mdi-package-variant',
  path: '/catalog'
} as const;

// Array for menu rendering
export const NAVIGATION_ITEMS: readonly NavigationItem[] = [
  MY_BUILDS,
  PRODUCT_CATALOG
] as const;

// Helper to get navigation item by path
export function getNavigationItem(path: string): NavigationItem | undefined {
  return NAVIGATION_ITEMS.find(item => item.path === path);
}
