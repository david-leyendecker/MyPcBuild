import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { type Build, type BuildValidation, type CompatibilityIssue } from '@/api/builds';
import { buildsApi } from '@/api/builds';

export const useBuildStore = defineStore('builds', () => {
  const builds = ref<Build[]>([]);
  const currentBuild = ref<Build | null>(null);
  const validationIssues = ref<CompatibilityIssue[]>([]);
  const isLoading = ref(false);
  const error = ref<string | null>(null);

  const errors = computed(() => validationIssues.value.filter(i => i.severity === 'Error'));
  const warnings = computed(() => validationIssues.value.filter(i => i.severity === 'Warning'));
  const isValid = computed(() => errors.value.length === 0);

  async function loadBuilds() {
    isLoading.value = true;
    error.value = null;
    try {
      builds.value = await buildsApi.getBuilds();
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load builds';
    } finally {
      isLoading.value = false;
    }
  }

  async function loadBuild(id: string) {
    isLoading.value = true;
    error.value = null;
    try {
      currentBuild.value = await buildsApi.getBuild(id);
      await validateBuild(id);
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load build';
    } finally {
      isLoading.value = false;
    }
  }

  async function createBuild(name: string) {
    isLoading.value = true;
    error.value = null;
    try {
      const newBuild = await buildsApi.createBuild(name);
      builds.value.push(newBuild);
      currentBuild.value = newBuild;
      return newBuild;
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to create build';
      throw err;
    } finally {
      isLoading.value = false;
    }
  }

  async function updateBuild(id: string, name: string) {
    isLoading.value = true;
    error.value = null;
    try {
      const updated = await buildsApi.updateBuild(id, name);
      const index = builds.value.findIndex(b => b.id === id);
      if (index !== -1) {
        builds.value[index] = updated;
      }
      if (currentBuild.value?.id === id) {
        currentBuild.value = updated;
      }
      return updated;
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to update build';
      throw err;
    } finally {
      isLoading.value = false;
    }
  }

  async function addPart(buildId: string, productId: string) {
    try {
      const updated = await buildsApi.addPart(buildId, productId);
      if (currentBuild.value?.id === buildId) {
        currentBuild.value = updated;
      }
      const index = builds.value.findIndex(b => b.id === buildId);
      if (index !== -1) {
        builds.value[index] = updated;
      }
      await validateBuild(buildId);
      return updated;
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to add part';
      throw err;
    }
  }

  async function removePart(buildId: string, productId: string) {
    try {
      const updated = await buildsApi.removePart(buildId, productId);
      if (currentBuild.value?.id === buildId) {
        currentBuild.value = updated;
      }
      const index = builds.value.findIndex(b => b.id === buildId);
      if (index !== -1) {
        builds.value[index] = updated;
      }
      await validateBuild(buildId);
      return updated;
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to remove part';
      throw err;
    }
  }

  async function validateBuild(buildId: string) {
    try {
      const validation: BuildValidation = await buildsApi.validateBuild(buildId);
      validationIssues.value = validation.issues;
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to validate build';
    }
  }

  function clearError() {
    error.value = null;
  }

  return {
    builds,
    currentBuild,
    validationIssues,
    errors,
    warnings,
    isValid,
    isLoading,
    error,
    loadBuilds,
    loadBuild,
    createBuild,
    updateBuild,
    addPart,
    removePart,
    validateBuild,
    clearError
  };
});
