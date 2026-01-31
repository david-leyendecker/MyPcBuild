<template>
  <n-flex vertical  style="width: 100%;" :size="12">
    <n-flex justify="space-between" align="center">
      <n-text strong>{{ label }}</n-text>
      <n-button v-if="editable" size="small" @click="addChamber">
        <template #icon>
          <n-icon :component="Icons.Add" />
        </template>
        Add Chamber
      </n-button>
    </n-flex>

    <n-empty v-if="localChambers.length === 0" description="No chambers defined" />

    <n-collapse v-else :expanded-names="expandedNames" @update:expanded-names="handleExpandedChange">
      <n-collapse-item v-for="(chamber, index) in localChambers" :key="index" :name="index">
        <template #header>
          <n-flex justify="space-between" align="center" style="width: 100%;">
            <n-text depth="3">
              Chamber {{ index + 1 }} — {{ chamber.name || 'Unnamed' }}
            </n-text>
            <n-button v-if="editable" size="small" text type="error" @click.stop="removeChamber(index)">
              <template #icon>
                <n-icon :component="Icons.Trash" />
              </template>
            </n-button>
          </n-flex>
        </template>

        <n-card :bordered="true" size="small">
          <n-grid :cols="2">
            <n-form-item-gi label="Chamber Name" :span="2">
              <n-input v-model:value="chamber.name" placeholder="Chamber Name *" :disabled="!editable"
                @update:value="emitUpdate" />
            </n-form-item-gi>

            <n-form-item-gi label="Chamber Position (mm)" :span="2">
              <Vector3Input v-model:modelValue="chamber.relativePosition" :editable="editable"
                @update:modelValue="emitUpdate" />
            </n-form-item-gi>

            <n-form-item-gi label="Chamber Dimensions (mm)" :span="2">
              <DimensionsInput v-model:modelValue="chamber.dimensions" :editable="editable"
                @update:modelValue="emitUpdate" />
            </n-form-item-gi>

            <n-form-item-gi :span="2">
              <SlotsInput v-model:modelValue="chamber.slots" :editable="editable"
                @update:modelValue="emitUpdate" />
            </n-form-item-gi>
          </n-grid>
        </n-card>
      </n-collapse-item>
    </n-collapse>
  </n-flex>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { NButton, NCard, NCollapse, NCollapseItem, NFlex, NInput, NIcon, NEmpty, NFormItemGi, NGrid, NText } from 'naive-ui';
import Vector3Input from './Vector3Input.vue';
import DimensionsInput from './DimensionsInput.vue';
import type { Chamber } from '@/types/products';
import SlotsInput from './SlotsInput.vue';
import { Icons } from '@/utils/icons';

interface Props {
  modelValue?: Chamber[];
  editable?: boolean;
  label?: string;
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => [],
  editable: true,
  label: 'Chambers'
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
