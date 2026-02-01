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

    <n-flex v-if="buildStore.isLoading" justify="center" style="padding: 32px 0;">
      <n-spin size="large" />
    </n-flex>

    <n-alert v-else-if="buildStore.error" type="error" style="margin-bottom: 12px;">
      {{ buildStore.error }}
    </n-alert>

    <n-empty 
      v-else-if="buildStore.builds.length === 0" 
      description="No builds yet. Create your first PC build!"
      style="padding: 64px 0;"
    >
      <template #extra>
        <n-button type="primary" @click="showNewBuildDialog = true">
          <template #icon>
            <n-icon><Add /></n-icon>
          </template>
          Create First Build
        </n-button>
      </template>
    </n-empty>

    <n-grid v-else :cols="1" :x-gap="16" :y-gap="16" responsive="screen" :item-responsive="true">
      <n-gi 
        v-for="build in buildStore.builds" 
        :key="build.id"
        :span="24" :suffix="true"
        :xs="24" :sm="24" :md="12" :lg="8" :xl="8"
      >
        <n-card class="build-card">
          <router-link :to="`/builds/${build.id}`" class="build-link" style="text-decoration: none;">
            <n-h3 style="margin: 0 0 12px 0;">{{ build.name }}</n-h3>
          </router-link>
          <n-flex vertical style="font-size: 14px;">
            <n-text depth="3"><n-text strong>Parts:</n-text> {{ build.parts.length }}</n-text>
            <n-text depth="3"><n-text strong>Total Cost:</n-text> ${{ build.parts.reduce((sum, p) => sum + p.pricePaid, 0).toFixed(2) }}</n-text>
          </n-flex>
          <template #footer>
            <n-flex justify="space-between">
              <n-button 
                type="primary"
                style="flex: 1;"
                @click="$router.push(`/builds/${build.id}`)"
              >
                <template #icon>
                  <n-icon><ArrowForward /></n-icon>
                </template>
                View Details
              </n-button>
              <n-button 
                type="error"
                text
                @click="deleteBuild(build.id)"
              >
                <template #icon>
                  <n-icon><Trash /></n-icon>
                </template>
              </n-button>
            </n-flex>
          </template>
        </n-card>
      </n-gi>
    </n-grid>

    <!-- New Build Dialog -->
    <n-modal 
      v-model:show="showNewBuildDialog"
      preset="card"
      title="Create New Build"
      style="width: 500px;"
    >
      <n-form-item label="Build Name">
        <n-input 
          v-model:value="newBuildName"
          placeholder="My Gaming PC"
        />
      </n-form-item>
      <template #footer>
        <n-flex justify="end" :size="12">
          <n-button 
            @click="showNewBuildDialog = false"
          >
            <template #icon>
              <n-icon><Close /></n-icon>
            </template>
            Cancel
          </n-button>
          <n-button 
            type="primary"
            :loading="buildStore.isLoading"
            @click="handleCreateBuild"
          >
            <template #icon>
              <n-icon><Check /></n-icon>
            </template>
            Create
          </n-button>
        </n-flex>
      </template>
    </n-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { NGrid, NGi, NCard, NButton, NFlex, NSpin, NAlert, NModal, NInput, NEmpty, NIcon, NFormItem, NH3, NText } from 'naive-ui';
import { Add, ArrowForward, TrashOutline as Trash, CheckmarkOutline as Check, Close } from '@vicons/ionicons5';
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
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
  transform: translateY(-2px);
}

.build-link {
  transition: opacity 0.3s ease;
}

.build-link:hover {
  opacity: 0.8;
}
</style>
