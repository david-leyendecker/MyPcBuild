<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useBuildStore } from '@/stores/buildStore'
import { Plus, ArrowUpDown } from 'lucide-vue-next'
import { Button } from '@/components/ui/button'
import PageHeader from '@/components/shared/PageHeader.vue'
import LoadingState from '@/components/shared/LoadingState.vue'
import ErrorState from '@/components/shared/ErrorState.vue'
import EmptyState from '@/components/shared/EmptyState.vue'
import BuildCard from '@/components/builds/BuildCard.vue'
import CreateBuildDialog from '@/components/builds/CreateBuildDialog.vue'

const router = useRouter()
const buildStore = useBuildStore()
const createDialog = ref<InstanceType<typeof CreateBuildDialog> | null>(null)

type SortBy = 'name' | 'date' | 'cost'
const sortBy = ref<SortBy>('date')

const sortedBuilds = computed(() => {
  const builds = [...buildStore.builds]
  
  switch (sortBy.value) {
    case 'name':
      return builds.sort((a, b) => a.name.localeCompare(b.name))
    case 'cost':
      return builds.sort((a, b) => {
        const costA = a.parts?.reduce((sum, p) => sum + (p.pricePaid || 0), 0) || 0
        const costB = b.parts?.reduce((sum, p) => sum + (p.pricePaid || 0), 0) || 0
        return costB - costA
      })
    case 'date':
    default:
      return builds.sort((a, b) => 
        new Date(b.updatedAt || b.createdAt).getTime() - 
        new Date(a.updatedAt || a.createdAt).getTime()
      )
  }
})

const confirmDelete = ref<string | null>(null)

onMounted(async () => {
  await buildStore.loadBuilds()
})

function handleCreateBuild() {
  createDialog.value?.openDialog()
}

function handleBuildCreated(buildId: string) {
  router.push(`/builds/${buildId}`)
}

function handleEditBuild(buildId: string) {
  router.push(`/builds/${buildId}`)
}

async function handleDeleteBuild(buildId: string) {
  if (confirmDelete.value === buildId) {
    buildStore.builds = buildStore.builds.filter(b => b.id !== buildId)
    confirmDelete.value = null
  } else {
    confirmDelete.value = buildId
    setTimeout(() => {
      if (confirmDelete.value === buildId) {
        confirmDelete.value = null
      }
    }, 3000)
  }
}

function cycleSortBy() {
  const options: SortBy[] = ['date', 'name', 'cost']
  const currentIndex = options.indexOf(sortBy.value)
  sortBy.value = options[(currentIndex + 1) % options.length] as SortBy
}

const sortLabel = computed(() => {
  const labels = {
    name: 'Name',
    date: 'Date',
    cost: 'Cost'
  }
  return labels[sortBy.value]
})
</script>

<template>
  <div class="container mx-auto p-6">
    <PageHeader
      title="My Builds"
      description="Manage your PC builds"
    >
      <template #actions>
        <Button @click="handleCreateBuild">
          <Plus class="h-4 w-4 mr-2" />
          New Build
        </Button>
      </template>
    </PageHeader>

    <LoadingState v-if="buildStore.isLoading && buildStore.builds.length === 0" />
    <ErrorState v-else-if="buildStore.error" :message="buildStore.error" />
    
    <div v-else>
      <EmptyState
        v-if="buildStore.builds.length === 0"
        title="No builds yet"
        description="Create your first PC build to get started"
      >
        <Button @click="handleCreateBuild">
          <Plus class="h-4 w-4 mr-2" />
          Create Your First Build
        </Button>
      </EmptyState>

      <div v-else class="space-y-4">
        <div class="flex justify-between items-center">
          <p class="text-sm text-muted-foreground">
            {{ buildStore.builds.length }} build{{ buildStore.builds.length !== 1 ? 's' : '' }}
          </p>
          <Button variant="outline" size="sm" @click="cycleSortBy">
            <ArrowUpDown class="h-4 w-4 mr-2" />
            Sort by: {{ sortLabel }}
          </Button>
        </div>

        <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
          <BuildCard
            v-for="build in sortedBuilds"
            :key="build.id"
            :build="build"
            @edit="handleEditBuild"
            @delete="handleDeleteBuild"
          />
        </div>
      </div>
    </div>

    <CreateBuildDialog ref="createDialog" @created="handleBuildCreated" />
  </div>
</template>
