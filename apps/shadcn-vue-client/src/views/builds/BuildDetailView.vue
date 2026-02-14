<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useBuildStore } from '@/stores/buildStore'
import { ArrowLeft, Pencil } from 'lucide-vue-next'
import { Button } from '@/components/ui/button'
import { Card, CardContent } from '@/components/ui/card'
import PageHeader from '@/components/shared/PageHeader.vue'
import LoadingState from '@/components/shared/LoadingState.vue'
import ErrorState from '@/components/shared/ErrorState.vue'
import BuildSummaryBar from '@/components/builds/BuildSummaryBar.vue'
import BuildPartsList from '@/components/builds/BuildPartsList.vue'
import CompatibilityPanel from '@/components/compatibility/CompatibilityPanel.vue'
import BuildViewer3D from '@/components/spatial/BuildViewer3D.vue'

interface Props {
  id: string
}

const props = defineProps<Props>()
const route = useRoute()
const router = useRouter()
const buildStore = useBuildStore()

type Tab = 'overview' | 'parts' | 'compatibility' | '3d'
const activeTab = ref<Tab>('overview')

const totalCost = computed(() => 
  buildStore.currentBuild?.parts.reduce((sum, part) => sum + (part.pricePaid || 0), 0) || 0
)

const partCount = computed(() => buildStore.currentBuild?.parts.length || 0)

const hasErrors = computed(() => buildStore.errors.length > 0)
const hasWarnings = computed(() => buildStore.warnings.length > 0)

const hasSpatialParts = computed(() =>
  buildStore.currentBuild?.parts.some(p => p.dimensions) ?? false
)

onMounted(async () => {
  const buildId = props.id || route.params.id as string
  await buildStore.loadBuild(buildId)
})

function goBack() {
  router.push('/builds')
}

async function handleRemovePart(productId: string) {
  if (!buildStore.currentBuild) {
    return
  }
  await buildStore.removePart(buildStore.currentBuild.id, productId)
}

function handleAddPart() {
  router.push('/catalog')
}

const tabs: { value: Tab; label: string }[] = [
  { value: 'overview', label: 'Overview' },
  { value: 'parts', label: 'Parts' },
  { value: 'compatibility', label: 'Compatibility' },
  { value: '3d', label: '3D View' },
]
</script>

<template>
  <div class="container mx-auto p-6">
    <LoadingState v-if="buildStore.isLoading && !buildStore.currentBuild" />
    <ErrorState v-else-if="buildStore.error" :message="buildStore.error" />
    
    <div v-else-if="buildStore.currentBuild" class="space-y-6">
      <div class="flex items-center gap-4">
        <Button variant="ghost" size="sm" @click="goBack">
          <ArrowLeft class="h-4 w-4 mr-2" />
          Back
        </Button>
      </div>

      <PageHeader
        :title="buildStore.currentBuild.name"
        description="Build details and compatibility status"
      >
        <template #actions>
          <Button variant="outline" size="sm">
            <Pencil class="h-4 w-4 mr-2" />
            Edit Name
          </Button>
        </template>
      </PageHeader>

      <BuildSummaryBar
        :total-cost="totalCost"
        :part-count="partCount"
        :has-errors="hasErrors"
        :has-warnings="hasWarnings"
      />


      <!-- Tabs -->
      <div class="border-b">
        <nav class="flex gap-4">
          <button
            v-for="tab in tabs"
            :key="tab.value"
            @click="activeTab = tab.value"
            :class="[
              'px-4 py-2 border-b-2 font-medium transition-colors',
              activeTab === tab.value
                ? 'border-primary text-primary'
                : 'border-transparent text-muted-foreground hover:text-foreground'
            ]"
          >
            {{ tab.label }}
          </button>
        </nav>
      </div>

      <!-- Tab Content -->
      <div>
        <!-- Overview Tab -->
        <div v-if="activeTab === 'overview'" class="space-y-4">
          <div class="grid gap-4 md:grid-cols-2">
            <Card>
              <CardContent class="p-6">
                <h3 class="text-lg font-semibold mb-4">Build Summary</h3>
                <div class="space-y-3">
                  <div class="flex justify-between">
                    <span class="text-muted-foreground">Build ID:</span>
                    <span class="font-mono text-sm">{{ buildStore.currentBuild.id }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-muted-foreground">Created:</span>
                    <span class="text-sm">{{ new Date(buildStore.currentBuild.createdAt).toLocaleDateString() }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-muted-foreground">Parts Count:</span>
                    <span class="font-semibold">{{ partCount }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-muted-foreground">Total Cost:</span>
                    <span class="font-semibold">${{ totalCost.toFixed(2) }}</span>
                  </div>
                </div>
              </CardContent>
            </Card>

            <Card>
              <CardContent class="p-6">
                <h3 class="text-lg font-semibold mb-4">Compatibility Status</h3>
                <div class="space-y-3">
                  <div class="flex justify-between">
                    <span class="text-muted-foreground">Status:</span>
                    <span :class="[
                      'font-semibold',
                      hasErrors ? 'text-destructive' : hasWarnings ? 'text-yellow-500' : 'text-green-500'
                    ]">
                      {{ hasErrors ? 'Has Errors' : hasWarnings ? 'Has Warnings' : 'Valid' }}
                    </span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-muted-foreground">Errors:</span>
                    <span class="font-semibold text-destructive">{{ buildStore.errors.length }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-muted-foreground">Warnings:</span>
                    <span class="font-semibold text-yellow-500">{{ buildStore.warnings.length }}</span>
                  </div>
                </div>
              </CardContent>
            </Card>
          </div>
        </div>

        <!-- Parts Tab -->
        <div v-if="activeTab === 'parts'">
          <BuildPartsList
            :parts="buildStore.currentBuild.parts"
            @remove="handleRemovePart"
            @add-part="handleAddPart"
          />
        </div>

        <!-- Compatibility Tab -->
        <div v-if="activeTab === 'compatibility'">
          <CompatibilityPanel :issues="buildStore.validationIssues" />
        </div>

        <!-- 3D View Tab -->
        <div v-if="activeTab === '3d'" class="h-[600px]">
          <BuildViewer3D
            :parts="buildStore.currentBuild.parts"
            :collisions="[]"
            :title="`3D View - ${buildStore.currentBuild.name}`"
          />
        </div>
      </div>
    </div>
  </div>
</template>
