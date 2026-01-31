<template>
  <n-grid :cols="3" :x-gap="12">
    <n-form-item-gi label="X (Pitch)">
      <n-input-number
        :value="(modelValue?.x ?? 0)"
        :placeholder="placeholders.x"
        :disabled="!editable"
        style="width: 100%"
        @update:value="onUpdate('x', $event)"
      />
    </n-form-item-gi>
    <n-form-item-gi label="Y (Yaw)">
      <n-input-number
        :value="(modelValue?.y ?? 0)"
        :placeholder="placeholders.y"
        :disabled="!editable"
        style="width: 100%"
        @update:value="onUpdate('y', $event)"
      />
    </n-form-item-gi>
    <n-form-item-gi label="Z (Roll)">
      <n-input-number
        :value="(modelValue?.z ?? 0)"
        :placeholder="placeholders.z"
        :disabled="!editable"
        style="width: 100%"
        @update:value="onUpdate('z', $event)"
      />
    </n-form-item-gi>
  </n-grid>
</template>

<script setup lang="ts">
import { NInputNumber, NFormItemGi, NGrid } from 'naive-ui';
import type { Rotation } from '@/types/products';

interface Props {
  modelValue?: Rotation | null;
  editable?: boolean;
  placeholders?: { x?: string; y?: string; z?: string };
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => ({ x: 0, y: 0, z: 0 }),
  editable: true,
  placeholders: () => ({ x: 'X (Pitch)', y: 'Y (Yaw)', z: 'Z (Roll)' })
});

const emit = defineEmits<{
  'update:modelValue': [value: Rotation]
}>();

function onUpdate(axis: 'x' | 'y' | 'z', value: number | null) {
  const next: Rotation = {
    x: props.modelValue?.x ?? 0,
    y: props.modelValue?.y ?? 0,
    z: props.modelValue?.z ?? 0
  };
  next[axis] = value ?? 0;
  emit('update:modelValue', next);
}
</script>
