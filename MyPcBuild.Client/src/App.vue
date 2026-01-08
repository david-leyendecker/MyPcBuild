<template>
  <v-app>
    <AppHeader />
    <v-main>
      <v-container style="max-width: 1200px">
        <RouterView />
      </v-container>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
import { RouterView } from 'vue-router';
import AppHeader from '@/components/AppHeader.vue';
</script>
