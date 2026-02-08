<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import type { ProductSummary } from '@/api/catalog'
import { getCategoryFromBackend } from '@/api/catalog'
import Card from '@/components/ui/card/Card.vue'
import CardHeader from '@/components/ui/card/CardHeader.vue'
import CardTitle from '@/components/ui/card/CardTitle.vue'
import CardContent from '@/components/ui/card/CardContent.vue'
import CategoryIcon from '@/components/shared/CategoryIcon.vue'
import StatusBadge from '@/components/shared/StatusBadge.vue'
import PriceDisplay from '@/components/shared/PriceDisplay.vue'

interface Props {
  product: ProductSummary
}

const props = defineProps<Props>()
const router = useRouter()

const category = computed(() => getCategoryFromBackend(props.product.categoryName))

function handleClick() {
  router.push({ name: 'product-detail', params: { id: props.product.id } })
}
</script>

<template>
  <Card
    class="cursor-pointer transition-all hover:shadow-lg hover:border-primary"
    @click="handleClick"
  >
    <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
      <div class="flex items-center gap-2">
        <CategoryIcon v-if="category" :category="category" class="h-5 w-5 text-muted-foreground" />
        <StatusBadge :is-draft="product.isDraft" />
      </div>
    </CardHeader>
    <CardContent>
      <CardTitle class="mb-2 text-lg">{{ product.name }}</CardTitle>
      <div class="text-sm text-muted-foreground mb-3">
        {{ product.manufacturer }}
      </div>
      <div class="flex items-center justify-between">
        <PriceDisplay :amount="product.price" />
        <span class="text-xs text-muted-foreground capitalize">{{ product.categoryName }}</span>
      </div>
    </CardContent>
  </Card>
</template>
