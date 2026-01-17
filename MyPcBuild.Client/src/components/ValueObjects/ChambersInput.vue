<template>
  <div>
    <div class="d-flex justify-space-between align-center mb-3">
      <label class="text-subtitle-2 font-weight-semibold">Chambers</label>
      <v-btn
        v-if="editable"
        size="small"
        prepend-icon="mdi-plus"
        variant="outlined"
        @click="addChamber"
      >
        Add Chamber
      </v-btn>
    </div>

    <div v-if="localChambers.length === 0" class="text-center py-4 text-medium-emphasis">
      <p class="text-body-2">No chambers defined</p>
    </div>

    <div v-else class="d-flex flex-column ga-3">
      <v-card
        v-for="(chamber, index) in localChambers"
        :key="index"
        variant="outlined"
        class="pa-3"
      >
        <div class="d-flex justify-space-between align-center mb-3">
          <h4 class="text-subtitle-1 font-weight-semibold">Chamber {{ index + 1 }}</h4>
          <v-btn
            v-if="editable"
            size="small"
            icon="mdi-delete"
            variant="text"
            color="error"
            @click="removeChamber(index)"
          ></v-btn>
        </div>

        <v-row dense>
          <v-col cols="12">
            <v-text-field
              v-model="chamber.name"
              label="Chamber Name *"
              :readonly="!editable"
              :variant="editable ? 'filled' : 'outlined'"
              density="compact"
              placeholder="e.g., Main Chamber, PSU Chamber"
              @update:model-value="emitUpdate"
            ></v-text-field>
          </v-col>
        </v-row>

        <v-row dense>
          <v-col cols="12">
            <label class="text-caption font-weight-semibold mb-1 d-block">Chamber Dimensions (mm)</label>
            <v-row dense>
              <v-col cols="4">
                <v-text-field
                  v-model.number="chamber.dimensions.length"
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
                  v-model.number="chamber.dimensions.width"
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
                  v-model.number="chamber.dimensions.height"
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

        <v-divider class="my-3"></v-divider>

        <!-- Slots within the chamber -->
        <SlotsInput
          v-model="chamber.slots"
          :editable="editable"
          label="Slots in Chamber"
          @update:model-value="emitUpdate"
        />
      </v-card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
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

const localChambers = ref<Chamber[]>(props.modelValue.map(chamber => ({
  ...chamber,
  dimensions: chamber.dimensions || { length: 0, width: 0, height: 0 },
  slots: chamber.slots || []
})));

watch(
  () => props.modelValue,
  (newValue) => {
    localChambers.value = newValue.map(chamber => ({
      ...chamber,
      dimensions: chamber.dimensions || { length: 0, width: 0, height: 0 },
      slots: chamber.slots || []
    }));
  },
  { deep: true }
);

function addChamber() {
  localChambers.value.push({
    name: '',
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

<style scoped>
.v-card {
  transition: all 0.2s ease;
}
</style>
