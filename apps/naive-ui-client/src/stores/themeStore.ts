import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useThemeStore = defineStore('theme', () => {
  const isDark = ref<boolean>(true);

  function toggleTheme() {
    isDark.value = !isDark.value;
    // Save preference to localStorage
    localStorage.setItem('theme', isDark.value ? 'dark' : 'light');
  }

  function initTheme() {
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme) {
      isDark.value = savedTheme === 'dark';
    } else {
      // Default to dark theme
      isDark.value = true;
    }
  }

  return {
    isDark,
    toggleTheme,
    initTheme
  };
});
