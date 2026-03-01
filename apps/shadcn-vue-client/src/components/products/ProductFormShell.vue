<script setup lang="ts">
import { ref } from 'vue'
import type { ProductCategory } from '@/types/product'
import { FormItemText, FormItemMoney } from '@/components/form-items'
import Button from '@/components/ui/button/Button.vue'

interface Props {
  category: ProductCategory
  isSubmitting?: boolean
  initialName?: string
  initialManufacturer?: string
  initialPrice?: number
}

const props = withDefaults(defineProps<Props>(), {
  initialName: '',
  initialManufacturer: '',
  initialPrice: 0
})

const emit = defineEmits<{
  submit: [data: { name: string; manufacturer: string; price: number; isDraft: boolean }]
}>()

const name = ref(props.initialName)
const manufacturer = ref(props.initialManufacturer)
const price = ref(props.initialPrice)

const errors = ref<Record<string, string>>({})

function validate() {
  errors.value = {}
  
  if (!name.value.trim()) {
    errors.value.name = 'Name is required'
  }
  
  if (!manufacturer.value.trim()) {
    errors.value.manufacturer = 'Manufacturer is required'
  }
  
  if (price.value <= 0) {
    errors.value.price = 'Price must be greater than 0'
  }
  
  return Object.keys(errors.value).length === 0
}

function handleSubmit(isDraft: boolean) {
  if (!validate()) return
  
  emit('submit', {
    name: name.value,
    manufacturer: manufacturer.value,
    price: price.value,
    isDraft
  })
}

defineExpose({
  handleSubmit,
  name,
  manufacturer,
  price
})
</script>

<template>
  <div class="space-y-6">
    <!-- Common Fields -->
    <div class="grid gap-4 md:grid-cols-2">
      <FormItemText
        label="Product Name *"
        v-model="name"
        placeholder="Enter product name"
        :error="errors.name"
      />

      <FormItemText
        label="Manufacturer *"
        v-model="manufacturer"
        placeholder="Enter manufacturer"
        :error="errors.manufacturer"
      />

      <FormItemMoney
        label="Price (USD) *"
        v-model="price"
        placeholder="0.00"
        :error="errors.price"
      />
    </div>

    <!-- Category-Specific Fields Slot -->
    <div class="border-t pt-6">
      <h3 class="text-lg font-semibold mb-4">Category-Specific Fields</h3>
      <slot />
    </div>

    <!-- Actions -->
    <div class="flex justify-end gap-2 pt-4 border-t">
      <Button
        variant="outline"
        :disabled="isSubmitting"
        @click="handleSubmit(true)"
      >
        Save as Draft
      </Button>
      <Button
        :disabled="isSubmitting"
        @click="handleSubmit(false)"
      >
        {{ isSubmitting ? 'Saving...' : 'Save & Publish' }}
      </Button>
    </div>
  </div>
</template>
