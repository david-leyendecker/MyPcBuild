<template>
  <div>
    <n-flex justify="space-between" align="center" style="margin-bottom: 12px;">
      <label style="font-weight: 600; font-size: 14px;">Chambers</label>
      <n-button
        v-if="editable"
        size="small"
        @click="addChamber"
      >
        <template #icon>
          <n-icon :component="Icons.Add" />
        </template>
        Add Chamber
      </n-button>
    </n-flex>

    <div v-if="localChambers.length === 0" style="text-align: center; padding: 16px 0; opacity: 0.6;">
      <p style="font-size: 14px;">No chambers defined</p>
    </div>

    <n-collapse
      v-else
      :expanded-names="expandedNames"
      @update:expanded-names="handleExpandedChange"
    >
      <n-collapse-item
        v-for="(chamber, index) in localChambers"
        :key="index"
        :name="index"
      >
        <template #header>
          <n-flex justify="space-between" align="center" :size="8" style="width: 100%;">
            <span style="font-weight: 600; font-size: 14px;">
              Chamber {{ index + 1 }} — {{ chamber.name || 'Unnamed' }}
            </span>
            <n-button
              v-if="editable"
              size="small"
              text
              type="error"
              @click.stop="removeChamber(index)"
            >
              <template #icon>
                <n-icon :component="Icons.Trash" />
              </template>
            </n-button>
          </n-flex>
        </template>

        <n-card :bordered="true" size="small">
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

          <SlotsInput
            v-model="chamber.slots"
            :editable="editable"
            label="Slots in Chamber"
            @update:model-value="emitUpdate"
          />
        </n-card>
      </n-collapse-item>
    </n-collapse>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { NButton, NCard, NCollapse, NCollapseItem, NFlex, NInput, NInputNumber, NIcon } from 'naive-ui';
import type { Chamber } from '@/types/products';
import SlotsInput from './SlotsInput.vue';
import { Icons } from '@/utils/icons';

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
  relativePosition: chamber.relativePosition || { ...DEFAULT_POSITION },
  dimensions: chamber.dimensions || { ...DEFAULT_DIMENSIONS },
  slots: chamber.slots || [...DEFAULT_SLOTS]
})));

const expandedNames = ref<(number | string)[]>(props.modelValue.map((_, index) => index));

watch(
  () => props.modelValue,
  (newValue) => {
    localChambers.value = newValue.map(chamber => ({
      ...chamber,
      relativePosition: chamber.relativePosition || { ...DEFAULT_POSITION },
      dimensions: chamber.dimensions || { ...DEFAULT_DIMENSIONS },
      slots: chamber.slots || [...DEFAULT_SLOTS]
    }));
    expandedNames.value = newValue.map((_, index) => index);
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
  expandedNames.value.push(localChambers.value.length - 1);
  emitUpdate();
}

function removeChamber(index: number) {
  localChambers.value.splice(index, 1);
  expandedNames.value = expandedNames.value
    .filter(name => name !== index)
    .map(name => (typeof name === 'number' && name > index ? name - 1 : name));
  emitUpdate();
}

function emitUpdate() {
  emit('update:modelValue', localChambers.value);
}

function handleExpandedChange(names: Array<string | number> | string | number) {
  expandedNames.value = Array.isArray(names) ? names : [names];
}
</script>
