<script setup lang="ts">
import { ref, watch } from 'vue'
import type { Chamber } from '@/types/spatial'
import { Button } from '@/components/ui/button'
import { Card, CardContent } from '@/components/ui/card'
import { Collapsible } from '@/components/ui/collapsible'
import { FormItemText } from '@/components/form-items'
import Label from '@/components/ui/label/Label.vue'
import Vector3Input from './Vector3Input.vue'
import DimensionsInput from './DimensionsInput.vue'
import SlotsInput from './SlotsInput.vue'
import { Plus, Trash2, ChevronDown, ChevronRight } from 'lucide-vue-next'

interface Props {
  modelValue?: Chamber[]
  editable?: boolean
  label?: string
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => [],
  editable: true,
  label: 'Chambers'
})

const emit = defineEmits<{
  'update:modelValue': [value: Chamber[]]
}>()

const localChambers = ref<Chamber[]>([...props.modelValue])

function addChamber() {
  localChambers.value.push({
    name: '',
    relativePosition: { x: 0, y: 0, z: 0 },
    dimensions: { length: 400, width: 200, height: 400 },
    slots: []
  })
  emitUpdate()
}

function removeChamber(index: number) {
  localChambers.value.splice(index, 1)
  emitUpdate()
}

function emitUpdate() {
  emit('update:modelValue', localChambers.value)
}

watch(() => props.modelValue, (newValue) => {
  localChambers.value = [...(newValue || [])]
}, { deep: true })
</script>

<template>
  <div class="space-y-3">
    <div class="flex items-center justify-between">
      <Label class="text-base font-semibold">{{ label }}</Label>
      <Button v-if="editable" size="sm" variant="outline" @click="addChamber">
        <Plus class="h-4 w-4 mr-1" />
        Add Chamber
      </Button>
    </div>

    <div v-if="localChambers.length === 0" class="text-sm text-muted-foreground py-4 text-center border border-dashed rounded-md">
      No chambers defined
    </div>

    <div v-else class="space-y-2">
      <Collapsible
        v-for="(chamber, index) in localChambers"
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
                Chamber {{ index + 1 }} — {{ chamber.name || 'Unnamed' }}
              </span>
            </div>
            <Button
              v-if="editable"
              size="sm"
              variant="ghost"
              @click.stop="removeChamber(index)"
            >
              <Trash2 class="h-4 w-4 text-destructive" />
            </Button>
          </div>
        </template>

        <Card class="mt-2">
          <CardContent class="p-4 space-y-4">
            <FormItemText
              label="Chamber Name *"
              :model-value="chamber.name"
              placeholder="Chamber Name"
              :disabled="!editable"
              @update:model-value="(v) => { chamber.name = v; emitUpdate() }"
            />

            <Vector3Input
              v-model="chamber.relativePosition"
              label="Chamber Position (mm) *"
              :editable="editable"
              @update:model-value="emitUpdate"
            />

            <DimensionsInput
              v-model="chamber.dimensions"
              label="Chamber Dimensions (mm) *"
              :editable="editable"
              @update:model-value="emitUpdate"
            />

            <SlotsInput
              v-model="chamber.slots"
              :editable="editable"
              @update:model-value="emitUpdate"
            />
          </CardContent>
        </Card>
      </Collapsible>
    </div>
  </div>
</template>
