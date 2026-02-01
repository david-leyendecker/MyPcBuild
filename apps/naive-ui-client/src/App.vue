<template>
  <n-config-provider :theme="isDark ? darkTheme : null">
    <n-message-provider>
      <n-notification-provider>
        <n-dialog-provider>
          <n-modal-provider>
            <n-layout position="absolute" style="top: 0; bottom: 0;">
              <n-layout-header bordered style="height: 64px; padding: 0;">
                <AppHeader />
              </n-layout-header>
              <n-layout position="absolute" style="top: 64px; bottom: 0;" has-sider>
                <n-layout-sider
                  bordered
                  collapse-mode="width"
                  :collapsed-width="0"
                  :width="280"
                  :collapsed="collapsed"
                  show-trigger
                  @collapse="collapsed = true"
                  @expand="collapsed = false"
                >
                  <AppSidebar />
                </n-layout-sider>
                <n-layout-content content-style="padding: 24px;">
                  <RouterView />
                </n-layout-content>
              </n-layout>
            </n-layout>
            <GlobalPopoutContainer />
          </n-modal-provider>
        </n-dialog-provider>
      </n-notification-provider>
    </n-message-provider>
  </n-config-provider>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { 
  darkTheme, 
  NConfigProvider, 
  NMessageProvider,
  NNotificationProvider,
  NDialogProvider,
  NModalProvider, 
  NLayout, 
  NLayoutHeader, 
  NLayoutSider,
  NLayoutContent 
} from 'naive-ui';
import { RouterView } from 'vue-router';
import { useThemeStore } from '@/stores/themeStore';
import AppHeader from '@/components/AppHeader.vue';
import AppSidebar from '@/components/AppSidebar.vue';
import GlobalPopoutContainer from '@/components/GlobalPopoutContainer.vue';

const themeStore = useThemeStore();
const isDark = computed(() => themeStore.isDark);
const collapsed = ref(false);
</script>
