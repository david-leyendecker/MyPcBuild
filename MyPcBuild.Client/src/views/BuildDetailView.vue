<template>
  <v-container class="fade-in" fluid>
    <!-- Loading State -->
    <v-row v-if="buildStore.isLoading" justify="center" align="center" style="min-height: 50vh">
      <v-col cols="12" class="text-center">
        <v-progress-circular indeterminate color="primary"></v-progress-circular>
      </v-col>
    </v-row>

    <!-- Error State -->
    <v-row v-else-if="buildStore.error">
      <v-col cols="12">
        <v-alert type="error">
          {{ buildStore.error }}
        </v-alert>
      </v-col>
    </v-row>

    <!-- Main Content -->
    <template v-else-if="buildStore.currentBuild">
      <!-- Header Section -->
      <v-row justify="space-between">
        <v-col cols="10">
          <h2 class="text-h4 text-primary">{{ buildStore.currentBuild.name }}</h2>
          <p class="text-medium-emphasis text-body-2 mt-2">
            Created: {{ new Date(buildStore.currentBuild.createdAt).toLocaleDateString() }}
          </p>
        </v-col>
        <v-col cols="1" class="text-right">
          <v-btn 
            prepend-icon="mdi-arrow-left"
            variant="text"
            @click="$router.back()"
          >
            Back
          </v-btn>
        </v-col>
      </v-row>

      <!-- Compatibility Status Section -->
      <v-row>
        <v-col cols="12">
          <CompatibilityPanel />
        </v-col>
      </v-row>

      <!-- 3D Visualization Section -->
      <v-row v-if="hasSpatialParts">
        <v-col cols="12">
          <v-card>
            <v-card-title>3D Build Visualization</v-card-title>
            <v-card-text>
              <Viewer3D 
                :parts="buildStore.currentBuild.parts"
                :collisions="collidingPartIds"
              />
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>

      <!-- Parts List Section -->
      <v-row>
        <v-col cols="12">
          <v-card>
            <v-card-title>PC Components</v-card-title>
            <v-card-text>
              <v-row v-if="buildStore.currentBuild.parts.length === 0" justify="center">
                <v-col cols="12" class="text-center py-6">
                  <p class="text-medium-emphasis mb-4">No components added yet.</p>
                  <v-btn 
                    prepend-icon="mdi-plus"
                    color="primary"
                    @click="showAddPartDialog = true"
                  >
                    Add Component
                  </v-btn>
                </v-col>
              </v-row>

              <v-row v-else>
                <v-col cols="12">
                  <v-row>
                    <v-col 
                      v-for="part in buildStore.currentBuild.parts"
                      :key="part.id"
                      cols="12"
                    >
                      <v-card variant="outlined">
                        <v-card-text>
                          <v-row justify="space-between">
                            <v-col cols="10">
                              <h4 class="text-h6 mb-1">{{ part.name }}</h4>
                              <p class="text-primary text-body-2 my-1">{{ part.category }}</p>
                              <p class="text-medium-emphasis font-weight-medium mt-2">${{ part.pricePaid.toFixed(2) }}</p>
                            </v-col>
                            <v-col cols="1" class="text-right">
                              <v-btn 
                                icon="mdi-delete"
                                size="small"
                                color="error"
                                variant="text"
                                @click="removePart(part.id)"
                              ></v-btn>
                            </v-col>
                          </v-row>
                        </v-card-text>
                      </v-card>
                    </v-col>
                  </v-row>

                  <v-divider class="my-4"></v-divider>

                  <v-row class="pt-3">
                    <v-col cols="12">
                      <p class="text-h6"><strong>Total Cost:</strong> ${{ totalCost.toFixed(2) }}</p>
                    </v-col>
                  </v-row>
                </v-col>
              </v-row>
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
        </v-col>
      </v-row>

      <!-- Add Part Dialog -->
      <v-dialog 
        v-model="showAddPartDialog"
        max-width="600"
      >
        <v-card>
          <v-card-title>Add Component</v-card-title>
          <v-card-text>
            <AddPartDialogWithSlots 
              :build-id="buildStore.currentBuild.id"
              @part-selected="handleAddPart"
              @part-selected-with-slot="handleAddPartToSlot"
              @close="showAddPartDialog = false"
            />
          </v-card-text>
        </v-card>
      </v-dialog>
    </template>
  </v-container>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { useBuildStore } from '@/stores/buildStore';
import CompatibilityPanel from '@/components/CompatibilityPanel.vue';
import AddPartDialogWithSlots from '@/components/AddPartDialogWithSlots.vue';
import Viewer3D from '@/components/Viewer3D.vue';

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

const hasSpatialParts = computed(() => {
  return buildStore.currentBuild?.parts.some(p => p.dimensions && p.position) ?? false;
});

const collidingPartIds = computed(() => {
  // Extract part IDs that have collision issues from compatibility issues
  // This is a simplified version - in a real implementation, we'd need to parse
  // the collision issue messages to extract the specific part IDs
  return [];
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

async function handleAddPartToSlot(productId: string, slotId: string, position: { x: number; y: number; z: number }) {
  if (!buildStore.currentBuild) return;
  
  try {
    await buildStore.addPartToSlot(buildStore.currentBuild.id, {
      productId,
      pricePaid: 0, // TODO: Get price from product
      slotId,
      position
    });
    showAddPartDialog.value = false;
  } catch (error) {
    console.error('Failed to add part to slot:', error);
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
