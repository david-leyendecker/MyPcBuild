<script setup lang="ts">
import { ref } from 'vue'
import { useBuildStore } from '@/stores/buildStore'
import { Dialog } from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { FormItemText } from '@/components/form-items'

interface Emits {
  (e: 'created', buildId: string): void
}

const emit = defineEmits<Emits>()

const buildStore = useBuildStore()

const open = ref(false)
const buildName = ref('')
const error = ref<string | null>(null)

function openDialog() {
  open.value = true
  buildName.value = ''
  error.value = null
}

async function handleCreate() {
  if (!buildName.value.trim()) {
    error.value = 'Build name is required'
    return
  }

  try {
    const newBuild = await buildStore.createBuild(buildName.value.trim())
    open.value = false
    buildName.value = ''
    error.value = null
    emit('created', newBuild.id)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Failed to create build'
  }
}

defineExpose({ openDialog })
</script>

<template>
  <Dialog v-model:open="open" title="Create New Build">
    <div class="space-y-4" @keydown.enter="handleCreate">
      <FormItemText
        label="Build Name"
        v-model="buildName"
        placeholder="My Gaming PC"
        :error="error ?? undefined"
      />
      
      <div class="flex justify-end gap-2 pt-4">
        <Button variant="outline" @click="open = false">
          Cancel
        </Button>
        <Button @click="handleCreate" :disabled="!buildName.trim()">
          Create Build
        </Button>
      </div>
    </div>
  </Dialog>
</template>
