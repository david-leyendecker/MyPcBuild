<template>
  <div>
    <div class="d-flex justify-space-between align-center mb-3">
      <label class="text-subtitle-2 font-weight-semibold">{{ label }}</label>
      <v-btn
        v-if="editable"
        size="small"
        prepend-icon="mdi-plus"
        variant="outlined"
        @click="addSlot"
      >
        Add Slot
      </v-btn>
    </div>

    <div v-if="localSlots.length === 0" class="text-center py-4 text-medium-emphasis">
      <p class="text-body-2">No slots defined</p>
    </div>

    <div v-else class="d-flex flex-column ga-3">
      <v-card
        v-for="(slot, index) in localSlots"
        :key="index"
        variant="outlined"
        class="pa-3"
      >
        <div class="d-flex justify-space-between align-center mb-2">
          <h4 class="text-subtitle-2 font-weight-semibold">Slot {{ index + 1 }}</h4>
          <v-btn
            v-if="editable"
            size="small"
            icon="mdi-delete"
            variant="text"
            color="error"
            @click="removeSlot(index)"
          ></v-btn>
        </div>

        <v-row dense>
          <v-col cols="12" md="6">
            <v-text-field
              v-model="slot.name"
              label="Slot Name *"
              :readonly="!editable"
              :variant="editable ? 'filled' : 'outlined'"
              density="compact"
              placeholder="e.g., CPU Socket, RAM Slot 1"
              @update:model-value="emitUpdate"
            ></v-text-field>
          </v-col>
          <v-col cols="12" md="6">
            <v-select
              v-model="slot.allowedCategory"
              :items="categoryOptions"
              label="Allowed Category *"
              :readonly="!editable"
              :variant="editable ? 'filled' : 'outlined'"
              density="compact"
              @update:model-value="emitUpdate"
            ></v-select>
          </v-col>
        </v-row>

        <v-row dense>
          <v-col cols="12">
            <label class="text-caption font-weight-semibold mb-1 d-block">Relative Position (mm)</label>
            <v-row dense>
              <v-col cols="4">
                <v-text-field
                  v-model.number="slot.relativePosition.x"
                  label="X"
                  type="number"
                  :readonly="!editable"
                  :variant="editable ? 'filled' : 'outlined'"
                  density="compact"
                  @update:model-value="emitUpdate"
                ></v-text-field>
              </v-col>
              <v-col cols="4">
                <v-text-field
                  v-model.number="slot.relativePosition.y"
                  label="Y"
                  type="number"
                  :readonly="!editable"
                  :variant="editable ? 'filled' : 'outlined'"
                  density="compact"
                  @update:model-value="emitUpdate"
                ></v-text-field>
              </v-col>
              <v-col cols="4">
                <v-text-field
                  v-model.number="slot.relativePosition.z"
                  label="Z"
                  type="number"
                  :readonly="!editable"
                  :variant="editable ? 'filled' : 'outlined'"
                  density="compact"
                  @update:model-value="emitUpdate"
                ></v-text-field>
              </v-col>
            </v-row>
          </v-col>
        </v-row>

        <v-row dense>
          <v-col cols="12">
            <label class="text-caption font-weight-semibold mb-1 d-block">Max Dimensions (mm)</label>
            <v-row dense>
              <v-col cols="4">
                <v-text-field
                  v-model.number="slot.maxDimensions.length"
                  label="Length"
                  type="number"
                  :readonly="!editable"
                  :variant="editable ? 'filled' : 'outlined'"
                  density="compact"
                  @update:model-value="emitUpdate"
                ></v-text-field>
              </v-col>
              <v-col cols="4">
                <v-text-field
                  v-model.number="slot.maxDimensions.width"
                  label="Width"
                  type="number"
                  :readonly="!editable"
                  :variant="editable ? 'filled' : 'outlined'"
                  density="compact"
                  @update:model-value="emitUpdate"
                ></v-text-field>
              </v-col>
              <v-col cols="4">
                <v-text-field
                  v-model.number="slot.maxDimensions.height"
                  label="Height"
                  type="number"
                  :readonly="!editable"
                  :variant="editable ? 'filled' : 'outlined'"
                  density="compact"
                  @update:model-value="emitUpdate"
                ></v-text-field>
              </v-col>
            </v-row>
          </v-col>
        </v-row>

        <v-row dense>
          <v-col cols="12">
            <label class="text-caption font-weight-semibold mb-1 d-block">Rotation (degrees, optional)</label>
            <v-row dense>
              <v-col cols="4">
                <v-text-field
                  :model-value="slot.rotation?.x ?? 0"
                  @update:model-value="updateRotation(slot, 'x', $event)"
                  label="X (Pitch)"
                  type="number"
                  :readonly="!editable"
                  :variant="editable ? 'filled' : 'outlined'"
                  density="compact"
                ></v-text-field>
              </v-col>
              <v-col cols="4">
                <v-text-field
                  :model-value="slot.rotation?.y ?? 0"
                  @update:model-value="updateRotation(slot, 'y', $event)"
                  label="Y (Yaw)"
                  type="number"
                  :readonly="!editable"
                  :variant="editable ? 'filled' : 'outlined'"
                  density="compact"
                ></v-text-field>
              </v-col>
              <v-col cols="4">
                <v-text-field
                  :model-value="slot.rotation?.z ?? 0"
                  @update:model-value="updateRotation(slot, 'z', $event)"
                  label="Z (Roll)"
                  type="number"
                  :readonly="!editable"
                  :variant="editable ? 'filled' : 'outlined'"
                  density="compact"
                ></v-text-field>
              </v-col>
            </v-row>
          </v-col>
        </v-row>
      </v-card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import type { Slot } from '@/types/products';

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
  { title: 'CPU', value: 'CPU' },
  { title: 'GPU', value: 'GPU' },
  { title: 'Motherboard', value: 'Motherboard' },
  { title: 'RAM', value: 'RAM' },
  { title: 'Storage', value: 'Storage' },
  { title: 'PowerSupply', value: 'PowerSupply' },
  { title: 'Cooler', value: 'Cooler' },
  { title: 'Case', value: 'Case' }
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

watch(
  () => props.modelValue,
  (newValue) => {
    localSlots.value = newValue.map(slot => ({
      ...slot,
      relativePosition: slot.relativePosition || { ...DEFAULT_POSITION },
      maxDimensions: slot.maxDimensions || { ...DEFAULT_DIMENSIONS },
      rotation: slot.rotation ? { ...slot.rotation } : { ...DEFAULT_ROTATION }
    }));
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
  emitUpdate();
}

function removeSlot(index: number) {
  localSlots.value.splice(index, 1);
  emitUpdate();
}

function updateRotation(slot: Slot, axis: 'x' | 'y' | 'z', value: string | number) {
  if (!slot.rotation) {
    slot.rotation = { ...DEFAULT_ROTATION };
  }
  slot.rotation[axis] = typeof value === 'string' ? parseFloat(value) || 0 : value;
  emitUpdate();
}

function emitUpdate() {
  emit('update:modelValue', localSlots.value);
}
</script>

<style scoped>
.v-card {
  transition: all 0.2s ease;
}
</style>
