<template>
  <n-config-provider :theme="darkTheme">
    <n-modal-provider>
      <n-layout style="height: 100vh;">
        <n-layout-header bordered>
          <AppHeader />
        </n-layout-header>
        <n-layout-content content-style="padding: 24px;">
          <RouterView />
        </n-layout-content>
      </n-layout>
      <GlobalPopoutContainer />
    </n-modal-provider>
  </n-config-provider>
</template>

<script setup lang="ts">
import { darkTheme, NConfigProvider, NModalProvider, NLayout, NLayoutHeader, NLayoutContent } from 'naive-ui';
import { RouterView } from 'vue-router';
import AppHeader from '@/components/AppHeader.vue';
import GlobalPopoutContainer from '@/components/GlobalPopoutContainer.vue';
</script>
