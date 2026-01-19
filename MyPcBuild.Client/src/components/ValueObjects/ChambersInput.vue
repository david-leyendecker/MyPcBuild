<template>
  <div>
    <n-flex justify="space-between" align="center" style="margin-bottom: 12px;">
      <label style="font-weight: 600; font-size: 14px;">Chambers</label>
      <n-button
        v-if="editable"
        size="small"
        @click="addChamber"
      >
        ➕ Add Chamber
      </n-button>
    </n-flex>

    <div v-if="localChambers.length === 0" style="text-align: center; padding: 16px 0; opacity: 0.6;">
      <p style="font-size: 14px;">No chambers defined</p>
    </div>

    <n-flex v-else vertical :size="12">
      <n-card
        v-for="(chamber, index) in localChambers"
        :key="index"
        :bordered="true"
        size="small"
      >
        <n-flex justify="space-between" align="center" style="margin-bottom: 12px;">
          <h4 style="font-weight: 600; font-size: 16px; margin: 0;">Chamber {{ index + 1 }}</h4>
          <n-button
            v-if="editable"
            size="small"
            text
            type="error"
            @click="removeChamber(index)"
          >
            🗑️
          </n-button>
        </n-flex>

        <div style="margin-bottom: 12px;">
          <n-input
            v-model:value="chamber.name"
            placeholder="Chamber Name *"
            :readonly="!editable"
            @update:value="emitUpdate"
          />
        </div>

        <div style="margin-bottom: 12px;">
          <label style="font-size: 12px; font-weight: 600; display: block; margin-bottom: 4px;">Chamber Position (mm)</label>
          <n-flex :size="8">
            <n-input-number
              v-model:value="chamber.relativePosition.x"
              placeholder="X"
              :readonly="!editable"
              style="flex: 1; min-width: 80px;"
              @update:value="emitUpdate"
            />
            <n-input-number
              v-model:value="chamber.relativePosition.y"
              placeholder="Y"
              :readonly="!editable"
              style="flex: 1; min-width: 80px;"
              @update:value="emitUpdate"
            />
            <n-input-number
              v-model:value="chamber.relativePosition.z"
              placeholder="Z"
              :readonly="!editable"
              style="flex: 1; min-width: 80px;"
              @update:value="emitUpdate"
            />
          </n-flex>
        </div>

        <div style="margin-bottom: 12px;">
          <label style="font-size: 12px; font-weight: 600; display: block; margin-bottom: 4px;">Chamber Dimensions (mm)</label>
          <n-flex :size="8">
            <n-input-number
              v-model:value="chamber.dimensions.length"
              placeholder="Length"
              :readonly="!editable"
              style="flex: 1; min-width: 80px;"
              @update:value="emitUpdate"
            />
            <n-input-number
              v-model:value="chamber.dimensions.width"
              placeholder="Width"
              :readonly="!editable"
              style="flex: 1; min-width: 80px;"
              @update:value="emitUpdate"
            />
            <n-input-number
              v-model:value="chamber.dimensions.height"
              placeholder="Height"
              :readonly="!editable"
              style="flex: 1; min-width: 80px;"
              @update:value="emitUpdate"
            />
          </n-flex>
        </div>

        <n-divider style="margin: 12px 0;" />

        <!-- Slots within the chamber -->
        <SlotsInput
          v-model="chamber.slots"
          :editable="editable"
          label="Slots in Chamber"
          @update:model-value="emitUpdate"
        />
      </n-card>
    </n-flex>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { NCard, NButton, NFlex, NInput, NInputNumber, NDivider } from 'naive-ui';
import type { Chamber } from '@/types/products';
import SlotsInput from './SlotsInput.vue';

interface Props {
  modelValue?: Chamber[];
  editable?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => [],
  editable: true
});

const emit = defineEmits<{
  'update:modelValue': [value: Chamber[]]
}>();

const DEFAULT_DIMENSIONS = { length: 0, width: 0, height: 0 };
const DEFAULT_POSITION = { x: 0, y: 0, z: 0 };
const DEFAULT_SLOTS: never[] = [];

const localChambers = ref<Chamber[]>(props.modelValue.map(chamber => ({
  ...chamber,
  relativePosition: chamber.relativePosition || DEFAULT_POSITION,
  dimensions: chamber.dimensions || DEFAULT_DIMENSIONS,
  slots: chamber.slots || DEFAULT_SLOTS
})));

watch(
  () => props.modelValue,
  (newValue) => {
    localChambers.value = newValue.map(chamber => ({
      ...chamber,
      relativePosition: chamber.relativePosition || DEFAULT_POSITION,
      dimensions: chamber.dimensions || DEFAULT_DIMENSIONS,
      slots: chamber.slots || DEFAULT_SLOTS
    }));
  },
  { deep: true }
);

function addChamber() {
  localChambers.value.push({
    name: '',
    relativePosition: { x: 0, y: 0, z: 0 },
    dimensions: { length: 400, width: 200, height: 400 },
    slots: []
  });
  emitUpdate();
}

function removeChamber(index: number) {
  localChambers.value.splice(index, 1);
  emitUpdate();
}

function emitUpdate() {
  emit('update:modelValue', localChambers.value);
}
</script>
