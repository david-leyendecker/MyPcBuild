<template>
  <header class="sticky top-0 z-5 px-4 md:px-6 py-3 border-bottom-1 surface-border surface-section">
    <div class="flex justify-content-between align-items-center mx-auto max-w-full" style="max-width: 1200px;">
      <router-link to="/" class="no-underline text-primary">
        <h1 class="m-0 text-2xl md:text-3xl font-bold text-primary">MyPCBuild</h1>
      </router-link>
      
      <nav class="flex gap-4 md:gap-6 align-items-center">
        <router-link to="/" class="nav-link no-underline font-medium transition-colors transition-duration-300">My Builds</router-link>
        <router-link to="/catalog" class="nav-link no-underline font-medium transition-colors transition-duration-300">Catalog</router-link>
      </nav>
    </div>
  </header>
</template>

<script setup lang="ts">
import { RouterLink } from 'vue-router';
</script>

<style scoped>
.nav-link {
  color: var(--text-color);
  transition: color 0.3s ease;
}

.nav-link:hover {
  color: var(--primary-color);
}

.nav-link.router-link-active {
  color: var(--primary-color);
  border-bottom: 2px solid var(--primary-color);
  padding-bottom: 0.25rem;
}
</style>
