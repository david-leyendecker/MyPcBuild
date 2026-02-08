import { defineStore } from 'pinia'
import { ref, watch } from 'vue'

export type ThemeMode = 'light' | 'dark' | 'system'

export const useThemeStore = defineStore('theme', () => {
  const mode = ref<ThemeMode>((localStorage.getItem('mypcbuild-theme') as ThemeMode) || 'system')

  const applyTheme = () => {
    const root = document.documentElement
    root.classList.remove('light', 'dark')

    if (mode.value === 'system') {
      const systemPreference = window.matchMedia('(prefers-color-scheme: dark)').matches
        ? 'dark'
        : 'light'
      root.classList.add(systemPreference)
    } else {
      root.classList.add(mode.value)
    }
  }

  const setMode = (newMode: ThemeMode) => {
    mode.value = newMode
    localStorage.setItem('mypcbuild-theme', newMode)
    applyTheme()
  }

  const initTheme = () => {
    applyTheme()

    // Listen for system theme changes
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', () => {
      if (mode.value === 'system') {
        applyTheme()
      }
    })
  }

  watch(mode, () => {
    applyTheme()
  })

  return {
    mode,
    setMode,
    initTheme,
  }
})
