<template>
  <n-menu
    :options="menuOptions"
    :value="activeKey"
    @update:value="handleMenuSelect"
  />
</template>

<script setup lang="ts">
import { h, computed } from 'vue';
import type { Component } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { NMenu, NIcon } from 'naive-ui';
import type { MenuOption } from 'naive-ui';
import { NAVIGATION_ITEMS } from '@/config/navigation';
import { Icons } from '@/utils/icons';

const router = useRouter();
const route = useRoute();

const iconMap: Record<string, Component> = {
  'mdi-hammer-wrench': Icons.Hammer,
  'mdi-package-variant': Icons.Cube
};

const menuOptions: MenuOption[] = NAVIGATION_ITEMS.map(item => ({
  label: item.title,
  key: item.path,
  icon: () => h(NIcon, null, { default: () => h(iconMap[item.icon] || Icons.Info) })
}));

const activeKey = computed(() => {
  const path = route.path;
  // Match the base path
  if (path === '/' || path.startsWith('/builds')) {
    return '/';
  }
  if (path.startsWith('/catalog')) {
    return '/catalog';
  }
  return path;
});

function handleMenuSelect(key: string) {
  router.push(key);
}
</script>
