<template>
  <div class="slots-editor">
    <div class="flex justify-content-between align-items-center mb-2">
      <p class="text-sm text-500 m-0">Define installation slots for this product</p>
      <Button 
        label="Add Slot"
        icon="pi pi-plus"
        @click="addSlot"
        size="small"
      />
    </div>

    <div v-if="slots.length === 0" class="text-center py-3 surface-ground border-round">
      <p class="text-sm text-500 m-0">No slots defined. Click "Add Slot" to create one.</p>
    </div>

    <div v-else class="flex flex-column gap-2">
      <Card 
        v-for="(slot, index) in slots"
        :key="index"
        class="p-2"
      >
        <template #content>
          <div class="flex justify-content-between align-items-start">
            <div class="flex-grow-1">
              <div class="grid">
                <div class="col-6">
                  <label class="text-xs font-semibold">Slot Name</label>
                  <InputText 
                    v-model="slot.name"
                    placeholder="e.g., PCIe x16"
                    class="w-full"
                    size="small"
                  />
                </div>
                <div class="col-6">
                  <label class="text-xs font-semibold">Allowed Category</label>
                  <Select 
                    v-model="slot.allowedCategory"
                    :options="categories"
                    placeholder="Select category"
                    class="w-full"
                  />
                </div>
              </div>
            </div>
            <Button 
              icon="pi pi-trash"
              @click="removeSlot(index)"
              severity="danger"
              text
              size="small"
            />
          </div>
        </template>
      </Card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import Button from 'primevue/button';
import Card from 'primevue/card';
import InputText from 'primevue/inputtext';
import Select from 'primevue/select';

interface SlotData {
  name: string;
  allowedCategory: string;
}

interface Props {
  modelValue?: string;
}

const props = defineProps<Props>();
const emit = defineEmits<{
  'update:modelValue': [value: string]
}>();

const categories = ref(['CPU', 'GPU', 'RAM', 'Storage', 'Cooler', 'PSU']);
const slots = ref<SlotData[]>([]);

// Parse initial value (simplified - just stores empty array for now)
if (props.modelValue) {
  try {
    const parsed = JSON.parse(props.modelValue);
    if (Array.isArray(parsed)) {
      slots.value = parsed;
    }
  } catch {
    // Invalid JSON, keep empty
  }
}

function addSlot() {
  slots.value.push({
    name: '',
    allowedCategory: ''
  });
}

function removeSlot(index: number) {
  slots.value.splice(index, 1);
}

// Watch for changes and emit as JSON
watch(slots, (newSlots) => {
  emit('update:modelValue', JSON.stringify(newSlots));
}, { deep: true });

// Watch for external changes
watch(() => props.modelValue, (newValue) => {
  if (newValue) {
    try {
      const parsed = JSON.parse(newValue);
      if (Array.isArray(parsed)) {
        slots.value = parsed;
      }
    } catch {
      // Invalid JSON, keep current value
    }
  }
});
</script>

<style scoped>
.slots-editor label {
  display: block;
  margin-bottom: 0.25rem;
}
</style>
