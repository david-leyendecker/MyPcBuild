<template>
  <v-app-bar elevation="1" style="background-color: var(--v-theme-surface)">
    <v-container style="max-width: 1200px">
      <div class="d-flex justify-space-between align-center w-100">
        <router-link to="/" class="text-decoration-none">
          <h1 class="text-h4 text-primary font-weight-bold">MyPCBuild</h1>
        </router-link>
        
        <v-app-bar-nav-icon @click="drawer = !drawer"></v-app-bar-nav-icon>
      </div>
    </v-container>
  </v-app-bar>

  <v-navigation-drawer
    v-model="drawer"
    location="right"
    temporary
  >
    <v-list>
      <v-list-item
        prepend-icon="mdi-hammer-wrench"
        title="My Builds"
        to="/"
        @click="drawer = false"
      ></v-list-item>
      
      <v-list-item
        prepend-icon="mdi-package-variant"
        title="Catalog"
        to="/catalog"
        @click="drawer = false"
      ></v-list-item>
    </v-list>
  </v-navigation-drawer>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { RouterLink } from 'vue-router';

const drawer = ref(false);
</script>

<style scoped>
/* No custom nav-link styles needed - using Vuetify navigation drawer */
</style>
