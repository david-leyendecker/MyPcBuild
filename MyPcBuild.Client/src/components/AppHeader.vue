<template>
  <v-app-bar elevation="1" style="background-color: var(--v-theme-surface)">
    
    <v-app-bar-nav-icon @click="drawer = !drawer"></v-app-bar-nav-icon>

    <v-app-bar-title>MyPCBuild</v-app-bar-title>

  </v-app-bar>

  <v-navigation-drawer v-model="drawer" temporary>
    <v-list>
      <v-list-item
        v-for="item in navigationItems"
        :key="item.path"
        :prepend-icon="item.icon"
        :title="item.title"
        :to="item.path"
        @click="drawer = false"
      />
    </v-list>
  </v-navigation-drawer>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { NAVIGATION_ITEMS } from '@/config/navigation';

const drawer = ref(false);
const navigationItems = NAVIGATION_ITEMS;
</script>

<style scoped>
/* No custom nav-link styles needed - using Vuetify navigation drawer */
</style>
