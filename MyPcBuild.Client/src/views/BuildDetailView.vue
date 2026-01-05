<template>
  <div class="fadein animation-duration-300">
    <div v-if="buildStore.isLoading" class="flex justify-content-center py-8">
      <ProgressSpinner />
    </div>

    <div v-else-if="buildStore.error" class="mb-3">
      <Message severity="error" :text="buildStore.error" />
    </div>

    <div v-else-if="buildStore.currentBuild" class="flex flex-column gap-4">
      <!-- Header -->
      <div class="flex justify-content-between align-items-start">
        <div>
          <h2 class="m-0 text-primary" style="text-shadow: 0 0 20px rgba(0, 212, 255, 0.3);">{{ buildStore.currentBuild.name }}</h2>
          <p class="mt-2 mb-0 text-500 text-sm">
            Created: {{ new Date(buildStore.currentBuild.createdAt).toLocaleDateString() }}
          </p>
        </div>
        <Button 
          icon="pi pi-arrow-left"
          label="Back"
          @click="$router.back()"
          severity="secondary"
        />
      </div>

      <!-- Compatibility Status -->
      <CompatibilityPanel />

      <!-- Parts List -->
      <Card>
        <template #header>
          <div class="p-3">
            <h3 class="m-0">PC Components</h3>
          </div>
        </template>
        <template #content>
          <div v-if="buildStore.currentBuild.parts.length === 0" class="text-center py-6">
            <p class="text-500 mb-4">No components added yet.</p>
            <Button 
              label="Add Component"
              icon="pi pi-plus"
              @click="showAddPartDialog = true"
            />
          </div>

          <div v-else class="flex flex-column gap-3">
            <div 
              v-for="part in buildStore.currentBuild.parts"
              :key="part.productId"
              class="flex justify-content-between align-items-center p-3 border-round surface-border"
              style="border: 1px solid var(--surface-border); background: rgba(255, 255, 255, 0.02);"
            >
              <div>
                <h4 class="mt-0 mb-1">{{ part.productName }}</h4>
                <p class="my-1 text-primary text-sm">{{ part.category }}</p>
                <p class="mt-2 mb-0 text-500 font-medium">${{ part.price.toFixed(2) }}</p>
              </div>
              <Button 
                icon="pi pi-trash"
                size="small"
                severity="danger"
                text
                @click="removePart(part.productId)"
              />
            </div>

            <div class="pt-3 border-top-1 surface-border">
              <p class="m-0 text-lg"><strong>Total Cost:</strong> ${{ totalCost.toFixed(2) }}</p>
            </div>
          </div>
        </template>
        <template #footer>
          <Button 
            label="Add Component"
            icon="pi pi-plus"
            @click="showAddPartDialog = true"
            class="w-full"
          />
        </template>
      </Card>

      <!-- Add Part Dialog -->
      <Dialog 
        v-model:visible="showAddPartDialog"
        header="Add Component"
        modal
      >
        <AddPartDialog 
          @part-selected="handleAddPart"
          @close="showAddPartDialog = false"
        />
      </Dialog>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { useBuildStore } from '@/stores/buildStore';
import Button from 'primevue/button';
import Card from 'primevue/card';
import Dialog from 'primevue/dialog';
import Message from 'primevue/message';
import ProgressSpinner from 'primevue/progressspinner';
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
  return buildStore.currentBuild?.parts.reduce((sum, part) => sum + part.price, 0) ?? 0;
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
