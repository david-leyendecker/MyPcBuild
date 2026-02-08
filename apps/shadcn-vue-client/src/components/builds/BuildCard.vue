<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { Pencil, Trash2 } from 'lucide-vue-next'
import type { Build } from '@/types/build'
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import PriceDisplay from '@/components/shared/PriceDisplay.vue'

interface Props {
  build: Build
  totalPrice?: number
  hasErrors?: boolean
  hasWarnings?: boolean
}

interface Emits {
  (e: 'edit', id: string): void
  (e: 'delete', id: string): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()
const router = useRouter()

const partCount = computed(() => props.build.parts?.length || 0)
const displayPrice = computed(() => props.totalPrice || 
  props.build.parts?.reduce((sum, part) => sum + (part.pricePaid || 0), 0) || 0)

function viewDetails() {
  router.push(`/builds/${props.build.id}`)
}
</script>

<template>
  <Card class="hover:shadow-lg transition-shadow cursor-pointer" @click="viewDetails">
    <CardHeader>
      <CardTitle class="flex items-center justify-between">
        <span>{{ build.name }}</span>
        <div class="flex gap-2" @click.stop>
          <Button
            variant="ghost"
            size="sm"
            @click="emit('edit', build.id)"
          >
            <Pencil class="h-4 w-4" />
          </Button>
          <Button
            variant="ghost"
            size="sm"
            @click="emit('delete', build.id)"
          >
            <Trash2 class="h-4 w-4 text-destructive" />
          </Button>
        </div>
      </CardTitle>
    </CardHeader>
    <CardContent>
      <div class="space-y-2">
        <div class="flex justify-between text-sm">
          <span class="text-muted-foreground">Parts:</span>
          <span class="font-medium">{{ partCount }}</span>
        </div>
        <div class="flex justify-between text-sm">
          <span class="text-muted-foreground">Total Cost:</span>
          <PriceDisplay :amount="displayPrice" />
        </div>
        <div v-if="hasErrors || hasWarnings" class="pt-2 border-t">
          <span v-if="hasErrors" class="text-xs text-destructive font-medium">
            Has compatibility errors
          </span>
          <span v-else-if="hasWarnings" class="text-xs text-yellow-500 font-medium">
            Has compatibility warnings
          </span>
        </div>
      </div>
    </CardContent>
  </Card>
</template>
