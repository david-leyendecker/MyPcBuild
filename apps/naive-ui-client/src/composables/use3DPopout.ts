import { ref, shallowRef, type Component } from 'vue';

interface PopoutContent {
  component: Component;
  props?: Record<string, any>;
  title?: string;
}

const isPopoutOpen = ref(false);
const popoutContent = shallowRef<PopoutContent | null>(null);

export function use3DPopout() {
  function openPopout(content: PopoutContent) {
    popoutContent.value = content;
    isPopoutOpen.value = true;
  }

  function closePopout() {
    isPopoutOpen.value = false;
    // Delay clearing content to allow close animation
    setTimeout(() => {
      popoutContent.value = null;
    }, 300);
  }

  function updatePopoutProps(props: Record<string, any>) {
    if (popoutContent.value) {
      popoutContent.value = {
        ...popoutContent.value,
        props: {
          ...popoutContent.value.props,
          ...props,
        },
      };
    }
  }

  return {
    isPopoutOpen,
    popoutContent,
    openPopout,
    closePopout,
    updatePopoutProps,
  };
}
