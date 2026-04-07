<script setup lang="ts">
import { ref, watch } from 'vue'
import type { Slot, ProductCategory } from '@/types/spatial'
import { Button } from '@/components/ui/button'
import { Card, CardContent } from '@/components/ui/card'
import { Collapsible } from '@/components/ui/collapsible'
import { FormItemText, FormItemSelect } from '@/components/form-items'
import Label from '@/components/ui/label/Label.vue'
import { productCategoryOptions } from '@/constants/enumOptions'
import Vector3Input from './Vector3Input.vue'
import DimensionsInput from './DimensionsInput.vue'
import RotationInput from './RotationInput.vue'
import { Plus, Trash2, ChevronDown, ChevronRight } from 'lucide-vue-next'

interface Props {
  modelValue?: Slot[]
  editable?: boolean
  label?: string
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => [],
  editable: true,
  label: 'Slots'
})

const emit = defineEmits<{
  'update:modelValue': [value: Slot[]]
}>()

const localSlots = ref<Slot[]>([...props.modelValue])

function addSlot() {
  localSlots.value.push({
    name: '',
    allowedCategory: 'cpu',
    relativePosition: { x: 0, y: 0, z: 0 },
    maxDimensions: { length: 100, width: 100, height: 100 },
    rotation: null,
    subSlots: []
  })
  emitUpdate()
}

function removeSlot(index: number) {
  localSlots.value.splice(index, 1)
  emitUpdate()
}

function emitUpdate() {
  emit('update:modelValue', localSlots.value)
}

watch(() => props.modelValue, (newValue) => {
  localSlots.value = [...(newValue || [])]
}, { deep: true })
</script>

<template>
  <div class="space-y-3">
    <div class="flex items-center justify-between">
      <Label class="text-base font-semibold">{{ label }}</Label>
      <Button v-if="editable" size="sm" variant="outline" @click="addSlot">
        <Plus class="h-4 w-4 mr-1" />
        Add Slot
      </Button>
    </div>

    <div v-if="localSlots.length === 0" class="text-sm text-muted-foreground py-4 text-center border border-dashed rounded-md">
      No slots defined
    </div>

    <div v-else class="space-y-2">
      <Collapsible
        v-for="(slot, index) in localSlots"
        :key="index"
        :default-open="false"
      >
        <template #trigger="{ isOpen, toggle }">
          <div
            class="flex items-center justify-between p-3 bg-muted/50 rounded-md cursor-pointer hover:bg-muted"
            @click="toggle"
          >
            <div class="flex items-center gap-2">
              <component :is="isOpen ? ChevronDown : ChevronRight" class="h-4 w-4" />
              <span class="font-medium text-sm">
                Slot {{ index + 1 }} — {{ slot.name || 'Unnamed' }} ({{ slot.allowedCategory }})
              </span>
            </div>
            <Button
              v-if="editable"
              size="sm"
              variant="ghost"
              @click.stop="removeSlot(index)"
            >
              <Trash2 class="h-4 w-4 text-destructive" />
            </Button>
          </div>
        </template>

        <Card class="mt-2">
          <CardContent class="p-4 space-y-4">
            <div class="grid gap-4 md:grid-cols-2">
              <FormItemText
                label="Slot Name *"
                :model-value="slot.name"
                placeholder="Slot Name"
                :disabled="!editable"
                @update:model-value="(v) => { slot.name = v; emitUpdate() }"
              />

              <FormItemSelect
                label="Allowed Category *"
                :model-value="slot.allowedCategory"
                :options="productCategoryOptions"
                :disabled="!editable"
                @update:model-value="(v) => { slot.allowedCategory = v as ProductCategory; emitUpdate() }"
              />
            </div>

            <Vector3Input
              v-model="slot.relativePosition"
              label="Relative Position (mm) *"
              :editable="editable"
              @update:model-value="emitUpdate"
            />

            <DimensionsInput
              v-model="slot.maxDimensions"
              label="Max Dimensions (mm) *"
              :editable="editable"
              @update:model-value="emitUpdate"
            />

            <RotationInput
              v-model="slot.rotation"
              label="Rotation (degrees, optional)"
              :editable="editable"
              @update:model-value="emitUpdate"
            />
          </CardContent>
        </Card>
      </Collapsible>
    </div>
  </div>
</template>
