<template>
  <div class="fade-in">
    <ViewHeader
      :title="MY_BUILDS.title"
      :action-button="{
        text: 'New Build',
        icon: 'mdi-plus',
        onClick: () => showNewBuildDialog = true
      }"
    />

    <div v-if="buildStore.isLoading" class="d-flex justify-center py-8">
      <v-progress-circular indeterminate color="primary"></v-progress-circular>
    </div>

    <v-alert v-else-if="buildStore.error" type="error" class="mb-3">
      {{ buildStore.error }}
    </v-alert>

    <div v-else-if="buildStore.builds.length === 0" class="text-center py-8">
      <p class="text-h6 text-medium-emphasis">No builds yet. Create your first PC build!</p>
    </div>

    <v-row v-else>
      <v-col 
        v-for="build in buildStore.builds" 
        :key="build.id"
        cols="12" md="6" lg="4"
      >
        <v-card class="build-card">
          <v-card-text>
            <router-link :to="`/builds/${build.id}`" class="build-link text-decoration-none">
              <h3 class="text-h5 mb-3">{{ build.name }}</h3>
            </router-link>
            <div class="text-body-2 text-medium-emphasis">
              <p class="my-2"><strong>Parts:</strong> {{ build.parts.length }}</p>
              <p class="my-2"><strong>Total Cost:</strong> ${{ build.parts.reduce((sum, p) => sum + p.pricePaid, 0).toFixed(2) }}</p>
            </div>
          </v-card-text>
          <v-card-actions>
            <v-btn 
              prepend-icon="mdi-arrow-right"
              size="small"
              class="flex-grow-1"
              @click="$router.push(`/builds/${build.id}`)"
            >
              View Details
            </v-btn>
            <v-btn 
              icon="mdi-delete"
              size="small"
              color="error"
              variant="text"
              @click="deleteBuild(build.id)"
            ></v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>

    <!-- New Build Dialog -->
    <v-dialog 
      v-model="showNewBuildDialog"
      max-width="500"
    >
      <v-card>
        <v-card-title>Create New Build</v-card-title>
        <v-card-text>
          <v-text-field 
            v-model="newBuildName"
            label="Build Name"
            placeholder="My Gaming PC"
          ></v-text-field>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn 
            prepend-icon="mdi-close"
            variant="text"
            @click="showNewBuildDialog = false"
          >
            Cancel
          </v-btn>
          <v-btn 
            prepend-icon="mdi-check"
            color="primary"
            :loading="buildStore.isLoading"
            @click="handleCreateBuild"
          >
            Create
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useBuildStore } from '@/stores/buildStore';
import ViewHeader from '@/components/ViewHeader.vue';
import { MY_BUILDS } from '@/config/navigation';

const router = useRouter();
const buildStore = useBuildStore();

const showNewBuildDialog = ref(false);
const newBuildName = ref('');

onMounted(() => {
  buildStore.loadBuilds();
});

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
.fade-in {
  animation: fadeIn 0.3s ease-in;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

.build-card {
  transition: all 0.3s ease;
}

.build-card:hover {
  border-color: rgb(var(--v-theme-primary));
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
  transform: translateY(-2px);
}

.build-link {
  color: rgb(var(--v-theme-primary));
  transition: color 0.3s ease;
}

.build-link:hover {
  opacity: 0.8;
}
</style>
