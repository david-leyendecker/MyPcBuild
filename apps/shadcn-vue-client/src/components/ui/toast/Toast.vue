<template>
  <Teleport to="body">
    <div class="toast-container">
      <TransitionGroup name="toast">
        <div
          v-for="toast in toasts"
          :key="toast.id"
          :class="['toast', `toast-${toast.type}`]"
          role="alert"
          @click="removeToast(toast.id)"
        >
          <div class="toast-icon">
            <CheckCircle v-if="toast.type === 'success'" class="h-5 w-5" />
            <XCircle v-if="toast.type === 'error'" class="h-5 w-5" />
            <Info v-if="toast.type === 'info'" class="h-5 w-5" />
            <AlertTriangle v-if="toast.type === 'warning'" class="h-5 w-5" />
          </div>
          <div class="toast-message">{{ toast.message }}</div>
          <button
            class="toast-close"
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

<style scoped>
.toast-container {
  position: fixed;
  top: 1rem;
  right: 1rem;
  z-index: 9999;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  max-width: 420px;
}

.toast {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1rem;
  border-radius: 0.5rem;
  background: hsl(var(--background));
  border: 1px solid hsl(var(--border));
  box-shadow: 0 10px 15px -3px rgb(0 0 0 / 0.1), 0 4px 6px -4px rgb(0 0 0 / 0.1);
  cursor: pointer;
}

.toast-icon {
  flex-shrink: 0;
}

.toast-message {
  flex: 1;
  font-size: 0.875rem;
  line-height: 1.25rem;
}

.toast-close {
  flex-shrink: 0;
  padding: 0.25rem;
  border-radius: 0.25rem;
  opacity: 0.7;
  transition: opacity 0.2s;
}

.toast-close:hover {
  opacity: 1;
}

.toast-success {
  border-color: hsl(142.1 76.2% 36.3%);
}

.toast-success .toast-icon {
  color: hsl(142.1 76.2% 36.3%);
}

.toast-error {
  border-color: hsl(0 84.2% 60.2%);
}

.toast-error .toast-icon {
  color: hsl(0 84.2% 60.2%);
}

.toast-info {
  border-color: hsl(221.2 83.2% 53.3%);
}

.toast-info .toast-icon {
  color: hsl(221.2 83.2% 53.3%);
}

.toast-warning {
  border-color: hsl(38 92% 50%);
}

.toast-warning .toast-icon {
  color: hsl(38 92% 50%);
}

.toast-enter-active,
.toast-leave-active {
  transition: all 0.3s ease;
}

.toast-enter-from {
  opacity: 0;
  transform: translateX(100%);
}

.toast-leave-to {
  opacity: 0;
  transform: translateX(100%) scale(0.8);
}
</style>
