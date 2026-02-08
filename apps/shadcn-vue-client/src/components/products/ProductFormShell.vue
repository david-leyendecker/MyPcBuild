<script setup lang="ts">
import { ref } from 'vue'
import type { ProductCategory } from '@/types/product'
import Input from '@/components/ui/input/Input.vue'
import Label from '@/components/ui/label/Label.vue'
import Button from '@/components/ui/button/Button.vue'

interface Props {
  category: ProductCategory
  isSubmitting?: boolean
}

const props = defineProps<Props>()

const emit = defineEmits<{
  submit: [data: { name: string; manufacturer: string; price: number; isDraft: boolean }]
}>()

const name = ref('')
const manufacturer = ref('')
const price = ref(0)

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
      <div class="space-y-2">
        <Label for="name">Product Name *</Label>
        <Input
          id="name"
          v-model="name"
          type="text"
          placeholder="Enter product name"
          :class="{ 'border-red-500': errors.name }"
        />
        <span v-if="errors.name" class="text-sm text-red-500">{{ errors.name }}</span>
      </div>

      <div class="space-y-2">
        <Label for="manufacturer">Manufacturer *</Label>
        <Input
          id="manufacturer"
          v-model="manufacturer"
          type="text"
          placeholder="Enter manufacturer"
          :class="{ 'border-red-500': errors.manufacturer }"
        />
        <span v-if="errors.manufacturer" class="text-sm text-red-500">{{ errors.manufacturer }}</span>
      </div>

      <div class="space-y-2">
        <Label for="price">Price (USD) *</Label>
        <Input
          id="price"
          v-model.number="price"
          type="number"
          step="0.01"
          min="0"
          placeholder="0.00"
          :class="{ 'border-red-500': errors.price }"
        />
        <span v-if="errors.price" class="text-sm text-red-500">{{ errors.price }}</span>
      </div>
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
