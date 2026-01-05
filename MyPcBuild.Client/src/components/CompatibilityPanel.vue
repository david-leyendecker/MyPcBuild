<template>
  <Card :class="{ 'p-card': true, 'p-card-success': buildStore.isValid, 'p-card-danger': !buildStore.isValid }">
    <template #header>
      <div class="p-3 flex align-items-center gap-2">
        <i :class="`pi ${buildStore.isValid ? 'pi-check-circle' : 'pi-exclamation-circle'} text-xl`" :style="{ color: buildStore.isValid ? 'var(--green-500)' : 'var(--red-500)' }"></i>
        <h3 class="m-0 p-card-title">Compatibility Status</h3>
      </div>
    </template>
    <template #content>
      <div v-if="buildStore.isValid" class="p-text-success font-medium flex align-items-center gap-2">
        <i class="pi pi-check text-xl" style="color: var(--green-500);"></i>
        <p class="m-0">All components are compatible</p>
      </div>

      <div v-else class="flex flex-column gap-3">
        <div v-if="buildStore.errors.length > 0">
          <h4 class="mt-0 mb-2 text-sm p-text-danger flex align-items-center gap-2"><i class="pi pi-exclamation-triangle" style="color: var(--red-500);"></i>Errors</h4>
          <div 
            v-for="(issue, index) in buildStore.errors"
            :key="`error-${index}`"
            class="p-3 border-round border-left-3 surface-section text-sm"
            style="border-left-color: var(--red-500); color: var(--red-500);"
          >
            {{ issue.message }}
          </div>
        </div>

        <div v-if="buildStore.warnings.length > 0">
          <h4 class="mt-0 mb-2 text-sm p-text-warning flex align-items-center gap-2"><i class="pi pi-bolt" style="color: var(--orange-500);"></i>Warnings</h4>
          <div 
            v-for="(issue, index) in buildStore.warnings"
            :key="`warning-${index}`"
            class="p-3 border-round border-left-3 surface-section text-sm"
            style="border-left-color: var(--orange-500); color: var(--orange-500);"
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