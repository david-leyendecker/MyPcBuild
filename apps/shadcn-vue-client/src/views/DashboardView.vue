<script setup lang="ts">
import { onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useBuildStore } from '@/stores/buildStore'
import { Plus, Package } from 'lucide-vue-next'
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import PageHeader from '@/components/shared/PageHeader.vue'
import LoadingState from '@/components/shared/LoadingState.vue'
import ErrorState from '@/components/shared/ErrorState.vue'
import EmptyState from '@/components/shared/EmptyState.vue'
import BuildCard from '@/components/builds/BuildCard.vue'

const router = useRouter()
const buildStore = useBuildStore()

const recentBuilds = computed(() => buildStore.builds.slice(0, 3))
const totalBuilds = computed(() => buildStore.builds.length)
const totalParts = computed(() => 
  buildStore.builds.reduce((sum, build) => sum + (build.parts?.length || 0), 0)
)
const totalInvestment = computed(() =>
  buildStore.builds.reduce((sum, build) => 
    sum + (build.parts?.reduce((pSum, part) => pSum + (part.pricePaid || 0), 0) || 0), 0
  )
)

onMounted(async () => {
  await buildStore.loadBuilds()
})

function createNewBuild() {
  router.push('/builds')
}

function browseCatalog() {
  router.push('/catalog')
}
</script>

<template>
  <div class="container mx-auto p-6">
    <PageHeader
      title="Dashboard"
      description="Overview of your PC builds and quick actions"
    />

    <LoadingState v-if="buildStore.isLoading && buildStore.builds.length === 0" />
    <ErrorState v-else-if="buildStore.error" :message="buildStore.error" />
    
    <div v-else class="space-y-6">
      <!-- Stats Section -->
      <div class="grid gap-4 md:grid-cols-3">
        <Card>
          <CardHeader>
            <CardTitle class="text-sm font-medium text-muted-foreground">
              Total Builds
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div class="text-3xl font-bold">{{ totalBuilds }}</div>
          </CardContent>
        </Card>
        
        <Card>
          <CardHeader>
            <CardTitle class="text-sm font-medium text-muted-foreground">
              Total Parts
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div class="text-3xl font-bold">{{ totalParts }}</div>
          </CardContent>
        </Card>
        
        <Card>
          <CardHeader>
            <CardTitle class="text-sm font-medium text-muted-foreground">
              Total Investment
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div class="text-3xl font-bold">${{ totalInvestment.toFixed(2) }}</div>
          </CardContent>
        </Card>
      </div>

      <!-- Quick Actions Section -->
      <Card>
        <CardHeader>
          <CardTitle>Quick Actions</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="flex gap-4">
            <Button @click="createNewBuild">
              <Plus class="h-4 w-4 mr-2" />
              New Build
            </Button>
            <Button variant="outline" @click="browseCatalog">
              <Package class="h-4 w-4 mr-2" />
              Browse Catalog
            </Button>
          </div>
        </CardContent>
      </Card>

      <!-- Recent Builds Section -->
      <div>
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-2xl font-bold">Recent Builds</h2>
          <Button variant="ghost" @click="router.push('/builds')">
            View All
          </Button>
        </div>

        <EmptyState
          v-if="buildStore.builds.length === 0"
          title="No builds yet"
          description="Create your first PC build to get started"
        >
          <Button @click="createNewBuild">
            <Plus class="h-4 w-4 mr-2" />
            Create Your First Build
          </Button>
        </EmptyState>

        <div v-else class="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
          <BuildCard
            v-for="build in recentBuilds"
            :key="build.id"
            :build="build"
          />
        </div>
      </div>
    </div>
  </div>
</template>
