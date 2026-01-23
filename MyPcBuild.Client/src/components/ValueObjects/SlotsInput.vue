<template>
  <div>
    <n-flex justify="space-between" align="center" style="margin-bottom: 12px;">
      <label style="font-weight: 600; font-size: 14px;">{{ label }}</label>
      <n-button
        v-if="editable"
        size="small"
        @click="addSlot"
      >
        <template #icon>
          <n-icon :component="Icons.Add" />
        </template>
        Add Slot
      </n-button>
    </n-flex>

    <div v-if="localSlots.length === 0" style="text-align: center; padding: 16px 0; opacity: 0.6;">
      <p style="font-size: 14px;">No slots defined</p>
    </div>

    <n-collapse
      v-else
      :expanded-names="expandedNames"
      @update:expanded-names="handleExpandedChange"
    >
      <n-collapse-item
        v-for="(slot, index) in localSlots"
        :key="index"
        :name="index"
      >
        <template #header>
          <n-flex justify="space-between" align="center" :size="8" style="width: 100%;">
            <span style="font-weight: 600; font-size: 14px;">
              Slot {{ index + 1 }} — {{ slot.name || 'Unnamed' }} ({{ slot.allowedCategory || 'Unknown' }})
            </span>
            <n-button
              v-if="editable"
              size="small"
              text
              type="error"
              @click.stop="removeSlot(index)"
            >
              <template #icon>
                <n-icon :component="Icons.Trash" />
              </template>
            </n-button>
          </n-flex>
        </template>

        <n-card :bordered="true" size="small">
          <n-flex :size="12" style="margin-bottom: 12px;">
            <n-input
              v-model:value="slot.name"
              placeholder="Slot Name *"
              :readonly="!editable"
              style="flex: 1; min-width: 150px;"
              @update:value="emitUpdate"
            />
            <n-select
              v-model:value="slot.allowedCategory"
              :options="categoryOptions"
              placeholder="Allowed Category *"
              :disabled="!editable"
              style="flex: 1; min-width: 150px;"
              @update:value="emitUpdate"
            />
          </n-flex>

          <div style="margin-bottom: 12px;">
            <label style="font-size: 12px; font-weight: 600; display: block; margin-bottom: 4px;">Relative Position (mm)</label>
            <n-flex :size="8">
              <n-input-number
                v-model:value="slot.relativePosition.x"
                placeholder="X"
                :readonly="!editable"
                style="flex: 1; min-width: 80px;"
                @update:value="emitUpdate"
              />
              <n-input-number
                v-model:value="slot.relativePosition.y"
                placeholder="Y"
                :readonly="!editable"
                style="flex: 1; min-width: 80px;"
                @update:value="emitUpdate"
              />
              <n-input-number
                v-model:value="slot.relativePosition.z"
                placeholder="Z"
                :readonly="!editable"
                style="flex: 1; min-width: 80px;"
                @update:value="emitUpdate"
              />
            </n-flex>
          </div>

          <div style="margin-bottom: 12px;">
            <label style="font-size: 12px; font-weight: 600; display: block; margin-bottom: 4px;">Max Dimensions (mm)</label>
            <n-flex :size="8">
              <n-input-number
                v-model:value="slot.maxDimensions.length"
                placeholder="Length"
                :readonly="!editable"
                style="flex: 1; min-width: 80px;"
                @update:value="emitUpdate"
              />
              <n-input-number
                v-model:value="slot.maxDimensions.width"
                placeholder="Width"
                :readonly="!editable"
                style="flex: 1; min-width: 80px;"
                @update:value="emitUpdate"
              />
              <n-input-number
                v-model:value="slot.maxDimensions.height"
                placeholder="Height"
                :readonly="!editable"
                style="flex: 1; min-width: 80px;"
                @update:value="emitUpdate"
              />
            </n-flex>
          </div>

          <div>
            <label style="font-size: 12px; font-weight: 600; display: block; margin-bottom: 4px;">Rotation (degrees, optional)</label>
            <n-flex :size="8">
              <n-input-number
                :value="slot.rotation?.x ?? 0"
                @update:value="updateRotation(slot, 'x', $event)"
                placeholder="X (Pitch)"
                :readonly="!editable"
                style="flex: 1; min-width: 80px;"
              />
              <n-input-number
                :value="slot.rotation?.y ?? 0"
                @update:value="updateRotation(slot, 'y', $event)"
                placeholder="Y (Yaw)"
                :readonly="!editable"
                style="flex: 1; min-width: 80px;"
              />
              <n-input-number
                :value="slot.rotation?.z ?? 0"
                @update:value="updateRotation(slot, 'z', $event)"
                placeholder="Z (Roll)"
                :readonly="!editable"
                style="flex: 1; min-width: 80px;"
              />
            </n-flex>
          </div>
        </n-card>
      </n-collapse-item>
    </n-collapse>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { NButton, NCard, NCollapse, NCollapseItem, NFlex, NInput, NSelect, NInputNumber, NIcon } from 'naive-ui';
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

function updateRotation(slot: Slot, axis: 'x' | 'y' | 'z', value: number | null) {
  if (!slot.rotation) {
    slot.rotation = { ...DEFAULT_ROTATION };
  }
  slot.rotation[axis] = value || 0;
  emitUpdate();
}

function emitUpdate() {
  emit('update:modelValue', localSlots.value);
}

function handleExpandedChange(names: Array<string | number> | string | number) {
  expandedNames.value = Array.isArray(names) ? names : [names];
}
</script>
