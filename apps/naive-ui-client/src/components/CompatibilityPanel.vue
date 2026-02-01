<template>
  <n-card :bordered="true" :type="buildStore.isValid ? 'success' : 'error'">
    <template #header>
      <n-flex align="center" :size="8">
        <n-icon :component="buildStore.isValid ? Icons.CheckCircle : Icons.Warning" />
        <span>Compatibility Status</span>
      </n-flex>
    </template>

    <div v-if="buildStore.isValid">
      <n-flex align="center" :size="8" style="color: var(--n-color-success);">
        <n-icon :component="Icons.CheckCircle" />
        <p style="margin: 0; font-weight: 500;">All components are compatible</p>
      </n-flex>
    </div>

    <n-flex v-else vertical :size="12">
      <div v-if="buildStore.errors.length > 0">
        <n-flex align="center" :size="8" style="margin-bottom: 8px; color: var(--n-color-error);">
          <n-icon :component="Icons.Alert" />
          <h4 style="margin: 0; font-weight: 600; font-size: 14px;">Errors</h4>
        </n-flex>
        <n-flex vertical :size="8">
          <n-alert 
            v-for="(issue, index) in buildStore.errors"
            :key="`error-${index}`"
            type="error"
          >
            {{ issue.message }}
          </n-alert>
        </n-flex>
      </div>

      <div v-if="buildStore.warnings.length > 0">
        <n-flex align="center" :size="8" style="margin-bottom: 8px; color: var(--n-color-warning);">
          <n-icon :component="Icons.Warning" />
          <h4 style="margin: 0; font-weight: 600; font-size: 14px;">Warnings</h4>
        </n-flex>
        <n-flex vertical :size="8">
          <n-alert 
            v-for="(issue, index) in buildStore.warnings"
            :key="`warning-${index}`"
            type="warning"
          >
            {{ issue.message }}
          </n-alert>
        </n-flex>
      </div>
    </n-flex>
  </n-card>
</template>

<script setup lang="ts">
import { NCard, NFlex, NAlert, NIcon } from 'naive-ui';
import { useBuildStore } from '@/stores/buildStore';
import { Icons } from '@/utils/icons';

const buildStore = useBuildStore();
</script>