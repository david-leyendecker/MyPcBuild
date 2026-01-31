<template>
  <n-flex vertical style="width: 100%;" :size="12">
    <n-flex justify="space-between" align="center">
      <n-text strong>{{ label }}</n-text>
      <n-button v-if="editable" size="small" @click="addSlot">
        <template #icon>
          <n-icon :component="Icons.Add" />
        </template>
        Add Slot
      </n-button>
    </n-flex>

    <n-empty v-if="localSlots.length === 0" description="No slots defined" />

    <n-collapse v-else :expanded-names="expandedNames" @update:expanded-names="handleExpandedChange">
      <n-collapse-item v-for="(slot, index) in localSlots" :key="index" :name="index">
        <template #header>
          <n-flex justify="space-between" align="center" :size="8" style="width: 100%;">
            <n-text>
              Slot {{ index + 1 }} — {{ slot.name || 'Unnamed' }} ({{ slot.allowedCategory || 'Unknown' }})
            </n-text>
            <n-button v-if="editable" size="small" text type="error" @click.stop="removeSlot(index)">
              <template #icon>
                <n-icon :component="Icons.Trash" />
              </template>
            </n-button>
          </n-flex>
        </template>

        <n-card :bordered="true" size="small">
          <n-grid :cols="2">
            <n-form-item-gi label="Slot Name" :span="2">
              <n-input v-model:value="slot.name" placeholder="Slot Name *" :disabled="!editable"
                @update:value="emitUpdate" />
            </n-form-item-gi>

            <n-form-item-gi label="Allowed Category" :span="2">
              <n-select v-model:value="slot.allowedCategory" :options="categoryOptions" :disabled="!editable"
                @update:value="emitUpdate" />
            </n-form-item-gi>

            <n-form-item-gi label="Relative Position (mm)" :span="2">
              <Vector3Input v-model:modelValue="slot.relativePosition" :editable="editable"
                @update:modelValue="emitUpdate" />
            </n-form-item-gi>

            <n-form-item-gi label="Max Dimensions (mm)" :span="2">
              <DimensionsInput v-model:modelValue="slot.maxDimensions" :editable="editable"
                @update:modelValue="emitUpdate" />
            </n-form-item-gi>

            <n-form-item-gi label="Rotation (degrees, optional)" :span="2">
              <RotationInput v-model:modelValue="slot.rotation" :editable="editable" @update:modelValue="emitUpdate" />
            </n-form-item-gi>
          </n-grid>
        </n-card>
      </n-collapse-item>
    </n-collapse>
  </n-flex>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { NButton, NCard, NCollapse, NCollapseItem, NFlex, NInput, NSelect, NIcon, NEmpty, NFormItemGi, NGrid, NText } from 'naive-ui';
import Vector3Input from './Vector3Input.vue';
import RotationInput from './RotationInput.vue';
import DimensionsInput from './DimensionsInput.vue';
import type { Slot } from '@/types/products';
import { Icons } from '@/utils/icons';

interface Props {
  modelValue?: Slot[];
  editable?: boolean;
  label?: string;
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => [],
  editable: true,
  label: 'Slots'
});

const emit = defineEmits<{
  'update:modelValue': [value: Slot[]]
}>();

const categoryOptions = [
  { label: 'CPU', value: 'CPU' },
  { label: 'GPU', value: 'GPU' },
  { label: 'Motherboard', value: 'Motherboard' },
  { label: 'RAM', value: 'RAM' },
  { label: 'Storage', value: 'Storage' },
  { label: 'PowerSupply', value: 'PowerSupply' },
  { label: 'Cooler', value: 'Cooler' },
  { label: 'Case', value: 'Case' }
];

const DEFAULT_POSITION = { x: 0, y: 0, z: 0 };
const DEFAULT_DIMENSIONS = { length: 0, width: 0, height: 0 };
const DEFAULT_ROTATION = { x: 0, y: 0, z: 0 };

const localSlots = ref<Slot[]>(props.modelValue.map(slot => ({
  ...slot,
  relativePosition: slot.relativePosition || { ...DEFAULT_POSITION },
  maxDimensions: slot.maxDimensions || { ...DEFAULT_DIMENSIONS },
  rotation: slot.rotation ? { ...slot.rotation } : { ...DEFAULT_ROTATION }
})));

const expandedNames = ref<(number | string)[]>(props.modelValue.map((_, index) => index));

watch(
  () => props.modelValue,
  (newValue) => {
    localSlots.value = newValue.map(slot => ({
      ...slot,
      relativePosition: slot.relativePosition || { ...DEFAULT_POSITION },
      maxDimensions: slot.maxDimensions || { ...DEFAULT_DIMENSIONS },
      rotation: slot.rotation ? { ...slot.rotation } : { ...DEFAULT_ROTATION }
    }));
    expandedNames.value = newValue.map((_, index) => index);
  },
  { deep: true }
);

function addSlot() {
  localSlots.value.push({
    name: '',
    allowedCategory: 'CPU',
    relativePosition: { ...DEFAULT_POSITION },
    maxDimensions: { length: 50, width: 50, height: 20 },
    rotation: { ...DEFAULT_ROTATION },
    subSlots: []
  });
  expandedNames.value.push(localSlots.value.length - 1);
  emitUpdate();
}

function removeSlot(index: number) {
  localSlots.value.splice(index, 1);
  expandedNames.value = expandedNames.value
    .filter(name => name !== index)
    .map(name => (typeof name === 'number' && name > index ? name - 1 : name));
  emitUpdate();
}

function emitUpdate() {
  emit('update:modelValue', localSlots.value);
}

function handleExpandedChange(names: Array<string | number> | string | number) {
  expandedNames.value = Array.isArray(names) ? names : [names];
}
</script>
