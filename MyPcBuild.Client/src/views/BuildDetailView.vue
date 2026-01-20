<template>
  <div class="fade-in" style="max-width: 100%;">
    <!-- Loading State -->
    <n-flex v-if="buildStore.isLoading" justify="center" align="center" style="min-height: 50vh;">
      <n-spin size="large" />
    </n-flex>

    <!-- Error State -->
    <n-alert v-else-if="buildStore.error" type="error">
      {{ buildStore.error }}
    </n-alert>

    <!-- Main Content -->
    <template v-else-if="buildStore.currentBuild">
      <!-- Header Section -->
      <n-flex justify="space-between" style="margin-bottom: 16px;">
        <div>
          <h2 style="font-size: 28px; font-weight: 600; margin-bottom: 8px;">{{ buildStore.currentBuild.name }}</h2>
          <p style="opacity: 0.7; font-size: 14px;">
            Created: {{ new Date(buildStore.currentBuild.createdAt).toLocaleDateString() }}
          </p>
        </div>
        <n-button text @click="$router.back()">
          ← Back
        </n-button>
      </n-flex>

      <!-- Compatibility Status Section -->
      <div style="margin-bottom: 16px;">
        <CompatibilityPanel />
      </div>

      <!-- 3D Visualization Section -->
      <n-card v-if="hasSpatialParts" style="margin-bottom: 16px;">
        <template #header>
          <n-flex justify="space-between" align="center">
            <span>3D Build Visualization</span>
            <n-button text size="small" @click="open3DInPopout">
              Open in Popout
            </n-button>
          </n-flex>
        </template>
        <Viewer3D 
          :parts="buildStore.currentBuild.parts"
          :collisions="collidingPartIds"
        />
      </n-card>

      <!-- Parts List Section -->
      <n-card>
        <template #header>PC Components</template>
        
        <n-empty v-if="buildStore.currentBuild.parts.length === 0" description="No components added yet">
          <template #extra>
            <n-button type="primary" @click="showAddPartDialog = true">
              <template #icon>
                <n-icon :component="Icons.Add" />
              </template>
              Add Component
            </n-button>
          </template>
        </n-empty>

        <template v-else>
          <n-flex vertical :size="12">
            <n-card 
              v-for="part in buildStore.currentBuild.parts"
              :key="part.id"
              size="small"
              :bordered="true"
            >
              <n-flex justify="space-between" align="center">
                <div style="flex: 1;">
                  <h4 style="font-size: 18px; margin-bottom: 4px;">{{ part.name }}</h4>
                  <p style="font-size: 14px; margin: 4px 0; opacity: 0.8;">{{ part.category }}</p>
                  <p style="font-weight: 600; margin-top: 8px; color: #18a058;">${{ part.pricePaid.toFixed(2) }}</p>
                </div>
                <n-button text type="error" @click="removePart(part.id)">
                  <template #icon>
                    <n-icon :component="Icons.Trash" />
                  </template>
                </n-button>
              </n-flex>
            </n-card>
          </n-flex>

          <n-divider />

          <p style="font-size: 18px; font-weight: 600; padding-top: 12px;">
            Total Cost: ${{ totalCost.toFixed(2) }}
          </p>
        </template>

        <template #footer>
          <n-button type="primary" block @click="showAddPartDialog = true">
            <template #icon>
              <n-icon :component="Icons.Add" />
            </template>
            Add Component
          </n-button>
        </template>
      </n-card>

      <!-- Add Part Dialog -->
      <n-modal 
        v-model:show="showAddPartDialog"
        preset="card"
        title="Add Component"
        style="width: 600px;"
      >
        <AddPartDialogWithSlots 
          :build-id="buildStore.currentBuild.id"
          @part-selected="handleAddPart"
          @part-selected-with-slot="handleAddPartToSlot"
          @close="showAddPartDialog = false"
        />
      </n-modal>
    </template>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { NCard, NButton, NFlex, NSpin, NAlert, NDivider, NModal, NIcon, NEmpty } from 'naive-ui';
import { useBuildStore } from '@/stores/buildStore';
import { useCatalogStore } from '@/stores/catalogStore';
import { use3DPopout } from '@/composables/use3DPopout';
import CompatibilityPanel from '@/components/CompatibilityPanel.vue';
import AddPartDialogWithSlots from '@/components/AddPartDialogWithSlots.vue';
import Viewer3D from '@/components/Viewer3D.vue';
import { Icons } from '@/utils/icons';

interface Props {
  id: string;
}

withDefaults(defineProps<Props>(), {});

const route = useRoute();
const buildStore = useBuildStore();
const catalogStore = useCatalogStore();
const showAddPartDialog = ref(false);
const { openPopout } = use3DPopout();

const totalCost = computed(() => {
  return buildStore.currentBuild?.parts.reduce((sum, part) => sum + part.pricePaid, 0) ?? 0;
});

const hasSpatialParts = computed(() => {
  return buildStore.currentBuild?.parts.some(p => p.dimensions) ?? false;
});

const collidingPartIds = computed(() => {
  // Extract part IDs from collision-related compatibility issues
  const collisionIssues = buildStore.validationIssues.filter(
    issue => issue.category.toLowerCase().includes('collision') || 
             issue.message.toLowerCase().includes('collision')
  );
  
  const partIds: string[] = [];
  
  // Try to extract part IDs from issue messages
  // Messages typically contain part names like "Collision detected between 'Part A' and 'Part B'"
  collisionIssues.forEach(issue => {
    // For now, mark all parts as potentially colliding if there are any collision issues
    // A more sophisticated implementation would parse the message to extract specific part IDs
    if (buildStore.currentBuild?.parts) {
      buildStore.currentBuild.parts.forEach(part => {
        if (issue.message.includes(part.name) && !partIds.includes(part.id)) {
          partIds.push(part.id);
        }
      });
    }
  });
  
  return partIds;
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

async function handleAddPartToSlot(productId: string, slotId: string, position: { x: number; y: number; z: number }, rotation?: { x: number; y: number; z: number } | null) {
  if (!buildStore.currentBuild) return;
  
  try {
    // Get product to fetch the actual price
    const product = catalogStore.products.find(p => p.id === productId);
    const pricePaid = product?.price ?? 0;
    
    await buildStore.addPartToSlot(buildStore.currentBuild.id, {
      productId,
      pricePaid,
      slotId,
      position,
      rotation: rotation || undefined
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

function open3DInPopout() {
  if (!buildStore.currentBuild) return;
  
  openPopout({
    component: Viewer3D,
    props: {
      parts: buildStore.currentBuild.parts,
      collisions: collidingPartIds.value,
    },
    title: `3D Preview - ${buildStore.currentBuild.name}`,
  });
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
