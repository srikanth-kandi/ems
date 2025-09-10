import { create } from 'zustand';

type AuthState = {
  token: string | null;
  username: string | null;
  role: string | null;
  setAuth: (payload: { token: string; username: string; role: string }) => void;
  logout: () => void;
};

export const useAuthStore = create<AuthState>((set) => ({
  token: localStorage.getItem('token'),
  username: localStorage.getItem('username'),
  role: localStorage.getItem('role'),
  setAuth: ({ token, username, role }) => {
    localStorage.setItem('token', token);
    localStorage.setItem('username', username);
    localStorage.setItem('role', role);
    set({ token, username, role });
  },
  logout: () => {
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    localStorage.removeItem('role');
    set({ token: null, username: null, role: null });
  },
}));


