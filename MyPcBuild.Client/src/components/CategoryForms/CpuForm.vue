<template>
  <div class="d-flex flex-column ga-3">
    <!-- CPU Socket -->
    <div>
      <label class="text-subtitle-2 font-weight-semibold mb-2 d-block">
        Socket <span class="text-error">*</span>
      </label>
      <v-select 
        v-model="localFields.Socket"
        :items="['AM5', 'AM4', 'LGA1700', 'LGA1200', 'LGA1151']"
        placeholder="Select CPU socket"
      ></v-select>
    </div>

    <!-- Cores -->
    <div>
      <label class="text-subtitle-2 font-weight-semibold mb-2 d-block">
        Cores <span class="text-error">*</span>
      </label>
      <v-text-field 
        v-model.number="localFields.Cores"
        type="number"
        placeholder="Number of cores"
      ></v-text-field>
    </div>

    <!-- Threads -->
    <div>
      <label class="text-subtitle-2 font-weight-semibold mb-2 d-block">
        Threads <span class="text-error">*</span>
      </label>
      <v-text-field 
        v-model.number="localFields.Threads"
        type="number"
        placeholder="Number of threads"
      ></v-text-field>
    </div>

    <!-- Base Clock -->
    <div>
      <label class="text-subtitle-2 font-weight-semibold mb-2 d-block">
        Base Clock <span class="text-error">*</span>
        <span class="text-medium-emphasis font-weight-regular">(GHz)</span>
      </label>
      <v-text-field 
        v-model.number="localFields.BaseClock"
        type="number"
        step="0.1"
        placeholder="Base clock frequency"
      ></v-text-field>
    </div>

    <!-- Boost Clock -->
    <div>
      <label class="text-subtitle-2 font-weight-semibold mb-2 d-block">
        Boost Clock <span class="text-error">*</span>
        <span class="text-medium-emphasis font-weight-regular">(GHz)</span>
      </label>
      <v-text-field 
        v-model.number="localFields.BoostClock"
        type="number"
        step="0.1"
        placeholder="Boost clock frequency"
      ></v-text-field>
    </div>

    <!-- TDP -->
    <div>
      <label class="text-subtitle-2 font-weight-semibold mb-2 d-block">
        TDP <span class="text-error">*</span>
        <span class="text-medium-emphasis font-weight-regular">(W)</span>
      </label>
      <v-text-field 
        v-model.number="localFields.TDP"
        type="number"
        placeholder="Thermal design power"
      ></v-text-field>
    </div>

    <!-- Integrated Graphics -->
    <div>
      <v-checkbox 
        v-model="localFields.IntegratedGraphics"
        label="Integrated Graphics"
      ></v-checkbox>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';

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
