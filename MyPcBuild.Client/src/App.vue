<template>
  <v-app>
    <AppHeader />
    <v-main>
      <v-container>
        <RouterView />
      </v-container>
    </v-main>
    <GlobalPopoutContainer />
  </v-app>
</template>

<script setup lang="ts">
import { RouterView } from 'vue-router';
import AppHeader from '@/components/AppHeader.vue';
import GlobalPopoutContainer from '@/components/GlobalPopoutContainer.vue';
</script>
