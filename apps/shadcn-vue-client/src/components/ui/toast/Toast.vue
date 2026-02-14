<template>
  <Teleport to="body">
    <div class="fixed top-4 right-4 z-[9999] flex flex-col gap-2 max-w-[420px]">
      <TransitionGroup
        enter-active-class="transition-all duration-300 ease-out"
        enter-from-class="opacity-0 translate-x-full"
        leave-active-class="transition-all duration-300 ease-out"
        leave-to-class="opacity-0 translate-x-full scale-[0.8]"
      >
        <div
          v-for="toast in toasts"
          :key="toast.id"
          :class="[
            'flex items-center gap-3 p-4 rounded-lg bg-background border shadow-lg cursor-pointer',
            {
              'border-green-600': toast.type === 'success',
              'border-destructive': toast.type === 'error',
              'border-blue-500': toast.type === 'info',
              'border-yellow-500': toast.type === 'warning',
            }
          ]"
          role="alert"
          @click="removeToast(toast.id)"
        >
          <div
            :class="[
              'flex-shrink-0',
              {
                'text-green-600': toast.type === 'success',
                'text-destructive': toast.type === 'error',
                'text-blue-500': toast.type === 'info',
                'text-yellow-500': toast.type === 'warning',
              }
            ]"
          >
            <CheckCircle v-if="toast.type === 'success'" class="h-5 w-5" />
            <XCircle v-if="toast.type === 'error'" class="h-5 w-5" />
            <Info v-if="toast.type === 'info'" class="h-5 w-5" />
            <AlertTriangle v-if="toast.type === 'warning'" class="h-5 w-5" />
          </div>
          <div class="flex-1 text-sm leading-5">{{ toast.message }}</div>
          <button
            class="flex-shrink-0 p-1 rounded opacity-70 hover:opacity-100 transition-opacity"
            @click.stop="removeToast(toast.id)"
            aria-label="Close notification"
          >
            <X class="h-4 w-4" />
          </button>
        </div>
      </TransitionGroup>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
import { CheckCircle, XCircle, Info, AlertTriangle, X } from 'lucide-vue-next';
import { useToast } from '@/composables/useToast';

const { toasts, removeToast } = useToast();
</script>
