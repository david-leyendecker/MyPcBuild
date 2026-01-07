<template>
  <div class="flex flex-column gap-3">
    <!-- CPU Socket -->
    <div class="field">
      <label for="socket" class="font-semibold">
        Socket <span class="text-red-500">*</span>
      </label>
      <Select 
        id="socket"
        v-model="localFields.Socket"
        :options="['AM5', 'AM4', 'LGA1700', 'LGA1200', 'LGA1151']"
        placeholder="Select CPU socket"
        class="w-full"
      />
    </div>

    <!-- Cores -->
    <div class="field">
      <label for="cores" class="font-semibold">
        Cores <span class="text-red-500">*</span>
      </label>
      <InputNumber 
        id="cores"
        v-model="localFields.Cores"
        placeholder="Number of cores"
        class="w-full"
        :min="1"
      />
    </div>

    <!-- Threads -->
    <div class="field">
      <label for="threads" class="font-semibold">
        Threads <span class="text-red-500">*</span>
      </label>
      <InputNumber 
        id="threads"
        v-model="localFields.Threads"
        placeholder="Number of threads"
        class="w-full"
        :min="1"
      />
    </div>

    <!-- Base Clock -->
    <div class="field">
      <label for="baseClock" class="font-semibold">
        Base Clock <span class="text-red-500">*</span>
        <span class="text-500 font-normal">(GHz)</span>
      </label>
      <InputNumber 
        id="baseClock"
        v-model="localFields.BaseClock"
        placeholder="Base clock frequency"
        class="w-full"
        :min="0"
        :step="0.1"
      />
    </div>

    <!-- Boost Clock -->
    <div class="field">
      <label for="boostClock" class="font-semibold">
        Boost Clock <span class="text-red-500">*</span>
        <span class="text-500 font-normal">(GHz)</span>
      </label>
      <InputNumber 
        id="boostClock"
        v-model="localFields.BoostClock"
        placeholder="Boost clock frequency"
        class="w-full"
        :min="0"
        :step="0.1"
      />
    </div>

    <!-- TDP -->
    <div class="field">
      <label for="tdp" class="font-semibold">
        TDP <span class="text-red-500">*</span>
        <span class="text-500 font-normal">(W)</span>
      </label>
      <InputNumber 
        id="tdp"
        v-model="localFields.TDP"
        placeholder="Thermal design power"
        class="w-full"
        :min="0"
      />
    </div>

    <!-- Integrated Graphics -->
    <div class="field">
      <label for="integratedGraphics" class="font-semibold flex align-items-center gap-2">
        <Checkbox 
          id="integratedGraphics"
          v-model="localFields.IntegratedGraphics"
          :binary="true"
        />
        Integrated Graphics
      </label>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import InputNumber from 'primevue/inputnumber';
import Select from 'primevue/select';
import Checkbox from 'primevue/checkbox';

interface Props {
  modelValue: Record<string, string>;
}

const props = defineProps<Props>();
const emit = defineEmits<{
  'update:modelValue': [value: Record<string, string>]
}>();

const localFields = ref<Record<string, any>>({
  Socket: '',
  Cores: 8,
  Threads: 16,
  BaseClock: 3.5,
  BoostClock: 5.0,
  TDP: 105,
  IntegratedGraphics: false,
  ...props.modelValue
});

watch(localFields, (newFields) => {
  const stringValues: Record<string, string> = {};
  Object.entries(newFields).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== '') {
      stringValues[key] = String(value);
    }
  });
  emit('update:modelValue', stringValues);
}, { deep: true });
</script>

<style scoped>
.field {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}
</style>
