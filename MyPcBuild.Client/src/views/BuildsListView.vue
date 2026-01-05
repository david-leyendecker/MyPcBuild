<template>
  <div class="fadein animation-duration-300">
    <div class="flex justify-content-between align-items-center mb-4">
      <h2 class="m-0 text-primary" style="text-shadow: 0 0 20px rgba(0, 212, 255, 0.3);">My PC Builds</h2>
      <Button 
        icon="pi pi-plus"
        label="New Build"
        @click="showNewBuildDialog = true"
        rounded
        severity="success"
      />
    </div>

    <div v-if="buildStore.isLoading" class="flex justify-content-center py-8">
      <ProgressSpinner />
    </div>

    <div v-else-if="buildStore.error" class="mb-3">
      <Message severity="error" :text="buildStore.error" />
    </div>

    <div v-else-if="buildStore.builds.length === 0" class="text-center py-8">
      <p class="text-xl text-500">No builds yet. Create your first PC build!</p>
    </div>

    <div v-else class="grid">
      <div 
        v-for="build in buildStore.builds" 
        :key="build.id"
        class="col-12 md:col-6 lg:col-4"
      >
        <Card class="build-card">
          <template #content>
            <router-link :to="`/builds/${build.id}`" class="build-link no-underline">
              <h3 class="mt-0 mb-3 text-xl">{{ build.name }}</h3>
            </router-link>
            <div class="text-sm text-400">
              <p class="my-2"><strong>Parts:</strong> {{ build.parts.length }}</p>
              <p class="my-2"><strong>Total Cost:</strong> ${{ build.parts.reduce((sum, p) => sum + p.price, 0).toFixed(2) }}</p>
            </div>
          </template>
          <template #footer>
            <div class="flex gap-2">
              <Button 
                label="View Details"
                icon="pi pi-arrow-right"
                @click="$router.push(`/builds/${build.id}`)"
                size="small"
                class="flex-grow-1"
              />
              <Button 
                icon="pi pi-trash"
                @click="deleteBuild(build.id)"
                size="small"
                severity="danger"
                text
              />
            </div>
          </template>
        </Card>
      </div>
    </div>

    <!-- New Build Dialog -->
    <Dialog 
      v-model:visible="showNewBuildDialog"
      header="Create New Build"
      modal
      @update:visible="onDialogClose"
    >
      <div class="flex flex-column gap-3 py-3">
        <label for="build-name" class="font-medium">Build Name</label>
        <InputText 
          id="build-name"
          v-model="newBuildName"
          placeholder="My Gaming PC"
        />
      </div>
      <template #footer>
        <Button 
          label="Cancel"
          icon="pi pi-times"
          @click="showNewBuildDialog = false"
          text
        />
        <Button 
          label="Create"
          icon="pi pi-check"
          @click="handleCreateBuild"
          :loading="buildStore.isLoading"
        />
      </template>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useBuildStore } from '@/stores/buildStore';
import Button from 'primevue/button';
import Card from 'primevue/card';
import Dialog from 'primevue/dialog';
import InputText from 'primevue/inputtext';
import Message from 'primevue/message';
import ProgressSpinner from 'primevue/progressspinner';

const router = useRouter();
const buildStore = useBuildStore();

const showNewBuildDialog = ref(false);
const newBuildName = ref('');

onMounted(() => {
  buildStore.loadBuilds();
});

function onDialogClose() {
  if (!showNewBuildDialog.value) {
    newBuildName.value = '';
  }
}

async function handleCreateBuild() {
  if (!newBuildName.value.trim()) {
    return;
  }
  
  try {
    const build = await buildStore.createBuild(newBuildName.value);
    showNewBuildDialog.value = false;
    newBuildName.value = '';
    await router.push(`/builds/${build.id}`);
  } catch (error) {
    console.error('Failed to create build:', error);
  }
}

async function deleteBuild(id: string) {
  // TODO: Implement delete functionality
  console.log('Delete build:', id);
}
</script>

<style scoped>
.build-card {
  transition: all 0.3s ease;
}

.build-card:hover {
  border-color: var(--primary-color);
  box-shadow: 0 0 20px rgba(0, 212, 255, 0.1);
}

.build-link {
  color: var(--primary-color);
  transition: color 0.3s ease;
}

.build-link:hover {
  color: #00ffff;
}
</style>
