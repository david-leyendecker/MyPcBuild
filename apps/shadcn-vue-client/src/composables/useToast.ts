import { ref } from 'vue';

export interface ToastMessage {
  id: string;
  type: 'success' | 'error' | 'info' | 'warning';
  message: string;
  duration?: number;
}

const toasts = ref<ToastMessage[]>([]);
let idCounter = 0;

export function useToast() {
  function addToast(type: ToastMessage['type'], message: string, duration: number = 3000) {
    const id = `toast-${++idCounter}`;
    const toast: ToastMessage = { id, type, message, duration };
    
    toasts.value.push(toast);

    if (duration > 0) {
      setTimeout(() => {
        removeToast(id);
      }, duration);
    }

    return id;
  }

  function removeToast(id: string) {
    const index = toasts.value.findIndex(t => t.id === id);
    if (index !== -1) {
      toasts.value.splice(index, 1);
    }
  }

  function success(message: string, duration?: number) {
    return addToast('success', message, duration);
  }

  function error(message: string, duration?: number) {
    return addToast('error', message, duration);
  }

  function info(message: string, duration?: number) {
    return addToast('info', message, duration);
  }

  function warning(message: string, duration?: number) {
    return addToast('warning', message, duration);
  }

  return {
    toasts,
    success,
    error,
    info,
    warning,
    removeToast
  };
}
