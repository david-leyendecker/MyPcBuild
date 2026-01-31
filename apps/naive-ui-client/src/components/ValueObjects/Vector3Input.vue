<template>
  <n-grid :cols="3" :x-gap="12">
    <n-form-item-gi label="X">
      <n-input-number
        :value="modelValue?.x ?? 0"
        :placeholder="placeholders.x"
        :disabled="!editable"
        style="width: 100%"
        @update:value="onUpdate('x', $event)"
      />
    </n-form-item-gi>
    <n-form-item-gi label="Y">
      <n-input-number
        :value="modelValue?.y ?? 0"
        :placeholder="placeholders.y"
        :disabled="!editable"
        style="width: 100%"
        @update:value="onUpdate('y', $event)"
      />
    </n-form-item-gi>
    <n-form-item-gi label="Z">
      <n-input-number
        :value="modelValue?.z ?? 0"
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

interface Vector3 { x: number; y: number; z: number }

interface Props {
  modelValue?: Vector3;
  editable?: boolean;
  placeholders?: { x?: string; y?: string; z?: string };
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => ({ x: 0, y: 0, z: 0 }),
  editable: true,
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
