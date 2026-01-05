<template>
  <div class="flex flex-column min-h-screen">
    <AppHeader />
    <main class="flex-grow-1 p-4 md:p-5 mx-auto w-full" style="max-width: 1200px">
      <RouterView />
    </main>
  </div>
</template>

<script setup lang="ts">
import { RouterView } from 'vue-router';
import AppHeader from '@/components/AppHeader.vue';
</script>
