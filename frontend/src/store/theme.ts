import { create } from 'zustand';

type ThemeMode = 'light' | 'dark';

type ThemeState = {
  mode: ThemeMode;
  toggleMode: () => void;
  setMode: (mode: ThemeMode) => void;
};

export const useThemeStore = create<ThemeState>((set) => ({
  mode: (localStorage.getItem('theme') as ThemeMode) || 'light',
  toggleMode: () => set((state) => {
    const newMode = state.mode === 'light' ? 'dark' : 'light';
    localStorage.setItem('theme', newMode);
    return { mode: newMode };
  }),
  setMode: (mode) => {
    localStorage.setItem('theme', mode);
    set({ mode });
  },
}));
