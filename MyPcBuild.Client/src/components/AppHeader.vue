<template>
  <n-space justify="space-between" align="center" style="padding: 12px 24px;">
    <n-space align="center">
      <n-button text @click="drawer = true">
        <template #icon>
          <n-icon><Grid /></n-icon>
        </template>
      </n-button>
      <h2 style="margin: 0; font-size: 20px; font-weight: 600;">MyPCBuild</h2>
    </n-space>
  </n-space>

  <n-drawer v-model:show="drawer" :width="280" placement="left">
    <n-drawer-content title="Navigation">
      <n-menu
        :options="menuOptions"
        @update:value="handleMenuSelect"
      />
    </n-drawer-content>
  </n-drawer>
</template>

<script setup lang="ts">
import { ref, h } from 'vue';
import type { Component } from 'vue';
import { useRouter } from 'vue-router';
import { NSpace, NButton, NDrawer, NDrawerContent, NMenu, NIcon } from 'naive-ui';
import type { MenuOption } from 'naive-ui';
import { GridOutline as Grid } from '@vicons/ionicons5';
import { NAVIGATION_ITEMS } from '@/config/navigation';
import { Icons } from '@/utils/icons';

const router = useRouter();
const drawer = ref(false);

const iconMap: Record<string, Component> = {
  'mdi-hammer-wrench': Icons.Hammer,
  'mdi-package-variant': Icons.Cube
};

const menuOptions: MenuOption[] = NAVIGATION_ITEMS.map(item => ({
  label: item.title,
  key: item.path,
  icon: () => h(NIcon, null, { default: () => h(iconMap[item.icon] || Icons.Info) })
}));

function handleMenuSelect(key: string) {
  router.push(key);
  drawer.value = false;
}
</script>

<style scoped>
/* No custom nav-link styles needed */
</style>
