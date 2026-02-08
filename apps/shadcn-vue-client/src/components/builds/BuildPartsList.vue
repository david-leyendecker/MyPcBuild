<script setup lang="ts">
import { ref } from 'vue'
import { Trash2, Plus } from 'lucide-vue-next'
import type { BuildPart } from '@/types/build'
import type { ProductCategory } from '@/types/product'
import { Button } from '@/components/ui/button'
import PriceDisplay from '@/components/shared/PriceDisplay.vue'
import CategoryIcon from '@/components/shared/CategoryIcon.vue'

interface Props {
  parts: BuildPart[]
}

interface Emits {
  (e: 'remove', productId: string): void
  (e: 'add-part'): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const confirmDelete = ref<string | null>(null)

function handleRemove(productId: string) {
  if (confirmDelete.value === productId) {
    emit('remove', productId)
    confirmDelete.value = null
  } else {
    confirmDelete.value = productId
    setTimeout(() => {
      if (confirmDelete.value === productId) {
        confirmDelete.value = null
      }
    }, 3000)
  }
}
</script>

<template>
  <div class="space-y-4">
    <div class="flex justify-between items-center">
      <h3 class="text-lg font-semibold">Parts List</h3>
      <Button @click="emit('add-part')">
        <Plus class="h-4 w-4 mr-2" />
        Add Part
      </Button>
    </div>

    <div v-if="parts.length === 0" class="text-center py-8 text-muted-foreground border rounded-lg">
      <p>No parts added yet. Click "Add Part" to get started.</p>
    </div>

    <div v-else class="border rounded-lg overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-muted">
            <tr>
              <th class="text-left p-3 text-sm font-medium">Category</th>
              <th class="text-left p-3 text-sm font-medium">Name</th>
              <th class="text-left p-3 text-sm font-medium">Manufacturer</th>
              <th class="text-right p-3 text-sm font-medium">Price</th>
              <th class="text-right p-3 text-sm font-medium w-24">Actions</th>
            </tr>
          </thead>
          <tbody class="divide-y">
            <tr v-for="part in parts" :key="part.id" class="hover:bg-muted/50">
              <td class="p-3">
                <div class="flex items-center gap-2">
                  <CategoryIcon :category="part.category as ProductCategory" class="h-4 w-4" />
                  <span class="text-sm">{{ part.categoryName }}</span>
                </div>
              </td>
              <td class="p-3 text-sm font-medium">{{ part.name }}</td>
              <td class="p-3 text-sm text-muted-foreground">{{ part.manufacturer }}</td>
              <td class="p-3 text-right">
                <PriceDisplay :amount="part.pricePaid" />
              </td>
              <td class="p-3 text-right">
                <Button
                  variant="ghost"
                  size="sm"
                  @click="handleRemove(part.id)"
                  :class="confirmDelete === part.id ? 'text-destructive' : ''"
                >
                  <Trash2 class="h-4 w-4" />
                  <span v-if="confirmDelete === part.id" class="ml-1 text-xs">Confirm?</span>
                </Button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
