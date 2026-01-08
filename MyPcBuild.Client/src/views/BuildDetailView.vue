<template>
  <div class="fade-in">
    <div v-if="buildStore.isLoading" class="d-flex justify-center py-8">
      <v-progress-circular indeterminate color="primary"></v-progress-circular>
    </div>

    <v-alert v-else-if="buildStore.error" type="error" class="mb-3">
      {{ buildStore.error }}
    </v-alert>

    <div v-else-if="buildStore.currentBuild" class="d-flex flex-column ga-4">
      <!-- Header -->
      <div class="d-flex justify-space-between align-start">
        <div>
          <h2 class="text-h4 text-primary">{{ buildStore.currentBuild.name }}</h2>
          <p class="text-medium-emphasis text-body-2 mt-2">
            Created: {{ new Date(buildStore.currentBuild.createdAt).toLocaleDateString() }}
          </p>
        </div>
        <v-btn 
          prepend-icon="mdi-arrow-left"
          variant="text"
          @click="$router.back()"
        >
          Back
        </v-btn>
      </div>

      <!-- Compatibility Status -->
      <CompatibilityPanel />

      <!-- Parts List -->
      <v-card>
        <v-card-title>PC Components</v-card-title>
        <v-card-text>
          <div v-if="buildStore.currentBuild.parts.length === 0" class="text-center py-6">
            <p class="text-medium-emphasis mb-4">No components added yet.</p>
            <v-btn 
              prepend-icon="mdi-plus"
              color="primary"
              @click="showAddPartDialog = true"
            >
              Add Component
            </v-btn>
          </div>

          <div v-else class="d-flex flex-column ga-3">
            <v-card 
              v-for="part in buildStore.currentBuild.parts"
              :key="part.id"
              variant="outlined"
            >
              <v-card-text>
                <div class="d-flex justify-space-between align-center">
                  <div>
                    <h4 class="text-h6 mb-1">{{ part.name }}</h4>
                    <p class="text-primary text-body-2 my-1">{{ part.category }}</p>
                    <p class="text-medium-emphasis font-weight-medium mt-2">${{ part.pricePaid.toFixed(2) }}</p>
                  </div>
                  <v-btn 
                    icon="mdi-delete"
                    size="small"
                    color="error"
                    variant="text"
                    @click="removePart(part.id)"
                  ></v-btn>
                </div>
              </v-card-text>
            </v-card>

            <v-divider></v-divider>

            <div class="pt-3">
              <p class="text-h6"><strong>Total Cost:</strong> ${{ totalCost.toFixed(2) }}</p>
            </div>
          </div>
        </v-card-text>
        <v-card-actions>
          <v-btn 
            prepend-icon="mdi-plus"
            color="primary"
            block
            @click="showAddPartDialog = true"
          >
            Add Component
          </v-btn>
        </v-card-actions>
      </v-card>

      <!-- Add Part Dialog -->
      <v-dialog 
        v-model="showAddPartDialog"
        max-width="600"
      >
        <v-card>
          <v-card-title>Add Component</v-card-title>
          <v-card-text>
            <AddPartDialog 
              @part-selected="handleAddPart"
              @close="showAddPartDialog = false"
            />
          </v-card-text>
        </v-card>
      </v-dialog>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { useBuildStore } from '@/stores/buildStore';
import CompatibilityPanel from '@/components/CompatibilityPanel.vue';
import AddPartDialog from '@/components/AddPartDialog.vue';

interface Props {
  id: string;
}

withDefaults(defineProps<Props>(), {});

const route = useRoute();
const buildStore = useBuildStore();
const showAddPartDialog = ref(false);

const totalCost = computed(() => {
  return buildStore.currentBuild?.parts.reduce((sum, part) => sum + part.pricePaid, 0) ?? 0;
});

onMounted(() => {
  buildStore.loadBuild(route.params.id as string);
});

async function handleAddPart(productId: string) {
  if (!buildStore.currentBuild) return;
  
  try {
    await buildStore.addPart(buildStore.currentBuild.id, productId);
    showAddPartDialog.value = false;
  } catch (error) {
    console.error('Failed to add part:', error);
  }
}

async function removePart(productId: string) {
  if (!buildStore.currentBuild) return;
  
  try {
    await buildStore.removePart(buildStore.currentBuild.id, productId);
  } catch (error) {
    console.error('Failed to remove part:', error);
  }
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
</style>
