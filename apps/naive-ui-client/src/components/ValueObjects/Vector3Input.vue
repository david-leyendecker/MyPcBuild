<template>
  <div>
    <label v-if="label" style="font-size: 12px; font-weight: 600; display: block; margin-bottom: 4px;">
      {{ label }}
    </label>
    <n-input-group>
      <n-input-group-label>X</n-input-group-label>
      <n-input-number
        :value="modelValue?.x ?? 0"
        :placeholder="placeholders.x"
        :readonly="!editable"
        style="flex: 1; min-width: 80px;"
        @update:value="onUpdate('x', $event)"
      />
      <n-input-group-label>Y</n-input-group-label>
      <n-input-number
        :value="modelValue?.y ?? 0"
        :placeholder="placeholders.y"
        :readonly="!editable"
        style="flex: 1; min-width: 80px;"
        @update:value="onUpdate('y', $event)"
      />
      <n-input-group-label>Z</n-input-group-label>
      <n-input-number
        :value="modelValue?.z ?? 0"
        :placeholder="placeholders.z"
        :readonly="!editable"
        style="flex: 1; min-width: 80px;"
        @update:value="onUpdate('z', $event)"
      />
    </n-input-group>
  </div>
</template>

<script setup lang="ts">
import { NInputNumber, NInputGroup, NInputGroupLabel } from 'naive-ui';

interface Vector3 { x: number; y: number; z: number }

interface Props {
  modelValue?: Vector3;
  editable?: boolean;
  label?: string;
  placeholders?: { x?: string; y?: string; z?: string };
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => ({ x: 0, y: 0, z: 0 }),
  editable: true,
  label: '',
  placeholders: () => ({ x: 'X', y: 'Y', z: 'Z' })
});

const emit = defineEmits<{
  'update:modelValue': [value: Vector3]
}>();

function onUpdate(axis: 'x' | 'y' | 'z', value: number | null) {
  const next: Vector3 = {
    x: props.modelValue?.x ?? 0,
    y: props.modelValue?.y ?? 0,
    z: props.modelValue?.z ?? 0
  };
  next[axis] = value ?? 0;
  emit('update:modelValue', next);
}
</script>
