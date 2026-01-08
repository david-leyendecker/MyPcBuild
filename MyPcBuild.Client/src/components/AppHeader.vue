<template>
  <v-app-bar elevation="1" style="background-color: var(--v-theme-surface)">
    <v-container style="max-width: 1200px">
      <div class="d-flex justify-space-between align-center">
        <router-link to="/" class="text-decoration-none">
          <h1 class="text-h4 text-primary font-weight-bold">MyPCBuild</h1>
        </router-link>
        
        <nav class="d-flex ga-6 align-center">
          <router-link to="/" class="nav-link text-decoration-none font-weight-medium">My Builds</router-link>
          <router-link to="/catalog" class="nav-link text-decoration-none font-weight-medium">Catalog</router-link>
        </nav>
      </div>
    </v-container>
  </v-app-bar>
</template>

<script setup lang="ts">
import { RouterLink } from 'vue-router';
</script>

<style scoped>
.nav-link {
  color: rgb(var(--v-theme-on-surface));
  transition: color 0.3s ease;
}

.nav-link:hover {
  color: rgb(var(--v-theme-primary));
}

.nav-link.router-link-active {
  color: rgb(var(--v-theme-primary));
  border-bottom: 2px solid rgb(var(--v-theme-primary));
  padding-bottom: 0.25rem;
}
</style>
