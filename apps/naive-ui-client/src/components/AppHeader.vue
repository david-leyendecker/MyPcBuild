<template>
  <n-space justify="space-between" align="center" style="padding: 0 24px; height: 64px;">
    <n-space align="center">
      <n-icon size="24" :component="Icons.Cube" />
      <h2 style="margin: 0; font-size: 20px; font-weight: 600;">MyPCBuild</h2>
    </n-space>
    <n-space align="center">
      <n-button text @click="themeStore.toggleTheme()" :title="isDark ? 'Switch to Light Mode' : 'Switch to Dark Mode'">
        <template #icon>
          <n-icon size="20" :component="isDark ? Icons.Sun : Icons.Moon" />
        </template>
      </n-button>
    </n-space>
  </n-space>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { NSpace, NButton, NIcon } from 'naive-ui';
import { useThemeStore } from '@/stores/themeStore';
import { Icons } from '@/utils/icons';

const themeStore = useThemeStore();
const isDark = computed(() => themeStore.isDark);
</script>

<style scoped>
/* No custom nav-link styles needed */
</style>
