import { useThemeStore } from '../store/theme';

export function useTheme() {
  const { mode, toggleMode } = useThemeStore();
  
  return {
    mode,
    toggleMode,
    isDark: mode === 'dark',
    isLight: mode === 'light',
  };
}
