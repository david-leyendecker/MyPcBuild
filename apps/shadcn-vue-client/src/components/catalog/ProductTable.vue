<script setup lang="ts">
import { useRouter } from 'vue-router'
import type { ProductSummary } from '@/api/catalog'
import { getCategoryFromBackend } from '@/api/catalog'
import CategoryIcon from '@/components/shared/CategoryIcon.vue'
import StatusBadge from '@/components/shared/StatusBadge.vue'
import PriceDisplay from '@/components/shared/PriceDisplay.vue'

interface Props {
  products: ProductSummary[]
}

const props = defineProps<Props>()
const router = useRouter()

function getCategoryForProduct(categoryName: string) {
  return getCategoryFromBackend(categoryName)
}

function handleRowClick(productId: string) {
  router.push({ name: 'product-detail', params: { id: productId } })
}
</script>

<template>
  <div class="rounded-md border">
    <table class="w-full">
      <thead>
        <tr class="border-b bg-muted/50">
          <th class="h-12 px-4 text-left align-middle font-medium text-muted-foreground w-12">
            Category
          </th>
          <th class="h-12 px-4 text-left align-middle font-medium text-muted-foreground">
            Name
          </th>
          <th class="h-12 px-4 text-left align-middle font-medium text-muted-foreground">
            Manufacturer
          </th>
          <th class="h-12 px-4 text-left align-middle font-medium text-muted-foreground">
            Price
          </th>
          <th class="h-12 px-4 text-left align-middle font-medium text-muted-foreground">
            Status
          </th>
        </tr>
      </thead>
      <tbody>
        <tr
          v-for="product in products"
          :key="product.id"
          class="border-b transition-colors hover:bg-muted/50 cursor-pointer"
          @click="handleRowClick(product.id)"
        >
          <td class="p-4 align-middle">
            <CategoryIcon
              v-if="getCategoryForProduct(product.categoryName)"
              :category="getCategoryForProduct(product.categoryName)!"
              class="h-5 w-5 text-muted-foreground"
            />
          </td>
          <td class="p-4 align-middle font-medium">
            {{ product.name }}
          </td>
          <td class="p-4 align-middle text-muted-foreground">
            {{ product.manufacturer }}
          </td>
          <td class="p-4 align-middle">
            <PriceDisplay :amount="product.price" />
          </td>
          <td class="p-4 align-middle">
            <StatusBadge :is-draft="product.isDraft" />
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
