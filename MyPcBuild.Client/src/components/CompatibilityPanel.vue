<template>
  <v-card :color="buildStore.isValid ? 'success' : 'error'" variant="outlined">
    <v-card-title>
      <div class="d-flex align-center ga-2">
        <v-icon :color="buildStore.isValid ? 'success' : 'error'">
          {{ buildStore.isValid ? 'mdi-check-circle' : 'mdi-alert-circle' }}
        </v-icon>
        <span>Compatibility Status</span>
      </div>
    </v-card-title>
    <v-card-text>
      <div v-if="buildStore.isValid" class="text-success font-weight-medium d-flex align-center ga-2">
        <v-icon color="success">mdi-check</v-icon>
        <p class="ma-0">All components are compatible</p>
      </div>

      <div v-else class="d-flex flex-column ga-3">
        <div v-if="buildStore.errors.length > 0">
          <h4 class="text-subtitle-2 text-error d-flex align-center ga-2 mb-2">
            <v-icon color="error">mdi-alert</v-icon>
            Errors
          </h4>
          <v-alert 
            v-for="(issue, index) in buildStore.errors"
            :key="`error-${index}`"
            type="error"
            density="compact"
            variant="tonal"
            class="mb-2"
          >
            {{ issue.message }}
          </v-alert>
        </div>

        <div v-if="buildStore.warnings.length > 0">
          <h4 class="text-subtitle-2 text-warning d-flex align-center ga-2 mb-2">
            <v-icon color="warning">mdi-alert-outline</v-icon>
            Warnings
          </h4>
          <v-alert 
            v-for="(issue, index) in buildStore.warnings"
            :key="`warning-${index}`"
            type="warning"
            density="compact"
            variant="tonal"
            class="mb-2"
          >
            {{ issue.message }}
          </v-alert>
        </div>
      </div>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
import { useBuildStore } from '@/stores/buildStore';

const buildStore = useBuildStore();
</script>