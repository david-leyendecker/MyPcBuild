<template>
  <Card :class="{ 'border-green-500': buildStore.isValid, 'border-red-500': !buildStore.isValid }">
    <template #header>
      <div class="p-3 flex align-items-center gap-2">
        <i :class="`pi ${buildStore.isValid ? 'pi-check-circle text-green-500' : 'pi-exclamation-circle text-red-500'} text-xl`"></i>
        <h3 class="m-0">Compatibility Status</h3>
      </div>
    </template>
    <template #content>
      <div v-if="buildStore.isValid" class="text-green-500 font-medium">
        <p class="m-0">✓ All components are compatible</p>
      </div>

      <div v-else class="flex flex-column gap-3">
        <div v-if="buildStore.errors.length > 0">
          <h4 class="mt-0 mb-2 text-sm text-red-500">⚠️ Errors</h4>
          <div 
            v-for="(issue, index) in buildStore.errors"
            :key="`error-${index}`"
            class="p-3 border-round border-left-3 border-red-500 text-red-400 text-sm"
            style="background: rgba(255, 0, 0, 0.1);"
          >
            {{ issue.message }}
          </div>
        </div>

        <div v-if="buildStore.warnings.length > 0">
          <h4 class="mt-0 mb-2 text-sm text-orange-500">⚡ Warnings</h4>
          <div 
            v-for="(issue, index) in buildStore.warnings"
            :key="`warning-${index}`"
            class="p-3 border-round border-left-3 border-orange-500 text-orange-400 text-sm"
            style="background: rgba(255, 170, 0, 0.1);"
          >
            {{ issue.message }}
          </div>
        </div>
      </div>
    </template>
  </Card>
</template>

<script setup lang="ts">
import { useBuildStore } from '@/stores/buildStore';
import Card from 'primevue/card';

const buildStore = useBuildStore();
</script>

<style scoped>
.border-green-500 {
  border-color: #22c55e !important;
  background: rgba(34, 197, 94, 0.05);
}

.border-red-500 {
  border-color: #ef4444 !important;
  background: rgba(239, 68, 68, 0.05);
}
</style>
